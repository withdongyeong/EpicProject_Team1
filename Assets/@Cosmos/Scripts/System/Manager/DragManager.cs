using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


/// <summary>
/// DragManager는 드래그 이벤트를 관리하는 싱글턴 클래스입니다.
/// 게임에서 이루어지는 모든 드래그 동작을 관리,처리 합니다.
/// 드래그는 Tile을 제외하고 그 무엇도 드래그 되지 않습니다.
/// 현재 1.상점->grid   2.grid-> grid  
/// </summary>
public class DragManager : Singleton<DragManager>
{
    private GameObject currentDragObject;
    private Camera mainCamera;
    private SmoothRotator smoothRotator;
    
    private bool isDragging = false; // 드래그 중인지 여부
    public bool IsDragging => isDragging; // 외부에서 드래그 상태를 확인할 수 있도록 공개

    public Vector3 LocalPos { get; set; }

    



    protected override void Awake()
    {
        base.Awake();
        smoothRotator = gameObject.AddComponent<SmoothRotator>();
    }
    
    
    private void Update()
    {
        if (isDragging)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetMouseButtonDown(1))
            {
                RotateObject();
            }
            Vector3 mousePosition = Input.mousePosition;
            mainCamera = Camera.main;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0f; // 2D 게임이므로 z값을 0으로 설정
                                  // 드래그 오브젝트 위치 업데이트
            currentDragObject.transform.position = worldPosition + LocalPos;
        }
    }
    
    public void BeginDrag(GameObject draggableObject)
    {
        isDragging = true;
        currentDragObject = draggableObject;
        Vector3 mousePosition = Input.mousePosition;
        mainCamera = Camera.main;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f; // 2D 게임이므로 z값을 0으로 설정
        LocalPos = currentDragObject.transform.position - worldPosition; //현재 마우스 위치 기준으로 현재 드래그되는 타일의 로컬 포지션.
        draggableObject.GetComponent<TileObject>().OnDragged();
    }
    
    public void Drag()
    {
        if (currentDragObject == null || !isDragging)
            return;

        // 그리드 미리보기 업데이트
        UpdatePreviewCell();
    }
    
    public void EndDrag()
    {
        isDragging = false;
        currentDragObject = null;
        GridManager.Instance.TilesOnGrid.SetTileObjectStarEffect();
        UpdatePreviewCell();
    }


    public void SetObjectPosition(Vector3 position)
    {
        if (currentDragObject == null) return;
        currentDragObject.transform.position = position;
    }
    
    public void PlaceObject()
    {
        //놓으려는 물체가 회전중인지 미리 검사하고, 회전중이면 멈춥니다.
        TryStopRotate();
        Vector3 corePos = currentDragObject.GetComponentInChildren<CombineCell>().GetCoreCell().transform.position;
        corePos = GridManager.Instance.GridToWorldPosition(GridManager.Instance.WorldToGridPosition(corePos));
        currentDragObject.transform.position = corePos;
        currentDragObject.transform.SetParent(GridManager.Instance.TilesOnGrid.gameObject.transform);
        foreach (var cell in currentDragObject.GetComponentsInChildren<Cell>())
        {
            
            Transform t = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(t.position);
            if (cell.GetType() == typeof(StarCell)) //스타셀일때
            {
                StarCell starCell = cell as StarCell;
                StarBase starSkill = starCell.GetStarSkill();
                GridManager.Instance.AddStarSkill(gridPos, starSkill);
                continue;
            }
            
            // 스타셀이 아니라 그냥 셀일때
            GridManager.Instance.OccupyCell(gridPos, cell);
        }

        SoundManager.Instance.UISoundClip("DeploymentActivate");
        SetGridSprite();
        
        
        TileObject tileObject = currentDragObject.GetComponent<TileObject>();

        tileObject.OnPlaced();
        //배치된 타일이 인접효과를 계산하게 합니다
        tileObject.UpdateStarList();
        //타일이 배치되었음을 알립니다. 현재 사용하는애가 없습니다.
        EventBus.PublishTilePlaced(tileObject);
    }


    public GameObject GetCurrentDragObject()
    {
        if (currentDragObject == null)
        {
            Debug.LogWarning("현재 드래그 중인 오브젝트가 없습니다.");
            return null;
        }
        return currentDragObject;
    }
    public void DestroyObject()
    {
        Destroy(currentDragObject);
    }
    
    /// <summary>
    /// 드래그 중인 오브젝트가 그리드에 배치 가능한지 확인합니다.
    /// 배치 가능한 경우 true를 반환하고, 그렇지 않은 경우 false를 반환합니다.
    /// </summary>
    public bool CanPlaceTile()
    {
        if (currentDragObject == null) return false;
        foreach (Cell cell in currentDragObject.GetComponentsInChildren<Cell>())
        {
            if (cell.GetType() == typeof(StarCell)) // 스타셀일 때는 배치 가능 여부를 따지지 않음
            {
                continue;
            }
            Transform child = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(child.position);
            if (!GridManager.Instance.IsCellAvailable(gridPos))
            {
                return false; // 하나라도 불가능한 셀이 있으면 false 반환
            }
        }
        return true; // 모든 셀이 가능하면 true 반환
    }
    
    // 회전시 그리드 미리보기 업데이트, 추후 미리보기 관련 스크립트 따로 분리 예정
    private void UpdatePreviewCell()
    {
        GridManager.Instance.ChangeCellSpriteAll();
        if (currentDragObject == null) return;
        foreach (Cell cell in currentDragObject.GetComponentsInChildren<Cell>())
        {
            if (cell.GetType() == typeof(StarCell)) continue;
            Transform t = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(t.position);
        }
        
        SetGridSprite(true);

        // 같은 타일을 중복으로 표시하지 않도록 하기 위해 List를 사용합니다.
        List<TileObject> skills = new List<TileObject>();
        List<string> archmagestaff = new List<string>();

        // 스타셀의 색상을 초기화하고, 해당 스타셀의 스킬이 조건을 만족하면 색을 바꿉니다.
        foreach (StarCell starCell in currentDragObject.GetComponentsInChildren<StarCell>())
        {
            // 스타셀의 색상을 초기화합니다.
            SpriteRenderer sr = starCell.GetComponent<SpriteRenderer>();
            Sprite spriteDisable = Resources.Load<Sprite>("Arts/UI/StarDisable");
            sr.sprite = spriteDisable;

            // 스타셀의 위치를 가져오고, 해당 위치의 CellData가 존재하는지 확인합니다.
            Vector3Int gridPos = starCell.GetStarCellPosition();
            if (!GridManager.Instance.IsWithinGrid(gridPos) || GridManager.Instance.GetCellData(gridPos) == null)
            {
                continue;
            }

            // 스타셀의 스킬을 가져오고, 해당 스킬이 조건을 만족하는지 확인합니다.
            SkillBase[] skillBases = GridManager.Instance.GetCellData(gridPos)?.GetCombineCell()?.Skills;
            if (skillBases == null || skillBases.Length == 0)
            {
                skillBases = new SkillBase[1];
                skillBases[0] = GridManager.Instance.GetCellData(gridPos).GetCombineCell().GetTileObject().GetComponentInChildren<SkillBase>();
            }
            foreach (SkillBase skill in skillBases)
            {
                if (starCell.GetStarSkill().CheckCondition(skill) && !skills.Contains(skill.TileObject))
                {
                    if (starCell.GetStarSkill().GetType().Name.Contains("ArchmageStaffStarSkill"))
                    {
                        if (archmagestaff.Contains(skill.TileObject.name))
                        {
                            continue; // 이미 추가된 스킬이면 건너뜀
                        }
                        archmagestaff.Add(skill.TileObject.name); // ArchmageStaffStarSkill이 중복되지 않도록 관리
                    }
                    Sprite sprite = Resources.Load<Sprite>("Arts/UI/Star");
                    sr.sprite = sprite; // 조건을 만족하면 색상을 흰색으로 변경
                    skills.Add(skill.TileObject);
                }
            }
            
            SetStarEffect();
        }
    }

    private void SetStarEffect()
    {
        //배치 씬 인접효과 비주얼을 위한 코드
        int conditionCount = currentDragObject.GetComponentInChildren<CombinedStarCell>().GetStarSkill().GetConditionCount();
        int activeStarCount = 0;
        foreach (var star in currentDragObject.GetComponentInChildren<CombinedStarCell>().GetComponentsInChildren<SpriteRenderer>())
        {
            if (star.sprite.name == "Star")
            {
                activeStarCount++;
            }
        }
        
        /*if (activeStarCount >= conditionCount)
        {
            currentDragObject.GetComponentInChildren<CombineCell>().GetSprite().color = new Color(1f, 1f, 0.5f, 1f);
        }
        else
        {
            currentDragObject.GetComponentInChildren<CombineCell>().GetSprite().color = new Color(1f, 1f, 1, 1f);
        }*/
        
        if (activeStarCount >= conditionCount)
        {
            foreach (var star in currentDragObject.GetComponentsInChildren<LightController>())
            {
                star.SetLightProperties(6,3,0.8f,0.1f,0.4f);
                
            }
        }
        else
        {
            foreach (var star in currentDragObject.GetComponentsInChildren<LightController>())
            {
                star.SetLightProperties(2,0,1f,0.1f,0.2f);
            }
        }
    }

    private void SetGridSprite(bool isPreview = false)
    {
        //그냥 cell의 포지션을 GridSpriteController에 보냄
        Cell[] allCells = currentDragObject.GetComponentsInChildren<Cell>();
        List<Vector3Int> cellsPos = new List<Vector3Int>();

        foreach (var cell in allCells)
        {
            if (cell.GetType() == typeof(Cell))
                cellsPos.Add(GridManager.Instance.WorldToGridPosition(cell.transform.position));
        }
        GridManager.Instance.GridSpriteController.SetSprite(cellsPos.ToArray());
        if (isPreview)
        {
            foreach (Vector3Int pos in cellsPos)
            {
                GridManager.Instance.SetCellSpritePreview(pos);
            }
        }
        //까지
    }

    private void RotateObject()
    {
        if (currentDragObject == null) return;
        
        smoothRotator.RotateZ(currentDragObject.transform,UpdatePreviewCell);
    }

    /// <summary>
    /// 팔거나 배치할려고 할때 이 함수를 실행해주세요
    /// </summary>
    public void TryStopRotate()
    {
        if (currentDragObject == null) return;

        smoothRotator.TryStopRotate();
        UpdatePreviewCell();
    }

}
