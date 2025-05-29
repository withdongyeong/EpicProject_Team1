using UnityEngine;



public class GridCell
{
    public InventoryItemData itemData { get; private set; } // 아이템 데이터 (인벤토리 아이템)
    public Vector3Int GridPosition { get; private set; } //칸의 논리적 좌표
    public Vector3 WorldPosition { get; private set; } //칸의 월드 좌표 (유니티 씬)
    public bool IsOccupied { get; set; } //칸이 점유되었는지 여부

    public GridCell(Vector3Int gridPos, Vector3 worldPos)
    {
        GridPosition = gridPos;
        WorldPosition = worldPos;
        IsOccupied = false; // 기본값은 false
    }
}

public class GridManager : Singleton<GridManager>
{
    public GameObject cellPrefab; // 그리드 셀에 사용할 프리팹
    [SerializeField]
    private int maxSize = 5;
    
    private Vector3Int gridSize; // 그리드의 크기
    public Vector3Int GridSize => gridSize; // 외부에서 접근할 수 있도록 프로퍼티로 제공
    
    private GridCell[,] grid;
    
    protected override void Awake()
    {
        base.Awake();
        InitializeGrid();
        InitGround();
    }

    private void InitializeGrid()
    {
        gridSize = new Vector3Int(maxSize, maxSize, 0); // 2D 그리드로 설정
        grid = new GridCell[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int gridPos = new Vector3Int(x, y, 0); // 논리적 좌표
                Vector3 worldPos = GridToWorldPosition(gridPos);
                grid[x, y] = new GridCell(gridPos, worldPos);
            }
        }
    }
    
    private void InitGround()
    {
        GameObject gridCells = new GameObject("GridBlocks");
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = grid[x, y].WorldPosition;
                Instantiate(cellPrefab, worldPos, Quaternion.identity, gridCells.transform);
            }
        }
    }
    
    private Vector3 GridToWorldPosition(Vector3Int gridPos)
    {
        // 그리드 좌표를 월드 좌표로 변환하는 로직
        // 현재는 1:1
        return new Vector3(gridPos.x, gridPos.y, 0); // z는 0으로 설정
    }

    public bool IsCellAvailable(Vector3Int gridPos)
    {
        if (IsWithinGrid(gridPos))
        {
            return !grid[gridPos.x, gridPos.y].IsOccupied;
        }
        return false; // 그리드 범위를 벗어난 경우 false 반환
    }
    
    private bool IsWithinGrid(Vector3Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < gridSize.x &&
               gridPos.y >= 0 && gridPos.y < gridSize.y;
    }
    
}
