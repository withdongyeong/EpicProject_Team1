using UnityEngine;

/// <summary>
/// 그리드 기반 시스템을 관리하는 클래스
/// 격자 위치 오프셋 지원
/// </summary>
public class GridSystem : MonoBehaviour
{
    [Header("그리드 설정")]
    [SerializeField] private int _width = 8;
    [SerializeField] private int _height = 8;
    [SerializeField] private float _cellSize = 1f;
    
    [Header("격자 위치 조정")]
    [SerializeField] private Vector3 _gridOffset = Vector3.zero; // 격자 시작점 오프셋

    private BaseTile[,] _grid;
    private bool[,] _blockedCells; // 이동 불가 셀 정보

    // Getters & Setters
    public int Width { get => _width; set => _width = value; }
    public int Height { get => _height; set => _height = value; }
    public float CellSize { get => _cellSize; set => _cellSize = value; }
    public Vector3 GridOffset { get => _gridOffset; set => _gridOffset = value; }
    
    /// <summary>
    /// 초기화
    /// </summary>
    void Awake()
    {
        _grid = new BaseTile[_width, _height];
        _blockedCells = new bool[_width, _height];
    }
    
    /// <summary>
    /// 그리드 좌표를 월드 위치로 변환 (오프셋 적용)
    /// </summary>
    /// <param name="x">그리드 X 좌표</param>
    /// <param name="y">그리드 Y 좌표</param>
    /// <returns>월드 위치</returns>
    public Vector3 GetWorldPosition(int x, int y)
    {
        Vector3 gridPosition = new Vector3(x, y, 0) * _cellSize;
        return gridPosition + _gridOffset + transform.position;
    }
    
    /// <summary>
    /// 월드 위치를 그리드 좌표로 변환 (오프셋 고려)
    /// </summary>
    /// <param name="worldPosition">월드 위치</param>
    /// <param name="x">출력할 그리드 X 좌표</param>
    /// <param name="y">출력할 그리드 Y 좌표</param>
    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        Vector3 adjustedPosition = worldPosition - _gridOffset - transform.position;
        x = Mathf.FloorToInt(adjustedPosition.x / _cellSize);
        y = Mathf.FloorToInt(adjustedPosition.y / _cellSize);
    }
    
    /// <summary>
    /// 격자를 특정 월드 위치로 이동
    /// </summary>
    /// <param name="newOffset">새로운 격자 시작점</param>
    public void SetGridOffset(Vector3 newOffset)
    {
        _gridOffset = newOffset;
        Debug.Log($"[GridSystem] 격자 오프셋 변경: {_gridOffset}");
    }
    
    /// <summary>
    /// 격자를 중앙으로 이동 (카메라 기준)
    /// </summary>
    public void CenterGrid()
    {
        // 격자의 중심을 (0,0)으로 맞춤
        float centerX = -(_width - 1) * _cellSize * 0.5f;
        float centerY = -(_height - 1) * _cellSize * 0.5f;
        _gridOffset = new Vector3(centerX, centerY, 0);
        Debug.Log($"[GridSystem] 격자를 중앙으로 이동: {_gridOffset}");
    }
    
    /// <summary>
    /// 격자를 화면 왼쪽 하단으로 이동
    /// </summary>
    /// <param name="margin">가장자리 여백</param>
    public void AlignToBottomLeft(float margin = 1f)
    {
        _gridOffset = new Vector3(margin, margin, 0);
        Debug.Log($"[GridSystem] 격자를 왼쪽 하단으로 이동: {_gridOffset}");
    }
    
    /// <summary>
    /// 특정 위치에 타일 등록
    /// </summary>
    /// <param name="tile">등록할 타일</param>
    /// <param name="x">그리드 X 좌표</param>
    /// <param name="y">그리드 Y 좌표</param>
    public void RegisterTile(BaseTile tile, int x, int y)
    {
        if (IsValidPosition(x, y))
        {
            _grid[x, y] = tile;
        }
    }
    
    /// <summary>
    /// 특정 위치의 타일 반환
    /// </summary>
    /// <param name="x">그리드 X 좌표</param>
    /// <param name="y">그리드 Y 좌표</param>
    /// <returns>해당 위치의 타일</returns>
    public BaseTile GetTileAt(int x, int y)
    {
        if (IsValidPosition(x, y))
        {
            return _grid[x, y];
        }
        return null;
    }
    
    /// <summary>
    /// 좌표가 유효 범위 내이고 이동 가능한지 확인
    /// </summary>
    /// <param name="x">그리드 X 좌표</param>
    /// <param name="y">그리드 Y 좌표</param>
    /// <returns>유효하고 이동 가능하면 true</returns>
    public bool IsValidPosition(int x, int y)
    {
        // 그리드 범위 체크
        bool isInBounds = x >= 0 && y >= 0 && x < _width && y < _height;
        
        // 범위 내에 있고 차단되지 않은 위치인지 확인
        return isInBounds && !IsBlocked(x, y);
    }
    
    /// <summary>
    /// 특정 위치가 차단되어있는지 확인
    /// </summary>
    /// <param name="x">그리드 X 좌표</param>
    /// <param name="y">그리드 Y 좌표</param>
    /// <returns>차단되어 있으면 true</returns>
    public bool IsBlocked(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
        {
            return _blockedCells[x, y];
        }
        // 그리드 바깥은 차단된 것으로 간주
        return true;
    }
    
    /// <summary>
    /// 특정 셀의 이동 가능 여부 설정
    /// </summary>
    /// <param name="x">그리드 X 좌표</param>
    /// <param name="y">그리드 Y 좌표</param>
    /// <param name="blocked">차단 여부</param>
    public void SetCellBlocked(int x, int y, bool blocked)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
        {
            _blockedCells[x, y] = blocked;
            Debug.Log($"[GridSystem] 셀 차단 상태 변경: ({x}, {y}) -> {blocked}");
        }
    }
    
    /// <summary>
    /// 격자 경계 표시 (디버그용)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 격자 경계 그리기
        Gizmos.color = Color.yellow;
        
        // 격자의 실제 경계 계산 (셀 중심점이 아닌 셀 경계)
        Vector3 bottomLeft = GetWorldPosition(0, 0) - Vector3.one * _cellSize * 0.5f;
        Vector3 topRight = GetWorldPosition(_width - 1, _height - 1) + Vector3.one * _cellSize * 0.5f;
        
        // 격자 테두리 (사각형)
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, bottomLeft.z);
        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, bottomLeft.z);
        
        // 외곽 테두리 그리기
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
        
        // 격자 라인 그리기
        Gizmos.color = Color.gray;
        
        // 세로 라인 (X축 방향)
        for (int x = 0; x <= _width; x++)
        {
            Vector3 start = bottomLeft + Vector3.right * (x * _cellSize);
            Vector3 end = topLeft + Vector3.right * (x * _cellSize);
            Gizmos.DrawLine(start, end);
        }
        
        // 가로 라인 (Y축 방향)
        for (int y = 0; y <= _height; y++)
        {
            Vector3 start = bottomLeft + Vector3.up * (y * _cellSize);
            Vector3 end = bottomRight + Vector3.up * (y * _cellSize);
            Gizmos.DrawLine(start, end);
        }
        
        // 셀 중심점 표시 (선택사항)
        Gizmos.color = Color.red;
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector3 cellCenter = GetWorldPosition(x, y);
                Gizmos.DrawWireCube(cellCenter, Vector3.one * 0.1f);
            }
        }
    }
}