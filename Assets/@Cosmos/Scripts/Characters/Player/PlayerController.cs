using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 이동 및 타일 상호작용 관리
/// </summary>
public class PlayerController : MonoBehaviour
{
    private float _moveSpeed = 5f;
    private GridManager _gridManager;
    private int _currentX, _currentY;
    private bool _isMoving;
    private float _moveTime = 0.2f;

    // 상태이상 처리용
    private PlayerDebuff _playerDebuff;

    // 애니메이션 관련
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    // 방향 기억용
    private bool _facingRight = false; // 기본은 왼쪽
    
    // Getters & Setters
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public bool IsMoving { get => _isMoving; }
    public int CurrentX { get => _currentX; set => _currentX = value; }
    public int CurrentY { get => _currentY; set => _currentY = value; }
    public Animator Animator { get => _animator; set => _animator = value; }
    public PlayerDebuff PlayerDebuff => _playerDebuff;

    private void Start()
    {
        _gridManager = GridManager.Instance;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerDebuff = GetComponent<PlayerDebuff>();
        UpdateCurrentPosition();

        SoundManager.Instance.PlayPlayerSound("StickRoll");

        // 초기 애니메이션 상태 설정
        if (_animator != null)
        {
            _animator.SetBool("IsMoving", false);
        }
    }
    
    private void Update()
    {
        // 게임이 Playing 상태일 때만 입력 처리
        if (GameStateManager.Instance.CurrentState == GameStateManager.GameState.Playing)
        {
            if (!_isMoving && !_playerDebuff.IsBind)
            {
                HandleMovement();
            }
            CheckTileInteraction();
        }
        else
        {
            // 게임이 Playing 상태가 아닐 때는 IsMoving을 false로 고정
            if (_animator != null)
            {
                _animator.SetBool("IsMoving", false);
            }
        }
    }
    
    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
    
        if (horizontal != 0 || vertical != 0)
        {
            int dx = Mathf.RoundToInt(horizontal);
            int dy = Mathf.RoundToInt(vertical);
            
            // 방향 전환 처리
            HandleFlip(dx);
            
            TryMove(dx, dy);
        }
    }
    
    /// <summary>
    /// 캐릭터 방향 전환 처리
    /// </summary>
    private void HandleFlip(int horizontalInput)
    {
        if (horizontalInput > 0 && !_facingRight) // 오른쪽 이동 + 현재 왼쪽 보고 있을 때
        {
            _facingRight = true;
            _spriteRenderer.flipX = true;
        }
        else if (horizontalInput < 0 && _facingRight) // 왼쪽 이동 + 현재 오른쪽 보고 있을 때
        {
            _facingRight = false;
            _spriteRenderer.flipX = false;
        }
        // 수직 이동이나 같은 방향 이동시에는 방향 유지
    }
    
    /// <summary>
    /// 이동 시도 - 유효한 위치인지 확인 후 애니메이션
    /// </summary>
    private bool TryMove(int dx, int dy)
    {
        int newX = _currentX + dx;
        int newY = _currentY + dy;
        Vector3Int pos = new Vector3Int(newX, newY, 0);
        
        if (_gridManager.IsWithinGrid(pos))
        {
            StartCoroutine(MoveAnimation(pos));
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 점프 효과가 있는 이동 애니메이션
    /// 이동 중간에 논리적 위치를 옮김
    /// </summary>
    private IEnumerator MoveAnimation(Vector3Int newPos)
    {
        _isMoving = true;
        _animator.SetBool("IsMoving", true);
    
        Vector3 startPos = transform.position;
        Vector3 targetPos = GridManager.Instance.GridToWorldPosition(newPos);
        float jumpHeight = 0.3f;
    
        float elapsedTime = 0;
        bool positionUpdated = false; // 논리적 위치 업데이트 여부
        float midPoint = _moveTime * 0.5f; // 애니메이션 중간 지점 (0.1초)
    
        while (elapsedTime < _moveTime)
        {
            float t = elapsedTime / _moveTime;
        
            // XY 평면에서 이동 (Z는 항상 0)
            float x = Mathf.Lerp(startPos.x, targetPos.x, t);
            float y = Mathf.Lerp(startPos.y, targetPos.y, t);
        
            // 점프 높이 계산
            float extraHeight = Mathf.Sin(t * Mathf.PI) * jumpHeight;
        
            transform.position = new Vector3(x, y + extraHeight, 0);
            
            // 애니메이션 중간 지점에서 논리적 위치 업데이트
            if (!positionUpdated && elapsedTime >= midPoint)
            {
                _currentX = newPos.x;
                _currentY = newPos.y;
                positionUpdated = true;
                
                // 디버그용 로그 (필요시 제거)
                Debug.Log($"논리적 위치 업데이트: ({_currentX}, {_currentY}) at {elapsedTime:F2}s");
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 설정 (물리적 위치만)
        transform.position = targetPos;
        
        // 논리적 위치가 업데이트되지 않은 경우 (프레임 드랍 등으로 인해)
        if (!positionUpdated)
        {
            _currentX = newPos.x;
            _currentY = newPos.y;
        }
        
        _isMoving = false;
        _animator.SetBool("IsMoving", false);

        SoundManager.Instance.PlayPlayerSound("PlayerMove");
    }
    /// <summary>
    /// 현재 그리드 위치 업데이트
    /// </summary>
    private void UpdateCurrentPosition()
    {
        Vector3Int pos = _gridManager.WorldToGridPosition(transform.position);

        _currentX = pos.x;
        _currentY = pos.y;
    }
    
    /// <summary>
    /// 애니메이션 이벤트에서 호출 - 지팡이가 땅을 찍는 순간
    /// </summary>
    public void OnStaffHitGround()
    {
        SoundManager.Instance.PlayPlayerSound("AriaActive");

        FindAnyObjectByType<GameManager>().SpawnGroundEffect();
    }
    
    /// <summary>
    /// 현재 위치 타일과 상호작용 확인
    /// </summary>
    private void CheckTileInteraction()
    {
        Vector3Int currentPos = new Vector3Int(_currentX, _currentY, 0); // 1. 현재 위치 가져오기 (_currentX, _currentY) 는 이미 grid 좌표로 설정되어 있음
        Cell currentCell = _gridManager.GetCellData(currentPos);
        CombineCell comCell = currentCell?.GetObjectData();
        comCell?.ExecuteSkill();
        /*if (currentTile != null && currentTile.GetState() == BaseTile.TileState.Ready)
        {
            
        }*/
    }

    /// <summary>
    /// 플레이어 속박
    /// </summary>
    /// <param name="time"></param>
    public void Bind(float time)
    {
        StartCoroutine(_playerDebuff.Bind(time));
    }
}