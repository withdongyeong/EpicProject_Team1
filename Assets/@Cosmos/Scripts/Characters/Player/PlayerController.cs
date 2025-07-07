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
    
    // 입력 버퍼링 시스템
    private float _inputBufferTime = 0.05f; // 입력을 수집할 시간
    private float _inputBufferTimer = 0f;
    private Vector2 _bufferedInput = Vector2.zero;
    private bool _hasBufferedInput = false;
    
    // 갸우뚱 효과 관련
    [Header("Wobble Effect")]
    private float _wobbleAmount = 15f; // 갸우뚱 각도 (도 단위)

    private bool _wobbleLeft = true;
    private bool _enableWobble = true; // 갸우뚱 효과 활성화 여부
    
    // Getters & Setters
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public bool IsMoving { get => _isMoving; }
    public int CurrentX { get => _currentX; set => _currentX = value; }
    public int CurrentY { get => _currentY; set => _currentY = value; }
    public Animator Animator { get => _animator; set => _animator = value; }
    public PlayerDebuff PlayerDebuff => _playerDebuff;

    //스킬을 발동할 수 있는지 여부입니다. 
    private bool _canInteractionTile = false;

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
        if (GameStateManager.Instance.CurrentState == GameState.Playing)
        {
            if (!_isMoving && !_playerDebuff.IsBind)
            {
                HandleMovementWithBuffer();
            }
            if(_canInteractionTile)
            {
                CheckTileInteraction();
            }
            
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
    
    /// <summary>
    /// 입력 버퍼링을 사용한 이동 처리
    /// </summary>
    private void HandleMovementWithBuffer()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        Vector2 currentInput = new Vector2(horizontal, vertical);
        
        // 새로운 입력이 들어왔을 때
        if (currentInput.magnitude > 0)
        {
            if (!_hasBufferedInput)
            {
                // 첫 번째 입력 - 버퍼링 시작
                _bufferedInput = currentInput;
                _hasBufferedInput = true;
                _inputBufferTimer = _inputBufferTime;
            }
            else
            {
                // 추가 입력 - 기존 입력과 합성
                _bufferedInput = CombineInputs(_bufferedInput, currentInput);
            }
        }
        
        // 버퍼 타이머 처리
        if (_hasBufferedInput)
        {
            _inputBufferTimer -= Time.deltaTime;
            
            // 버퍼 시간이 끝났거나, 입력이 없어졌을 때 이동 실행
            if (_inputBufferTimer <= 0 || currentInput.magnitude == 0)
            {
                ExecuteBufferedMovement();
            }
        }
    }
    
    /// <summary>
    /// 두 입력을 합성하여 최종 이동 방향 결정
    /// </summary>
    private Vector2 CombineInputs(Vector2 input1, Vector2 input2)
    {
        // 각 축에서 가장 최근의 0이 아닌 입력을 사용
        float finalX = Mathf.Abs(input2.x) > 0 ? input2.x : input1.x;
        float finalY = Mathf.Abs(input2.y) > 0 ? input2.y : input1.y;
        
        return new Vector2(finalX, finalY);
    }
    
    /// <summary>
    /// 버퍼된 입력을 실행
    /// </summary>
    private void ExecuteBufferedMovement()
    {
        if (_bufferedInput.magnitude > 0)
        {
            int dx = Mathf.RoundToInt(_bufferedInput.x);
            int dy = Mathf.RoundToInt(_bufferedInput.y);
            
            // 방향 전환 처리
            HandleFlip(dx);
            
            TryMove(dx, dy);
        }
        
        // 버퍼 리셋
        _bufferedInput = Vector2.zero;
        _hasBufferedInput = false;
        _inputBufferTimer = 0f;
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
        
        bool isMovable = _gridManager.UnmovableGridPositions.Contains(pos) == false; // 이동 불가능한 위치인지 확인
        if (_gridManager.IsWithinGrid(pos) && isMovable)
        {
            StartCoroutine(MoveAnimation(pos));
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 점프 효과가 있는 이동 애니메이션 (갸우뚱 효과 포함)
    /// 이동 중간에 논리적 위치를 옮김
    /// </summary>
    private IEnumerator MoveAnimation(Vector3Int newPos)
    {
        _isMoving = true;
        _animator.SetBool("IsMoving", true);
        _canInteractionTile = false;
    
        Vector3 startPos = transform.position;
        Vector3 targetPos = GridManager.Instance.GridToWorldPosition(newPos);
        
        // 이동 방향 계산
        Vector3 moveDirection = targetPos - startPos;
        
        // 이동 전에 논리적 위치를 미리 옮김
        // 따라서, 공격 판정이 오기 전에 아슬아슬하게 피했을 때, 이미 점프를 시작했으면 안 맞게 되면서 피하는 느낌이 들 것
        _currentX = newPos.x;
        _currentY = newPos.y;
        
        float jumpHeight = 0.3f;
    
        float elapsedTime = 0;
    
        while (elapsedTime < _moveTime)
        {
            float t = elapsedTime / _moveTime;
        
            // XY 평면에서 이동 (Z는 항상 0)
            float x = Mathf.Lerp(startPos.x, targetPos.x, t);
            float y = Mathf.Lerp(startPos.y, targetPos.y, t);
        
            //점프의 반이 지났을때부터 타일과의 상호작용을 가능하게 합니다.
            if(t > 0.5f)
            {
                _canInteractionTile = true;
            }
            
            // 점프 높이 계산
            float extraHeight = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            
            // 갸우뚱 효과 계산 (점프 중에만 적용)
            float wobbleRotation = 0f;
            if (_enableWobble)
            {
                // 점프 궤도와 같은 사인파를 사용하여 갸우뚱 효과
                // 점프 시작과 끝에서는 0도, 중간에서 최대 각도
                float wobbleAmount = Mathf.Sin(t * Mathf.PI) * _wobbleAmount;
                
                // 번갈아가며 갸우뚱 방향 결정
                wobbleRotation = _wobbleLeft ? -wobbleAmount : wobbleAmount;
            }
            
            // 위치 설정
            transform.position = new Vector3(x, y + extraHeight, 0);
            
            // 회전 설정 (갸우뚱 효과)
            transform.rotation = Quaternion.Euler(0, 0, wobbleRotation);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 설정 (물리적 위치만)
        transform.position = targetPos;
        
        // 회전 초기화
        transform.rotation = Quaternion.identity;

        // 다음 이동을 위해 갸우뚱 방향 토글
        _wobbleLeft = !_wobbleLeft;

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

        FindAnyObjectByType<StageHandler>().SpawnGroundEffect();
    }
    
    /// <summary>
    /// 현재 위치 타일과 상호작용 확인
    /// </summary>
    private void CheckTileInteraction()
    {
        Vector3Int currentPos = new Vector3Int(_currentX, _currentY, 0); // 1. 현재 위치 가져오기 (_currentX, _currentY) 는 이미 grid 좌표로 설정되어 있음
        Cell currentCell = _gridManager.GetCellData(currentPos);
        CombineCell comCell = currentCell?.GetCombineCell();
        comCell?.ExecuteSkill();

    }

    /// <summary>
    /// 플레이어 속박
    /// </summary>
    /// <param name="time"></param>
    public void Bind(float time)
    {
        StartCoroutine(_playerDebuff.Bind(time));
    }
    
    /// <summary>
    /// 갸우뚱 효과 활성화/비활성화
    /// </summary>
    /// <param name="enable">활성화 여부</param>
    public void SetWobbleEffect(bool enable)
    {
        _enableWobble = enable;
    }
    
    /// <summary>
    /// 갸우뚱 효과 강도 조정
    /// </summary>
    /// <param name="amount">갸우뚱 각도 (도 단위)</param>
    public void SetWobbleAmount(float amount)
    {
        _wobbleAmount = amount;
    }
    
    /// <summary>
    /// 갸우뚱 효과 속도 조정 (사용 안 함 - 점프와 동일한 속도)
    /// </summary>
    /// <param name="speed">갸우뚱 속도</param>
    public void SetWobbleSpeed(float speed)
    {
        // 점프와 동일한 속도로 갸우뚱하므로 사용하지 않음
    }
}