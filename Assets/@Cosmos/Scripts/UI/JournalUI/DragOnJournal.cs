using Unity.VisualScripting;
using UnityEngine;

public class DragOnJournal : DraggableObject
{

    private JournalSlot journalSlot;

    private GameObject dragObject;
    private GameObject originalObject;

    private void Awake()
    {
        journalSlot = GetComponent<JournalSlot>();
    }

    protected override void BeginDrag()
    {
        if(FindAnyObjectByType<TestManager>().isCheatEnabled)
        originalObject = journalSlot.GetObject();
        //dragObject = Instantiate(originalObject);
        if (originalObject != null)
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
        //1. 그리드 안에 배치 가능하면
        if ( DragManager.Instance.CanPlaceTile())
        {
            DragManager.Instance.PlaceObject();
            DragManager.Instance.GetCurrentDragObject().AddComponent<DragOnGrid>();
            GameObject g = DragManager.Instance.GetCurrentDragObject();
            foreach (Cell cell in g.GetComponentsInChildren<Cell>())
            {
                if (cell.GetType() == typeof(Cell))
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
        StorageArea _storageArea;
        _storageArea = FindAnyObjectByType<StorageArea>();

        if (_storageArea.IsCanStore)
        {
            //튜토리얼 중이고 , 회전 퀘스트 중이면.. 
            //if (GameManager.Instance.IsInTutorial)
            //{
            //    StorageQuest quest = GuideHandler.instance.CurrentQuest as StorageQuest;
            //    if (quest != null)
            //    {
            //        quest.count++;
            //    }
            //}
            //까지입니다 ..
            _storageArea.StoreTileObject(dragObject.GetComponent<TileObject>());
            foreach (Cell cell in dragObject.GetComponentsInChildren<Cell>())
            {
                if (cell.GetType() == typeof(Cell))
                {
                    cell.AddComponent<BoxCollider2D>();
                    HoverTileInfo hti = cell.AddComponent<HoverTileInfo>();
                    hti.SetTileObject(dragObject.GetComponent<TileObject>());
                }

            }
            dragObject.transform.SetParent(GridManager.Instance.TilesOnGrid.gameObject.transform);
            dragObject.AddComponent<DragOnStorage>();

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
