using System.Collections.Generic;
using UnityEngine;


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
    private Sell_Blackhole sellScript;
    
    private bool isDragging = false; // 드래그 중인지 여부
    public bool IsDragging => isDragging; // 외부에서 드래그 상태를 확인할 수 있도록 공개

    



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
        }
    }
    
    public void BeginDrag(GameObject draggableObject)
    {
        isDragging = true;
        currentDragObject = draggableObject;
        
    }
    
    public void Drag()
    {
        if (currentDragObject == null || !isDragging)
            return;

        Vector3 mousePosition = Input.mousePosition;
        mainCamera = Camera.main;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f; // 2D 게임이므로 z값을 0으로 설정
        
        // 드래그 오브젝트 위치 업데이트
        currentDragObject.transform.position = worldPosition;
        // 그리드 미리보기 업데이트
        UpdatePreviewCell();
    }
    
    public void EndDrag()
    {
        
        isDragging = false;
        currentDragObject = null;
        UpdatePreviewCell();
    }


    public void SetObjectPosition(Vector3 position)
    {
        if (currentDragObject == null) return;
        currentDragObject.transform.position = position;
    }
    
    public void PlaceObject()
    {
        Vector3 corePos = currentDragObject.GetComponentInChildren<CombineCell>().GetCoreCell().transform.position;
        corePos = GridManager.Instance.GridToWorldPosition(GridManager.Instance.WorldToGridPosition(corePos));
        currentDragObject.transform.position = corePos;
        currentDragObject.transform.SetParent(GridManager.Instance.transform);
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
        TileObject tileObject = currentDragObject.GetComponent<TileObject>();
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
            GridManager.Instance.ChangeCellSprite(gridPos, true);
        }
    }

    private void RotateObject()
    {
        if (currentDragObject == null) return;
        
        smoothRotator.RotateZ(currentDragObject.transform,UpdatePreviewCell);
    }

    

    //이 밑은 판매와 관련된 메서드입니다
    public void AssignSell(Sell_Blackhole sell_Blackhole)
    {
        sellScript = sell_Blackhole;
    }

    public void ActivateSellText(GameObject tile)
    {
        sellScript.ActivateSellText((tile.GetComponent<TileObject>().GetTileData().TileCost+1)/2);
    }
    
    public bool TrySellTile(TileObject tile)
    {
        if(sellScript == null)
        {
            Debug.Log("판매를 담당하는 스크립트가 없어요");
            return false;
        }
        if(sellScript.CheckSell(tile))
        {
            //가격의 50%를 돌려받습니다. +1은 올림을 위해 적용하였습니다.
            GoldManager.Instance.ModifyCurrentGold((tile.GetTileData().TileCost + 1) / 2);
            EventBus.PublishTileSell(tile);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DisableSellText()
    {
        sellScript.DisableSellText();
    }

}
