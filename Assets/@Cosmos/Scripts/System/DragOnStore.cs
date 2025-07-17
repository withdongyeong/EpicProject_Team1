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
    private StorageArea _storageArea;

    private void Awake()
    {
        storeSlot = GetComponent<StoreSlot>();
        _storageArea = FindAnyObjectByType<StorageArea>();
    }
    

    protected override void BeginDrag()
    {
        originalObject = storeSlot.GetObject();
        //dragObject = Instantiate(originalObject);
        if(originalObject != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            Camera mainCamera = Camera.main;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
            worldPosition.z = 0f; // 2D 게임이므로 z값을 0으로 설정
                                  //드래그 오브젝트 생성 및 위치 초기화
            dragObject = Instantiate(originalObject, worldPosition, originalObject.transform.rotation);
            DragManager.Instance.LocalPos = Vector3.zero;
        }
        else
        {
            //이미 상점에서 산 오브젝트를 드래그 하려 했으므로, null로 반환합니다.
            dragObject = null;
        }
       



        //dragObject.name = "dragObject";
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
                if(cell.GetType() == typeof(Cell))
                {
                    cell.AddComponent<BoxCollider2D>();
                    HoverTileInfo hti = cell.AddComponent<HoverTileInfo>();
                    hti.SetTileObject(g.GetComponent<TileObject>());
                }
                
            }
            return;
        }
        
        
        //3. 보관함에 배치한다면
        if(_storageArea == null) 
        {
            _storageArea = FindAnyObjectByType<StorageArea>();
        }
        if (storeSlot.CanPurchase() &&_storageArea.IsCanStore)
        {
            GameObject g = DragManager.Instance.GetCurrentDragObject();
            foreach (Cell cell in g.GetComponentsInChildren<Cell>())
            {
                if(cell.GetType() == typeof(Cell))
                {
                    cell.AddComponent<BoxCollider2D>();
                    HoverTileInfo hti = cell.AddComponent<HoverTileInfo>();
                    hti.SetTileObject(g.GetComponent<TileObject>());
                }
                
            }
            
            //튜토리얼 중이고 , 회전 퀘스트 중이면.. 
            if (GameManager.Instance.IsInTutorial)
            {
                StorageQuest quest = GuideHandler.instance.CurrentQuest as StorageQuest;
                if (quest != null)
                {
                    quest.count++;
                }
            }
            //까지입니다 ..
            
            
            
            
            _storageArea.StoreTileObject(GetComponent<TileObject>());
            foreach (var coll in DragManager.Instance.GetCurrentDragObject().GetComponentsInChildren<Collider2D>())
            {
                coll.enabled = true;
            }
            DragManager.Instance.GetCurrentDragObject().transform.SetParent(GridManager.Instance.TilesOnGrid.gameObject.transform);
            DragManager.Instance.GetCurrentDragObject().AddComponent<DragOnStorage>();
            storeSlot.BuyObject();
            Destroy(this);
            return;
        }
        
        
        
        //그 외
        DragManager.Instance.DestroyObject();
    }

    protected override GameObject GetDraggableObject()
    {
        return dragObject;
    }

}
