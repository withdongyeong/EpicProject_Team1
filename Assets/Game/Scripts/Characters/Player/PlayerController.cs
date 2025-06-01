using System.Collections;
using UnityEngine;

/// <summary>
/// �÷��̾� �̵� �� Ÿ�� ��ȣ�ۿ� ����
/// </summary>
public class PlayerController : MonoBehaviour
{
    private float _moveSpeed = 5f;
    private GridSystem _gridSystem;
    private int _currentX, _currentY;
    private bool _isMoving;
    private float _moveTime = 0.2f;

    //���ε� ���� �߰�
    private bool _isBind;
    
    // Getters & Setters
    public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
    public bool IsMoving { get => _isMoving; }
    public int CurrentX { get => _currentX; set => _currentX = value; }
    public int CurrentY { get => _currentY; set => _currentY = value; }

    public bool IsBind { get => _isBind; set => _isBind = value; }

    private void Start()
    {
        _gridSystem = FindAnyObjectByType<GridSystem>();
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
            TryMove(dx, dy);
        }
    }
    
    /// <summary>
    /// �̵� �õ� - ��ȿ�� ��ġ���� Ȯ�� �� �ִϸ��̼�
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
    /// ���� ȿ���� �ִ� �̵� �ִϸ��̼�
    /// </summary>
    private IEnumerator MoveAnimation(int targetX, int targetY)
    {
        _isMoving = true;
    
        Vector3 startPos = transform.position;
        Vector3 targetPos = _gridSystem.GetWorldPosition(targetX, targetY);
        float jumpHeight = 0.3f;
    
        float elapsedTime = 0;
    
        while (elapsedTime < _moveTime)
        {
            float t = elapsedTime / _moveTime;
        
            // XY ��鿡�� �̵� (Z�� �׻� 0)
            float x = Mathf.Lerp(startPos.x, targetPos.x, t);
            float y = Mathf.Lerp(startPos.y, targetPos.y, t);
        
            // �߰� ���� ����
            float extraHeight = Mathf.Sin(t * Mathf.PI) * jumpHeight;
        
            transform.position = new Vector3(x, y + extraHeight, 0);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        transform.position = targetPos;
        _currentX = targetX;
        _currentY = targetY;
        _isMoving = false;
    }
    
    /// <summary>
    /// ���� ���� ��ġ ������Ʈ
    /// </summary>
    private void UpdateCurrentPosition()
    {
        _gridSystem.GetXY(transform.position, out _currentX, out _currentY);
    }
    
    /// <summary>
    /// ���� ��ġ Ÿ�ϰ� ��ȣ�ۿ� Ȯ��
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