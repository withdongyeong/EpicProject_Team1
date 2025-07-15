using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;


public class GridCell
{
    public Cell cell { get; private set; }
    
    public List<StarBase> starList { get; private set; } //AKA 인접효과
    public Vector3Int GridPosition { get; private set; }
    public Vector3 WorldPosition { get; private set; }
    public bool IsOccupied { get; set; }
    public SpriteRenderer sr { get; set; }

    /// <summary>
    /// 이 셀에 할당된 인접 효과가 변동되었을때 발동할 이벤트입니다. 이 셀에 놓인 타일 오브젝트가 구독합니다.
    /// </summary>
    private event Action _onStarListChange;

    /// <summary>
    /// 이벤트를 구독할 때 쓰는 메서드입니다.
    /// </summary>
    public event Action OnStarListChange
    {
        add => _onStarListChange += value;
        remove => _onStarListChange -= value;
    }

    /// <summary>
    /// 이벤트를 발동시킬 때 쓰는 메서드입니다.
    /// </summary>
    public void OnStarListChanged()
    {
        _onStarListChange?.Invoke();
    }

    public GridCell(Vector3Int gridPos, Vector3 worldPos)
    {
        GridPosition = gridPos;
        WorldPosition = worldPos;
        IsOccupied = false;
        cell = null; // 초기에는 셀 데이터가 없습니다.
        starList = new List<StarBase>(); // 스타 스킬 리스트 초기화
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
        //Debug.Log("좌표 " + GridPosition + "셀 데이터가 할당되었습니다: " + cell);
    }

    public void ReleaseCellData()
    {
        if(cell == null)
        {
            Debug.Log("이미 데이터가 없는 셀의 데이터를 없애려고 했어요");
            return;
        }

        cell = null;
        //Debug.Log(cell);
        ChangeSpriteTest();
    }

    public void AddStarSkill(StarBase starSkill)
    {
        starList.Add(starSkill);
        //인접 효과 리스트에 추가가 되었으므로 변경되었다는 액션을 호출합니다
        OnStarListChanged();
    }
    
    public void RemoveStarSkill(StarBase starSkill)
    {
        if (starList.Contains(starSkill))
        {
            //Debug.Log("스타 스킬 제거합니다");
            starList.Remove(starSkill);
            //인접 효과 리스트에 추가가 되었으므로 변경되었다는 액션을 호출합니다
            OnStarListChanged();
        }
        else
        {
            Debug.LogWarning("스타 스킬이 리스트에 없습니다: " + starSkill);
        }
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
            //sr.sprite = GridManager.Instance.GetOccupiedSprite();
        }
        else 
        {
            sr.color = Color.white;
            // 점유 해제되었을 때 기본 스프라이트로 복원
            sr.sprite = GridManager.Instance.GetDefaultSprite();
        }
    }
}

public class GridManager : Singleton<GridManager>
{
    private GameObject cellPrefab;
    private int maxSize = 9;
    private Vector3Int gridSize;
    public Vector3Int GridSize => gridSize;
    private GameObject startPoint;
    private GridCell[,] grid;

    private GameObject gridBlocks;
    
    private GridSpriteController _gridSpriteController;
    public GridSpriteController GridSpriteController => _gridSpriteController;
    
    // 스프라이트 관리
    [SerializeField] private Sprite redSprite;
    private Sprite defaultSprite; // 기본 스프라이트

    // 이동불가 그리드
    [SerializeField] private List<Vector3Int> unmovableGridPositions = new List<Vector3Int>();

    public List<Vector3Int> UnmovableGridPositions => unmovableGridPositions;

    /// <summary>
    /// 현재 배치되어있는 타일들의 이름 리스트입니다.
    /// </summary>
    private List<string> _placedTileList = new();

    public List<string> PlacedTileList => _placedTileList;
    
    private TilesOnGrid _tilesOnGrid;
    public TilesOnGrid TilesOnGrid => _tilesOnGrid;


    
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
        _gridSpriteController = GetComponentInChildren<GridSpriteController>();
        _tilesOnGrid = GetComponentInChildren<TilesOnGrid>();
        EventBus.SubscribeSceneLoaded(GridPosChange);
        EventBus.SubscribeTilePlaced(AddPlacedTileList);
        EventBus.SubscribeTileSell(RemovePlacedTileList);
        EventBus.SubscribeGameStart(IsGridFull);
        cellPrefab = Resources.Load<GameObject>("Prefabs/Tiles/TIleBase/board");
        Sprite[] cells = Resources.LoadAll<Sprite>("NewBoard/cellLine");
        //occupiedSprite = cells.FirstOrDefault(s => s.name == "cellLineOccupied"); // 점유 스프라이트 로드
        defaultSprite = cells.FirstOrDefault(s => s.name == "cellLine"); // 기본 스프라이트 로드
        if(redSprite == null){
            Debug.LogError("점유 스프라이트를 로드하지 못했습니다. 경로를 확인하세요.");
        }
        if(defaultSprite == null){
            Debug.LogError("기본 스프라이트를 로드하지 못했습니다. 경로를 확인하세요.");
        }
        InitializeGrid();
        InitGround();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            ResetGridCompletely();
        }
    }

    private void GridPosChange(Scene scene, LoadSceneMode mode)
    {
        if(SceneLoader.IsInBuilding())
            transform.position = new Vector3(0, 2f, 0);
        else if(SceneLoader.IsInStage())
            transform.position = new Vector3(0, 0, 0);
    }
    
    private void InitializeGrid()
    {
        startPoint = transform.GetChild(0).gameObject;
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
        gridBlocks = new GameObject("GridBlocks");
        gridBlocks.transform.SetParent(this.transform);
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 worldPos = grid[x, y].WorldPosition;
                GameObject go = Instantiate(cellPrefab, worldPos, Quaternion.identity, gridBlocks.transform);
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
            grid[gridPos.x, gridPos.y].IsOccupied = true;
            grid[gridPos.x, gridPos.y].SetCellData(cellData);

            //곽민준이 친 코드입니다. 해당 그리드에 할당된 인접 효과가 변경되면 타일이 다시 계산하게 합니다.
            grid[gridPos.x, gridPos.y].OnStarListChange += cellData.GetCombineCell().GetTileObject().UpdateStarList;
            //Debug.Log(grid[gridPos.x, gridPos.y].cell);
            //Debug.Log(GetCellData(gridPos));
        }
    }

    public void ReleaseCell(Vector3Int gridPos)
    {
        if (!IsCellAvailable(gridPos))
        {
            grid[gridPos.x, gridPos.y].IsOccupied = false;
            //곽민준이 친 코드입니다. 액션을 구독 취소하는 스크립트입니다.
            grid[gridPos.x, gridPos.y].OnStarListChange -= grid[gridPos.x, gridPos.y].cell.GetCombineCell().GetTileObject().UpdateStarList;
            grid[gridPos.x, gridPos.y].ReleaseCellData();       
            //Debug.Log("해제했습니다");
            grid[gridPos.x, gridPos.y].ChangeSpriteTest(); // 기본 스프라이트로 복원
        }
    }

    public bool IsCellAvailable(Vector3Int gridPos)
    {
        if (IsWithinGrid(gridPos))
        {
            //if(grid[gridPos.x, gridPos.y].IsOccupied) Debug.Log("해당 위치는 이미 점유되어 있습니다: " + gridPos);
            return !grid[gridPos.x, gridPos.y].IsOccupied;
        }
        //Debug.Log("범위 밖 " + gridPos);
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

    public void AddStarSkill(Vector3Int gridPos, StarBase starSkill)
    {
        if (IsWithinGrid(gridPos))
        {
            if (grid[gridPos.x, gridPos.y].starList == null)
            {
                Debug.LogError("존재해서는 안되는 에러로그: " + gridPos);
                return;
            }
            grid[gridPos.x, gridPos.y].AddStarSkill(starSkill);
        }
        else
        {
            Debug.Log("범위 바깥입니다: " + gridPos);
        }
    }
    
    public void RemoveStarSkill(Vector3Int gridPos, StarBase starSkill)
    {
        if (IsWithinGrid(gridPos))
        {
            if (grid[gridPos.x, gridPos.y].starList == null)
            {
                Debug.LogError("존재해서는 안되는 에러로그: " + gridPos);
                return;
            }
            grid[gridPos.x, gridPos.y].RemoveStarSkill(starSkill);
        }
        else
        {
            Debug.Log("범위 바깥입니다: " + gridPos);
        }
    }
    
    public List<StarBase> GetStarSkills(Vector3Int gridPos)
    {
        if (IsWithinGrid(gridPos))
        {
            return grid[gridPos.x, gridPos.y].starList;
        }
        else
        {
            Debug.Log("범위 바깥입니다: " + gridPos);
            return null;
        }
    }

    public void SetCellSprite(Vector3Int gridPos, Sprite sprite)
    {
        if (!IsWithinGrid(gridPos)) return;
        
        if (grid[gridPos.x, gridPos.y].IsOccupied)
        {
            return;
        }
        grid[gridPos.x, gridPos.y].sr.sprite = sprite;
    }
    
    public void SetCellSpritePreview(Vector3Int gridPos)
    {
        if (!IsWithinGrid(gridPos)) return;
        
        if (grid[gridPos.x, gridPos.y].IsOccupied)
        {
            grid[gridPos.x, gridPos.y].sr.color = Color.red;
        }
        else
        {
            grid[gridPos.x, gridPos.y].sr.color = Color.white;
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

    /// <summary>
    /// 이동 불가 위치를 추가합니다. 이미 추가된 위치는 무시합니다.
    /// </summary>
    /// <param name="position"></param>
    public void AddUnmovableGridPosition(Vector3Int position)
    {
        if (!unmovableGridPositions.Contains(position))
        {
            unmovableGridPositions.Add(position);
        }
        else
        {
            Debug.LogWarning("이미 추가된 이동 불가 위치입니다: " + position);
        }
    }

    /// <summary>
    /// 이동 불가 위치를 제거합니다. 존재하지 않는 위치는 무시합니다.
    /// </summary>
    /// <param name="position"></param>
    public void RemoveUnmovableGridPosition(Vector3Int position)
    {
        if (unmovableGridPositions.Contains(position))
        {
            unmovableGridPositions.Remove(position);
        }
        else
        {
            Debug.LogWarning("이동 불가 위치에 존재하지 않는 위치입니다: " + position);
        }
    }

    //이 밑은 타일이 배치되었을때/판매되었을때 
    private void AddPlacedTileList(TileObject tileObject)
    {
        _placedTileList.Add(tileObject.GetTileData().TileName);
    }

    public void RemovePlacedTileList(TileObject tileObject)
    {
        _placedTileList.Remove(tileObject.GetTileData().TileName);
    }    

 
    
    /// <summary>
    /// 모든 이동 불가 위치를 한번에 제거합니다. (플레이어 사망 시 등)
    /// </summary>
    public void ClearAllUnmovableGridPositions()
    {
        if (unmovableGridPositions.Count > 0)
        {
            Debug.Log($"모든 이동 불가 위치 해제: {unmovableGridPositions.Count}개");
            unmovableGridPositions.Clear();
        }
        else
        {
            Debug.Log("해제할 이동 불가 위치가 없습니다.");
        }
    }


    /// <summary>
    /// 그리드를 완전히 초기화합니다. 모든 타일 오브젝트를 제거하고, 그리드와 관련된 정보를 초기화합니다.
    /// </summary>
    public void ResetGridCompletely()
    {
        foreach (TileObject to in _tilesOnGrid.GetComponentsInChildren<TileObject>())
        {
            Destroy(to.gameObject);
        }
        Destroy(gridBlocks.gameObject);
        grid = null;
        
        InitializeGrid();
        InitGround();
        // 4. 부가 정보 초기화
        _placedTileList.Clear();
        ClearAllUnmovableGridPositions();
        
        
    }


    //AnalyticsManager에서 타일 배치 수를 가져오는 메서드입니다.
    public Dictionary<string, int> GetPlacedTileCount()
    {
        Dictionary<string, int> tileCount = new Dictionary<string, int>();
        foreach (string tileName in _placedTileList)
        {
            if (tileCount.ContainsKey(tileName))
            {
                tileCount[tileName]++;
            }
            else
            {
                tileCount[tileName] = 1;
            }
        }
        return tileCount;
    }
    //전부 배치 도전과제용 함수입니다
    private void IsGridFull()
    {
        bool result = true;
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int gridPos = new Vector3Int(x, y, 0);
                if (IsCellAvailable(gridPos))
                {
                    result = false;

                }
            }
        }

        if(result)
        {
            SteamAchievement.Achieve("ACH_BLD_FULL");
        }
    }
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(GridPosChange);
        EventBus.UnSubscribeTilePlaced(AddPlacedTileList);
        EventBus.UnSubscribeTileSell(RemovePlacedTileList);
        EventBus.UnsubscribeGameStart(IsGridFull);
    }
}