using System;
using UnityEngine;



/// <summary>
/// Grid에서 Grid로 드래그할 때 사용되는 DraggableObject 클래스입니다.
/// </summary>
public class DragOnGrid : DraggableObject
{
    private Vector3 originalPosition;
    private float rotateZ;
    private SellTilePanel _sellTilePanel;
    private TileObject _tileObject;


    private void Awake()
    {
        _sellTilePanel = FindAnyObjectByType<SellTilePanel>();
        _tileObject = GetComponent<TileObject>();
    }

    protected override void BeginDrag()
    {

        ShowSellPanel();
        
        // 드래그 시작 시 원래 위치와 회전값 저장
        rotateZ = transform.rotation.eulerAngles.z;
        //드래그 시작 시 원래 위치 저장
        originalPosition = transform.position;
        rotateZ = transform.rotation.eulerAngles.z;
        GameObject dragObject = gameObject;
        
        //StarCell도 Cell이기 때문에 잡아냅니다
        foreach(Cell cell in dragObject.GetComponentsInChildren<Cell>())
        {
            
            Transform t = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(t.position);
            if (cell.GetType() == typeof(StarCell))
            {
                StarBase starSkill = (cell as StarCell).GetStarSkill();
                GridManager.Instance.RemoveStarSkill(gridPos, starSkill);
                continue;
            }
            cell.GetComponent<Collider2D>().enabled = false;
            GridManager.Instance.ReleaseCell(gridPos);
        }
    }


    protected override void EndDrag()
    {
        HideSellPanel();
        
        
        
        //1. 그리드 안에 배치 가능하다면 -> 배치
        if (DragManager.Instance.CanPlaceTile())
        {
            foreach (var coll in gameObject.GetComponentsInChildren<Collider2D>())
            {
                coll.enabled = true;
            }
            DragManager.Instance.PlaceObject();
            return;
        }

        if (_sellTilePanel.IsCanSell)
        {
            _sellTilePanel.SellTileObject(_tileObject);
            return;
        }
        //2. 그리드 안에 배치 불가능하다면 -> 원래 위치로
        if (!DragManager.Instance.CanPlaceTile())
        {
            foreach (var coll in gameObject.GetComponentsInChildren<Collider2D>())
            {
                coll.enabled = true;
            }
            //원래 위치로 되돌리기
            DragManager.Instance.SetObjectPosition(originalPosition);
            //원래 회전으로 되돌리기
            transform.rotation = Quaternion.Euler(0f, 0f, rotateZ);
            DragManager.Instance.PlaceObject();
            return;
        }
        
        DragManager.Instance.DestroyObject();
    }
    
    private void ShowSellPanel()
    {
        if (_sellTilePanel == null)
        {
            _sellTilePanel = FindAnyObjectByType<SellTilePanel>();
        }
        
        _sellTilePanel.ShowPanel(_tileObject.GetTileData().TileCost);
    }
    
    public void HideSellPanel()
    {
        if (_sellTilePanel == null)
        {
            _sellTilePanel = FindAnyObjectByType<SellTilePanel>();
        }
        
        _sellTilePanel.HidePanel();
    }
}
