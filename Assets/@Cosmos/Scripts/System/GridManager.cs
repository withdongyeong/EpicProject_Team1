using UnityEngine;

public class GridCell
{
    public Cell cell { get; private set; }
    public Vector3Int GridPosition { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public bool IsOccupied { get; set; }
    public SpriteRenderer sr { get; set; }

    public GridCell(Vector3Int gridPos, Vector3 worldPos)
    {
        GridPosition = gridPos;
        WorldPosition = worldPos;
        IsOccupied = false;
    }

    /// <summary>
    /// 오브젝트를 배치할 때 해당 좌표에 Cell을 할당시킵니다.
    /// </summary>
    public void SetCellData(Cell cellData)
    {
        if (cellData == null)
        {
            Debug.Log("음 null입니다. 셀 데이터가 할당되지 않았습니다.");
            return;
        }
        
        cell = cellData;
        Debug.Log("좌표 " + GridPosition + "셀 데이터가 할당되었습니다: " + cell);
        ChangeSpriteTest();
    }

    public void ReleaseCellData()
    {
        if(cell == null)
        {
            Debug.Log("이미 데이터가 없는 셀의 데이터를 없애려고 했어요");
            return;
        }

        cell = null;
        Debug.Log(cell);
        ChangeSpriteTest();
    }
    
    /// <summary>
    /// 스프라이트를 점유 상태에 따라 변경합니다.
    /// </summary>
    public void ChangeSpriteTest()
    {
        if(IsOccupied) 
        {
            sr.color = Color.white;
            // 점유되었을 때 점유 스프라이트로 변경
            sr.sprite = GridManager.Instance.GetOccupiedSprite();
        }
        else 
        {
            sr.color = Color.white;
            // 점유 해제되었을 때 기본 스프라이트로 복원
            sr.sprite = GridManager.Instance.GetDefaultSprite();
        }
    }

    /// <summary>
    /// 미리 보기용 스프라이트로 변경합니다
    /// </summary>

    public void ChangeSpritePreview(bool isPreview)
    {
        if (isPreview)
        {
            if (IsOccupied)
            {
                sr.color = Color.red;
                return;
            }
            sr.color = Color.white;
            sr.sprite = GridManager.Instance.GetOccupiedSprite();
        }
        else
        {
            sr.color = Color.white; // 미리 보기 해제 시 색상 복원
            sr.sprite = GridManager.Instance.GetDefaultSprite();
        }
    }
}

public class GridManager : Singleton<GridManager>
{
    public GameObject cellPrefab;
    [SerializeField] private int maxSize = 5;
    private Vector3Int gridSize;
    public Vector3Int GridSize => gridSize;
    [SerializeField] private GameObject startPoint;
    private GridCell[,] grid;
    
    // 스프라이트 관리
    [SerializeField] private Sprite occupiedSprite; // 점유용 스프라이트
    [SerializeField] private Sprite defaultSprite; // 기본 스프라이트

    /// <summary>
    /// 점유 스프라이트를 반환합니다.
    /// </summary>
    public Sprite GetOccupiedSprite()
    {
        return occupiedSprite;
    }
    
    /// <summary>
    /// 기본 스프라이트를 반환합니다.
    /// </summary>
    public Sprite GetDefaultSprite()
    {
        return defaultSprite;
    }

    protected override void Awake()
    {
        base.Awake();
        InitializeGrid();
        InitGround();
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void InitializeGrid()
    {
        gridSize = new Vector3Int(maxSize, maxSize, 0);
        grid = new GridCell[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int gridPos = new Vector3Int(x, y, 0);
                Vector3 worldPos = GridToWorldPosition(gridPos);
                grid[x, y] = new GridCell(gridPos, worldPos);
            }
        }
    }

    private void InitGround()
    {
        GameObject gridCells = new GameObject("GridBlocks");
        gridCells.transform.SetParent(this.transform);
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = grid[x, y].WorldPosition;
                GameObject go = Instantiate(cellPrefab, worldPos, Quaternion.identity, gridCells.transform);
                grid[x, y].sr = go.GetComponent<SpriteRenderer>();
                
                // 기본 스프라이트 설정
                if (defaultSprite != null)
                {
                    grid[x, y].sr.sprite = defaultSprite;
                }
            }
        }
    }

    public Vector3 GridToWorldPosition(Vector3Int gridPos)
    {
        return new Vector3(startPoint.transform.position.x + gridPos.x, startPoint.transform.position.y + gridPos.y, 0);
    }

    public Vector3Int WorldToGridPosition(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x - startPoint.transform.position.x);
        int y = Mathf.RoundToInt(worldPos.y - startPoint.transform.position.y);
        return new Vector3Int(x, y, 0);
    }

    /// <summary>
    /// 그리드 셀을 점유하는 메서드
    /// </summary>
    public void OccupyCell(Vector3Int gridPos, Cell cellData = null)
    {
        if (IsCellAvailable(gridPos))
        {
            SoundManager.Instance.UISoundClip("DeploymentActivate");
            grid[gridPos.x, gridPos.y].IsOccupied = true;
            grid[gridPos.x, gridPos.y].SetCellData(cellData);
            Debug.Log(grid[gridPos.x, gridPos.y].cell);
            Debug.Log(GetCellData(gridPos));
        }
    }

    public void ReleaseCell(Vector3Int gridPos)
    {
        if (!IsCellAvailable(gridPos))
        {
            grid[gridPos.x, gridPos.y].IsOccupied = false;
            grid[gridPos.x, gridPos.y].ReleaseCellData();
            Debug.Log("해제했습니다");
            grid[gridPos.x, gridPos.y].ChangeSpriteTest(); // 기본 스프라이트로 복원
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
        return false;
    }

    public bool IsWithinGrid(Vector3Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < gridSize.x &&
               gridPos.y >= 0 && gridPos.y < gridSize.y;
    }

    public Cell GetCellData(Vector3Int gridPos)
    {
        if (grid[gridPos.x,gridPos.y].cell == null)
        {
            return null;
        }
        return grid[gridPos.x,gridPos.y].cell;
    }

    public bool CanPlaceBlock(Transform draggedTransform)
    {
        foreach (Cell cell in draggedTransform.GetComponentsInChildren<Cell>())
        {
            Transform child = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(child.position);
            if (!GridManager.Instance.IsCellAvailable(gridPos))
            {
                return false;
            }
        }
        return true;
    }
    

    public void ChangeCellSprite(Vector3Int gridPos, bool isPreview)
    {
        if (IsWithinGrid(gridPos))
        {
            grid[gridPos.x, gridPos.y].ChangeSpritePreview(isPreview);
        }
        else
        {
            //Debug.Log("범위 바깥ㅇ인데요 " + gridPos);
        }
    }

    public void ChangeCellSpriteAll()
    {
        for( int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int gridPos = new Vector3Int(x, y, 0);
                grid[x, y].ChangeSpriteTest(); // 미리 보기 해제
            }
        }
    }
    // 테스트 메서드들
    public void TestPrintInventoryItemDataGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int gridPos = new Vector3Int(x, y, 0);
                if (GetCellData(gridPos) == null)
                {
                    Debug.Log("셀 데이터가 할당되지 않았습니다." + gridPos);
                }
                else
                {
                    Debug.Log(grid[x,y].cell.name);
                }
            }
            Debug.Log("----------");
        }
    }

    public void TestResetGrid()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                grid[x, y].IsOccupied = false;
                grid[x, y].ChangeSpriteTest(); // 기본 스프라이트로 복원
            }
        }
    }
}