using UnityEngine;
using System.Collections;
using System; // InputManager 이벤트를 위해 추가

/// <summary>
/// 최종 정리된 플레이어 이동 및 상호작용 관리 스크립트
/// </summary>
public class PlayerController : MonoBehaviour
{
    // --- 이동 관련 설정 ---
    [Header("Movement Settings")]
    [SerializeField] private float _moveTime = 0.2f;

    // --- 입력 처리 관련 설정 ---
    [Header("Input Settings")]
    [Tooltip("키보드 대각선 입력을 위한 유예 시간 (초)")]
    [SerializeField] private float _inputGraceTime = 0.05f;
    [Tooltip("첫 이동 후, 반복 이동이 시작되기까지의 대기 시간 (초)")]
    [SerializeField] private float _initialMoveDelay = 0.3f;
    [Tooltip("반복 이동 시의 각 이동 사이의 대기 시간 (초)")]
    [SerializeField] private float _repeatMoveDelay = 0.15f;

    // --- 내부 상태 변수 ---
    private Vector2Int _heldDirection;
    private bool _isHolding = false;
    private bool _hasMovedOnceInSequence = false; // 한 시퀀스에서 첫 이동을 했는지 체크
    [SerializeField]
    private float _moveTimer = 0f;
    [SerializeField]
    private bool _isMoving = false;
    private bool _facingRight = true;
    private int _currentX, _currentY;

    // --- 컴포넌트 및 매니저 참조 ---
    private GridManager _gridManager;
    private PlayerDebuff _playerDebuff;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    // --- 갸우뚱 효과 관련 ---
    [Header("Wobble Effect")]
    [SerializeField] private float _wobbleAmount = 15f;
    private bool _wobbleLeft = true;
    [SerializeField] private bool _enableWobble = true;

    // Getters & Setters
    public bool IsMoving => _isMoving;
    public int CurrentX { get => _currentX; set => _currentX = value; }
    public int CurrentY { get => _currentY; set => _currentY = value; }
    public Animator Animator => _animator;
    public PlayerDebuff PlayerDebuff => _playerDebuff;

    //======================================================================
    // Unity 생명주기 및 이벤트 연결
    //======================================================================

    private void Awake()
    {
        _gridManager = GridManager.Instance;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerDebuff = GetComponent<PlayerDebuff>();
    }

    private void Start()
    {
        UpdateCurrentPosition();
        if (_spriteRenderer != null) _spriteRenderer.flipX = true;
        if (_animator != null) _animator.SetBool("IsMoving", false);
        // SoundManager.Instance.PlayPlayerSound("StickRoll"); // 필요 시 주석 해제
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMove += HandleMoveInput;
        }
    }



    // ★★★ 추가된 부분 (2) ★★★
    // 메모리 누수 방지를 위해 이벤트를 해제합니다.
    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMove -= HandleMoveInput;
        }
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Playing)
        {
            if (_animator != null) _animator.SetBool("IsMoving", false);
            return;
        }

        // --- 최종 입력 로직 ---
        if (_heldDirection == Vector2Int.zero)
        {
            _isHolding = false;
            _hasMovedOnceInSequence = false;
            return;
        }

        if (!_isHolding)
        {
            _isHolding = true;
            _moveTimer = _inputGraceTime; // '입력 유예' 타이머 시작
        }

        if (_moveTimer > 0)
        {
            _moveTimer -= Time.deltaTime;
        }

        if (!_isMoving && !_playerDebuff.IsBind && _moveTimer <= 0)
        {
            ExecuteMovement(_heldDirection);

            if (!_hasMovedOnceInSequence)
            {
                _hasMovedOnceInSequence = true;
                _moveTimer = _initialMoveDelay; // 첫 이동 후, 긴 딜레이 설정
            }
            else
            {
                _moveTimer = _repeatMoveDelay; // 반복 이동 시, 짧은 딜레이 설정
            }
        }
    }

    //======================================================================
    // 입력 및 이동 실행
    //======================================================================
    
    // InputManager로부터 입력이 들어올 때마다 호출됩니다.
    private void HandleMoveInput(Vector2 direction)
    {
        _heldDirection = new Vector2Int(Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
    }

    private void ExecuteMovement(Vector2Int direction)
    {
        HandleFlip(direction.x);
        TryMove(direction.x, direction.y);
    }

    private void HandleFlip(int horizontalInput)
    {
        if (horizontalInput > 0 && !_facingRight)
        {
            _facingRight = true;
            _spriteRenderer.flipX = true;
        }
        else if (horizontalInput < 0 && _facingRight)
        {
            _facingRight = false;
            _spriteRenderer.flipX = false;
        }
    }
    
    private bool TryMove(int dx, int dy)
    {
        int newX = _currentX + dx;
        int newY = _currentY + dy;
        Vector3Int newGridPos = new Vector3Int(newX, newY, 0);

        if (_gridManager.IsWithinGrid(newGridPos) && !_gridManager.UnmovableGridPositions.Contains(newGridPos))
        {
            StartCoroutine(MoveAnimation(newGridPos));
            return true;
        }
        return false;
    }

    //======================================================================
    // 이동 애니메이션 및 기타 로직
    //======================================================================

    private IEnumerator MoveAnimation(Vector3Int newPos)
    {
        _isMoving = true;
        _animator.SetBool("IsMoving", true);
        
        Vector3 startPos = transform.position;
        Vector3 targetPos = _gridManager.GridToWorldPosition(newPos);

        _currentX = newPos.x;
        _currentY = newPos.y;

        float jumpHeight = 0.3f;
        float elapsedTime = 0;
        
        // SoundManager.Instance.PlayPlayerSound("PlayerMove"); // 필요 시 주석 해제

        while (elapsedTime < _moveTime)
        {
            float t = elapsedTime / _moveTime;
            float x = Mathf.Lerp(startPos.x, targetPos.x, t);
            float y = Mathf.Lerp(startPos.y, targetPos.y, t);
            float extraHeight = Mathf.Sin(t * Mathf.PI) * jumpHeight;

            float wobbleRotation = 0f;
            if (_enableWobble)
            {
                float wobbleAmount = Mathf.Sin(t * Mathf.PI) * _wobbleAmount;
                wobbleRotation = _wobbleLeft ? -wobbleAmount : wobbleRotation;
            }

            transform.position = new Vector3(x, y + extraHeight, 0);
            transform.rotation = Quaternion.Euler(0, 0, wobbleRotation);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        transform.rotation = Quaternion.identity;
        _wobbleLeft = !_wobbleLeft;

        _isMoving = false;
        _animator.SetBool("IsMoving", false);
        
        CheckTileInteraction();
    }
    
    private void UpdateCurrentPosition()
    {
        Vector3Int pos = _gridManager.WorldToGridPosition(transform.position);
        _currentX = pos.x;
        _currentY = pos.y;
    }
    
    private void CheckTileInteraction()
    {
        Vector3Int currentPos = new Vector3Int(_currentX, _currentY, 0);
        Cell currentCell = _gridManager.GetCellData(currentPos);
        currentCell?.GetCombineCell()?.ExecuteSkill();
    }

    public void OnStaffHitGround()
    {
        // SoundManager.Instance.PlayPlayerSound("AriaActive"); // 필요 시 주석 해제
        FindAnyObjectByType<StageHandler>().SpawnGroundEffect();
    }

    public void Bind(float time)
    {
        StartCoroutine(_playerDebuff.Bind(time));
    }
    
    public void SetWobbleEffect(bool enable) => _enableWobble = enable;
    public void SetWobbleAmount(float amount) => _wobbleAmount = amount;
}