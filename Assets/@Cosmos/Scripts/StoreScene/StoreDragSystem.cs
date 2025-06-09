using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoreDragSystem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera cam;
    private Vector3 offset;
    private GameObject originalObject; // 원본 오브젝트
    private GameObject dragCopy;
    private bool isDragging = false;
    
    private StoreSlot storeSlot;

    private SmoothRotator rotator;


    private void Awake()
    {
        cam = Camera.main;
        storeSlot = GetComponent<StoreSlot>();
        rotator = gameObject.AddComponent<SmoothRotator>();
    }

    private void Update()
    {
        if (isDragging)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotatePreviewBlock();
            }
        }
    }

    #region Event Handlers

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        originalObject = storeSlot.GetObject(); // StoreSlot에서 원본 오브젝트를 가져옵니다
        if (originalObject == null)
        {
            Debug.Log("오브젝트가 할당되지 않았습니다.");
            return; // 오브젝트가 없으면 드래그 시작하지 않음
        }
        // 드래그 위치 맞추기
        Vector3 worldPoint = cam.ScreenToWorldPoint(eventData.position);
        worldPoint.z = 0;
        
        
        dragCopy = Instantiate(originalObject);
        dragCopy.name = "DragCopy";
        
        offset = dragCopy.transform.position - worldPoint;
        dragCopy.transform.position = offset;

        // 복제본을 투명하게 설정
        foreach (var sr in dragCopy.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = sr.color;
            c.a = 0.5f;
            sr.color = c;
        }
        // 복제본의 충돌체를 비활성화하여 드래그 중 충돌 무시
        foreach (var coll in dragCopy.GetComponentsInChildren<Collider2D>())
        {
            coll.enabled = false;
        }

        isDragging = true;
        dragCopy.layer = LayerMask.NameToLayer("Ignore Raycast");
        
        
        
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (dragCopy == null) return;
        
        Vector3 worldPoint = cam.ScreenToWorldPoint(eventData.position);
        worldPoint.z = 0;
        dragCopy.transform.position = worldPoint;
        
        
        //이 부분은 나중에 InputSystem으로 변경하여 Gamepad에서도 작동하도록 해야합니다
        if(Input.GetMouseButtonDown(1))
        {
            RotatePreviewBlock();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragCopy == null) return;
        
        //배치 불가능하거나 혹은 물건을 사는데 실패한다면
        if (!CanPlaceBlock() || !storeSlot.BuyObject())
        {
            // 배치가 불가능한 경우 복제본 제거
            Debug.Log("배치 불가능한 위치이거나 돈이 부족하네요.");
            Destroy(dragCopy);
            return;
        }
        
        //배치 가능할시
        
        
        //배치 위치로 오브젝트 복사 
        Vector3 corePos = GridManager.Instance.GridToWorldPosition(GridManager.Instance.WorldToGridPosition(dragCopy.GetComponentInChildren<CombineCell>().coreCell.transform.position));
        GameObject placedObject = Instantiate(originalObject, corePos, dragCopy.transform.rotation, GridManager.Instance.transform);
        
        
        foreach (Cell cell in placedObject.GetComponentsInChildren<Cell>())
        {
            Transform child = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(child.position);
            GridManager.Instance.OccupyCell(gridPos, cell);
        }
        

        isDragging = false;
        Destroy(dragCopy);
    }

    #endregion
    
    
    
    //블럭 배치가 가능한지 확인하는 메서드
    private bool CanPlaceBlock()
    {
        if (dragCopy == null) return false;
        foreach (Cell cell in dragCopy.GetComponentsInChildren<Cell>())
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
    
    private void RotatePreviewBlock()
    {
        if (dragCopy == null) return;
        
        rotator.RotateZ(dragCopy.transform);
        /*rotationZ = (rotationZ +90) % 360; // 90도씩 회전
        dragCopy.transform.rotation = Quaternion.Euler(0, 0, rotationZ);*/
    }
    
}
