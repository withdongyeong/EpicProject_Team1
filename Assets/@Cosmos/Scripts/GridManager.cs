 using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;



public class GridCell
{
    
    public Cell cell { get; private set; } // 셀의 Cell 컴포넌트
    public Vector3Int GridPosition { get; private set; } //칸의 논리적 좌표
    public Vector3 WorldPosition { get; private set; } //칸의 월드 좌표 (유니티 씬)
    public bool IsOccupied { get; set; } //칸이 점유되었는지 여부
    public SpriteRenderer sr { get;  set; } // 셀의 스프라이트 렌더러 테스트용 ... 오브젝트가 아닌 그리드 셸의 스프라이트 렌더러 입니다.

    
    /// <summary>
    /// 그리드 셀을 초기화하는 생성자 입니다.
    /// </summary>
    /// <param name="gridPos"></param>
    /// <param name="worldPos"></param>
    public GridCell(Vector3Int gridPos, Vector3 worldPos)
    {
        GridPosition = gridPos;
        WorldPosition = worldPos;
        IsOccupied = false; // 기본값은 false
    }

    
    /// <summary>
    /// 오브젝트를 배치할 때 해당 좌표에 Cell을 할당시킵니다.
    /// </summary>
    /// <param name="cellData"></param>
    public void SetCellData(Cell cellData)
    {
        if (cellData == null)
        {
            Debug.Log("음 null입니다. 셀 데이터가 할당되지 않았습니다.");
            return;
        }
        
        cell = cellData; // 셀의 Cell 컴포넌트를 설정
        Debug.Log("좌표 "+GridPosition +"셀 데이터가 할당되었습니다: " +cell);
        ChangeColorTest();
    }
    
  
    
    // 아마 나중에 할당된 그리드 인벤 구분용으로 쓰이지 않을까 싶네요
    public void ChangeColorTest()
    {
        if(IsOccupied) sr.color = Color.yellow; // occupied가 true일 때 노란색으로 변경
        else sr.color = new Color(176f / 255f, 152f / 255f, 152f / 255f); // occupied가 false일 때 흰색으로 변경
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

    [SerializeField]
    private GameObject startPoint; // 시작점 오브젝트
    private GridCell[,] grid;

    //private Transform draggedTransform; // 드래그 되고 있는 오브젝트의 트랜스폼입니다.
    [SerializeField] GameObject draggablePlane; // 드래그를 담당할 오브젝트입니다.
    

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
        gridCells.transform.SetParent(this.transform); // GridManager의 자식으로 설정
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = grid[x, y].WorldPosition;
                GameObject go = Instantiate(cellPrefab, worldPos, Quaternion.identity, gridCells.transform);
                grid[x, y].sr = go.GetComponent<SpriteRenderer>(); // 셀의 스프라이트 렌더러를 저장
            }
        }
        //드래그를 담당하는 평면을 생성합니다
        SpawnDragPlane();
    }

    public Vector3 GridToWorldPosition(Vector3Int gridPos)
    {
        // 그리드 좌표를 월드 좌표로 변환하는 로직
        // 현재는 1:1
        return new Vector3(startPoint.transform.position.x +gridPos.x, startPoint.transform.position.y + gridPos.y, 0); // z는 0으로 설정
    }

    public Vector3Int WorldToGridPosition(Vector3 worldPos)
    {
        // 월드 좌표를 그리드 좌표로 변환하는 로직
        int x = Mathf.RoundToInt(worldPos.x - startPoint.transform.position.x);
        int y = Mathf.RoundToInt(worldPos.y - startPoint.transform.position.y);
        return new Vector3Int(x, y, 0); // z는 0으로 설정
    }

    
    
    /// 그리드 셀을 점유하는 메서드

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
            grid[gridPos.x, gridPos.y].ChangeColorTest();
        }
    }

    
    
    
    /// <summary>
    /// gridPos가 그리드 내에 있는지 확인하고, 해당 셀이 점유되지 않은 경우 true를 반환합니다.
    /// </summary>
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
    
    
    /// <summary>
    /// gridpos가 그리드 내에 있는지 확인하는 메서드입니다.
    /// </summary>
    public bool IsWithinGrid(Vector3Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < gridSize.x &&
               gridPos.y >= 0 && gridPos.y < gridSize.y;
    }
    
    
    /// <summary>
    /// cell을 가져옵니다
    /// </summary>
    public Cell GetCellData(Vector3Int gridPos)
    {
        if (grid[gridPos.x,gridPos.y].cell == null)
        {
            //Debug.Log("셀 데이터가 할당되지 않았습니다." + gridPos);
            return null;
        }
        return grid[gridPos.x,gridPos.y].cell; // 셀의 Cell 컴포넌트를 반환
    }

    #region 곽민준이 구현하는 배치된 오브젝트 드래그


    public bool CanPlaceBlock(Transform draggedTransform)
    {
        foreach (Cell cell in draggedTransform.GetComponentsInChildren<Cell>())
        {
            Transform child = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(child.position);
            if (!GridManager.Instance.IsCellAvailable(gridPos))
            {
                return false; // 하나라도 불가능한 셀이 있으면 false 반환
            }
        }
        return true; // 모든 셀이 가능하면 true 반환
    }

    private void SpawnDragPlane()
    {
        Vector3 planePosition = (GridToWorldPosition(Vector3Int.zero) + GridToWorldPosition(new Vector3Int(maxSize, maxSize, 0))) / 2 - new Vector3(0.5f,0.5f,0);
        var plane = Instantiate(draggablePlane, planePosition,Quaternion.identity,transform);
        plane.transform.localScale = new Vector3(maxSize, maxSize, 1);

    }

    #endregion


    //-------------------------------------------------------------------------------//
    // 이 밑으로는 테스트용 메서드들입니다.

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
                grid[x, y].ChangeColorTest();
            }
        }
    }

    
}
