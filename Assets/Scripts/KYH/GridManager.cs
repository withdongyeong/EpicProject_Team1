using UnityEngine;



public class GridCell
{
    public InventoryItemData itemData { get; private set; } // 아이템 데이터 (인벤토리 아이템)
    public Vector3Int GridPosition { get; private set; } //칸의 논리적 좌표
    public Vector3 WorldPosition { get; private set; } //칸의 월드 좌표 (유니티 씬)
    public bool IsOccupied { get; set; } //칸이 점유되었는지 여부

    public SpriteRenderer sr { get;  set; } // 셀의 스프라이트 렌더러 테스트용 ...

    public GridCell(Vector3Int gridPos, Vector3 worldPos)
    {
        GridPosition = gridPos;
        WorldPosition = worldPos;
        IsOccupied = false; // 기본값은 false
    }

    public void SetItemData(InventoryItemData itemData)
    {
        this.itemData = itemData;
        // 아이템 데이터가 설정되면 추가 작업이 필요할 수 있습니다.
    }
    public void ChangeColorTest()
    {
        if(IsOccupied) sr.color = Color.yellow; // occupied가 true일 때 노란색으로 변경
        else sr.color = Color.white; // occupied가 false일 때 흰색으로 변경
        // 이 메서드는 나중에 셀의 색상을 변경하는 로직으로 확장할 수 있습니다.
        // 예를 들어, occupied가 true일 때 셀의 색상을 변경하는 등의 작업을 할 수 있습니다.
        
    }
}

public class GridManager : Singleton<GridManager>
{
    public GameObject cellPrefab; // 그리드 셀에 사용할 프리팹
    [SerializeField] private int maxSize = 5;

    private Vector3Int gridSize; // 그리드의 크기
    public Vector3Int GridSize => gridSize; // 외부에서 접근할 수 있도록 프로퍼티로 제공

    private GridCell[,] grid;
    private InventoryItemData[,] itemDataGrid; // 아이템 데이터를 저장할 2D 배열

    protected override void Awake()
    {
        base.Awake();
        InitializeGrid();
        InitGround();
        InitItemDataGrid();        
    }

    
    private void InitItemDataGrid()
    {
        itemDataGrid = new InventoryItemData[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                itemDataGrid[x, y] = null; // 초기화
            }
        }
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
                GameObject go = Instantiate(cellPrefab, worldPos, Quaternion.identity, gridCells.transform);
                grid[x, y].sr = go.GetComponent<SpriteRenderer>(); // 셀의 스프라이트 렌더러를 저장
            }
        }
    }

    private Vector3 GridToWorldPosition(Vector3Int gridPos)
    {
        // 그리드 좌표를 월드 좌표로 변환하는 로직
        // 현재는 1:1
        return new Vector3(gridPos.x, gridPos.y, 0); // z는 0으로 설정
    }

    public Vector3Int WorldToGridPosition(Vector3 worldPos)
    {
        return Vector3Int.RoundToInt(worldPos);
    }

    
    
    /// 그리드 셀을 점유하는 메서드

    public void OccupyCell(Vector3Int gridPos, InventoryItemData itemData = null)
    {
        if (IsWithinGrid(gridPos))
        {
            grid[gridPos.x, gridPos.y].IsOccupied = true;
            grid[gridPos.x, gridPos.y].ChangeColorTest();
            if (itemData != null)
            {
                grid[gridPos.x, gridPos.y].SetItemData(itemData);
                itemDataGrid[gridPos.x, gridPos.y] = itemData; // 아이템 데이터 저장
            }
        }
    }

    public bool IsCellAvailable(Vector3Int gridPos)
    {
        if (IsWithinGrid(gridPos))
        {
            if(grid[gridPos.x, gridPos.y].IsOccupied) Debug.Log("해당 위치는 이미 점유되어 있습니다: " + gridPos);
            return !grid[gridPos.x, gridPos.y].IsOccupied;
        }
        Debug.Log("범위 밖 " + gridPos);
        return false; // 그리드 범위를 벗어난 경우 false 반환
    }
    
    private bool IsWithinGrid(Vector3Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < gridSize.x &&
               gridPos.y >= 0 && gridPos.y < gridSize.y;
    }
    
    
    public InventoryItemData[,] GetItemDataGrid()
    {
        return itemDataGrid; // 아이템 데이터 그리드를 반환
    }
    
    public void TestPrintInventoryItemDataGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (itemDataGrid[x, y] != null)
                {
                    Debug.Log($"Item at ({x}, {y}): {itemDataGrid[x, y].itemName}");
                }
                else
                {
                    Debug.Log($"No item at ({x}, {y})");
                }
            }
            Debug.Log("----------");
        }
    }
    
    public void TestResetGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y].IsOccupied = false;
                grid[x, y].ChangeColorTest();
                grid[x, y].SetItemData(null);
                itemDataGrid[x, y] = null; // 아이템 데이터 초기화
            }
        }
    }
    
}
