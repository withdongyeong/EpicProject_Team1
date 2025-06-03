using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class StoreObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Camera cam;
    private Vector3 offset;
    private int rotationZ = 0;
    public GameObject originalObject; // 원본 오브젝트
    private GameObject dragCopy;


    private void Awake()
    {
        cam = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
        // 드래그 위치 맞추기
        Vector3 worldPoint = cam.ScreenToWorldPoint(eventData.position);
        worldPoint.z = 0;
        
        
        dragCopy = Instantiate(originalObject);
        dragCopy.name = "DragCopy";
        
        offset = dragCopy.transform.position - worldPoint;
        dragCopy.transform.position = offset;
        Destroy(dragCopy.GetComponent<DragTransparentCopy>());

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
        
        //배치 불가능할시
        if (!CanPlaceBlock())
        {
            // 배치가 불가능한 경우 복제본 제거
            Debug.Log("배치 불가능한 위치입니다.");
            Destroy(dragCopy);
            return;
        }
        
        //배치 가능할시
        foreach (Cell cell in dragCopy.GetComponentsInChildren<Cell>())
        {
            Transform child = cell.transform;
            Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(child.position);
            GridManager.Instance.OccupyCell(gridPos);
        }
        
        //배치 위치로 오브젝트 복사 
        Vector3 corePos = GridManager.Instance.GridToWorldPosition(GridManager.Instance.WorldToGridPosition(dragCopy.GetComponentInChildren<CombineCell>().coreCell.transform.position));
        GameObject placedObject = Instantiate(originalObject, corePos, dragCopy.transform.rotation, GridManager.Instance.transform);
        Destroy(placedObject.GetComponent<DragTransparentCopy>());
        Destroy(dragCopy);
    }
    
    //블럭 배치가 가능한지 확인하는 메서드
    private bool CanPlaceBlock()
    {
        if (dragCopy == null) return false;
        foreach (Cell cell in dragCopy.GetComponentsInChildren<Cell>())
        {
            Transform child = cell.transform;
            Debug.Log("Checking position: " + child.position);
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
        rotationZ = (rotationZ +90) % 360; // 90도씩 회전
        dragCopy.transform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }
    
}
