using System.Collections;
using UnityEngine;

/// <summary>
/// 플레이어 이동 및 타일 상호작용 관리
/// </summary>
public class PlayerController : MonoBehaviour
{
    private float _moveSpeed = 5f;
    private GridSystem _gridSystem;
    private int _currentX, _currentY;
    private bool _isMoving;
    private float _moveTime = 0.2f;

    // 바인딩 상태 추가
    private bool _isBind;
    
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
    public bool IsBind { get => _isBind; set => _isBind = value; }

    private void Start()
    {
        _gridSystem = FindAnyObjectByType<GridSystem>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateCurrentPosition();
    }
    
    private void Update()
    {
        if (!_isMoving && !_isBind)
        {
            HandleMovement();
        }
        CheckTileInteraction();
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
        
        if (_gridSystem.IsValidPosition(newX, newY))
        {
            StartCoroutine(MoveAnimation(newX, newY));
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 점프 효과가 있는 이동 애니메이션
    /// </summary>
    private IEnumerator MoveAnimation(int targetX, int targetY)
    {
        _isMoving = true;
        _animator.SetBool("IsMoving", true);
    
        Vector3 startPos = transform.position;
        Vector3 targetPos = _gridSystem.GetWorldPosition(targetX, targetY);
        float jumpHeight = 0.3f;
    
        float elapsedTime = 0;
    
        while (elapsedTime < _moveTime)
        {
            float t = elapsedTime / _moveTime;
        
            // XY 평면에서 이동 (Z는 항상 0)
            float x = Mathf.Lerp(startPos.x, targetPos.x, t);
            float y = Mathf.Lerp(startPos.y, targetPos.y, t);
        
            // 점프 높이 계산
            float extraHeight = Mathf.Sin(t * Mathf.PI) * jumpHeight;
        
            transform.position = new Vector3(x, y + extraHeight, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        transform.position = targetPos;
        _currentX = targetX;
        _currentY = targetY;
        _isMoving = false;
        _animator.SetBool("IsMoving", false);
    }
    
    /// <summary>
    /// 현재 그리드 위치 업데이트
    /// </summary>
    private void UpdateCurrentPosition()
    {
        _gridSystem.GetXY(transform.position, out _currentX, out _currentY);
    }
    
    /// <summary>
    /// 현재 위치 타일과 상호작용 확인
    /// </summary>
    private void CheckTileInteraction()
    {
        BaseTile currentTile = _gridSystem.GetTileAt(_currentX, _currentY);
    
        if (currentTile != null && currentTile.GetState() == BaseTile.TileState.Ready)
        {
            currentTile.Activate();
        }
    }
}