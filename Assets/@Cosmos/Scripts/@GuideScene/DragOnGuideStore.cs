using UnityEngine;
using Unity.VisualScripting;


public class DragOnGuideStore : DraggableObject
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
        Vector3 mousePosition = Input.mousePosition;
        Camera mainCamera = Camera.main;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f; // 2D 게임이므로 z값을 0으로 설정
        //드래그 오브젝트 생성 및 위치 초기화
        dragObject = Instantiate(originalObject,worldPosition,originalObject.transform.rotation);
        DragManager.Instance.LocalPos = Vector3.zero;
        //dragObject.name = "dragObject";
    }                                 
    
    protected override void EndDrag()
    {
        //1. 그리드 안에 배치 가능하고 구매 가능하다면
        if (storeSlot.CanPurchase() && DragManager.Instance.CanPlaceTile())
        {
            DragManager.Instance.PlaceObject();
            PlaceQuest quest = GuideHandler.instance.CurrentQuest as PlaceQuest;
            if (quest != null)
            {
                quest.tilesPlaced++;
            }
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
