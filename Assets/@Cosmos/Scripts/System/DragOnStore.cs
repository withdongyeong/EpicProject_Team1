using System;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// StoreSlot에서 Grid로 드래그할 때 사용되는 DraggableObject 클래스입니다.
/// </summary>
public class DragOnStore : DraggableObject
{

    private StoreSlot storeSlot;
    
    private GameObject dragObject;
    private GameObject originalObject;

    private void Awake()
    {
        storeSlot = GetComponent<StoreSlot>();
    }
    

    protected override void BeginDrag()
    {
        originalObject = storeSlot.GetObject();
        dragObject = Instantiate(originalObject);
        dragObject.name = "dragObject";
    }                                 
    
    protected override void EndDrag()
    {
        //1. 그리드 안에 배치 가능하고 구매 가능하다면
        if (storeSlot.CanPurchase() && DragManager.Instance.CanPlaceTile())
        {
            DragManager.Instance.PlaceObject();
            storeSlot.BuyObject();
            DragManager.Instance.GetCurrentDragObject().AddComponent<DragOnGrid>();
            GameObject g = DragManager.Instance.GetCurrentDragObject();
            foreach (Cell cell in g.GetComponentsInChildren<Cell>())
            {
                cell.AddComponent<BoxCollider2D>();
                HoverTileInfo hti = cell.AddComponent<HoverTileInfo>();
                hti.SetTileObject(g.GetComponent<TileObject>());
            }
            
            
            return;
        }
        //2. 그리드 밖에 배치한다면
        
        //3. 보관함에 배치한다면
        
        //그 외
        DragManager.Instance.DestroyObject();
    }

    protected override GameObject GetDraggableObject()
    {
        return dragObject;
    }

}
