using UnityEngine;

/// <summary>
/// ������ ���� �ý����� �����ϴ� Ŭ����
/// </summary>
public class GridSystem : MonoBehaviour
{
    private int _width = 8;
    private int _height = 8;
    private float _cellSize = 1f;

    private BaseTile[,] _grid;
    private bool[,] _blockedCells; // �̵� �Ұ� �� ����

    // Getters & Setters
    public int Width { get => _width; set => _width = value; }
    public int Height { get => _height; set => _height = value; }
    public float CellSize { get => _cellSize; set => _cellSize = value; }
    
    /// <summary>
    /// �ʱ�ȭ
    /// </summary>
    void Awake()
    {
        _grid = new BaseTile[_width, _height];
        _blockedCells = new bool[_width, _height];
    }
    
    /// <summary>
    /// ���� ��ǥ�� ���� ��ġ�� ��ȯ
    /// </summary>
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y, 0) * _cellSize;
    }
    
    /// <summary>
    /// ���� ��ġ�� ���� ��ǥ�� ��ȯ
    /// </summary>
    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / _cellSize);
        y = Mathf.FloorToInt(worldPosition.y / _cellSize);
    }
    
    /// <summary>
    /// Ư�� ��ġ�� Ÿ�� ���
    /// </summary>
    public void RegisterTile(BaseTile tile, int x, int y)
    {
        if (IsValidPosition(x, y))
        {
            _grid[x, y] = tile;
        }
    }
    
    /// <summary>
    /// Ư�� ��ġ�� Ÿ�� ��ȯ
    /// </summary>
    public BaseTile GetTileAt(int x, int y)
    {
        if (IsValidPosition(x, y))
        {
            return _grid[x, y];
        }
        return null;
    }
    
    /// <summary>
    /// ��ǥ�� ���� ���� ���̰� �̵� �������� Ȯ��
    /// </summary>
    public bool IsValidPosition(int x, int y)
    {
        // �׸��� ���� üũ
        bool isInBounds = x >= 0 && y >= 0 && x < _width && y < _height;
        
        // ���� ���� �ְ� ���ܵ��� ���� ������ Ȯ��
        return isInBounds && !IsBlocked(x, y);
    }
    
    /// <summary>
    /// Ư�� ��ġ�� ���ܵǾ����� Ȯ��
    /// </summary>
    public bool IsBlocked(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
        {
            return _blockedCells[x, y];
        }
        // �׸��� �ٱ��� ���ܵ� ������ ����
        return true;
    }
    
    /// <summary>
    /// Ư�� ���� �̵� ���� ���� ����
    /// </summary>
    public void SetCellBlocked(int x, int y, bool blocked)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
        {
            _blockedCells[x, y] = blocked;
            Debug.Log($"�� ���� ���� ����: ({x}, {y}) -> {blocked}");
        }
    }
}