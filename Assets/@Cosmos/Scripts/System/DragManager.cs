using UnityEngine;
using UnityEngine.EventSystems;


/// <summary>
/// DragManager는 드래그 이벤트를 관리하는 싱글턴 클래스입니다.
/// 게임에서 이루어지는 모든 드래그 동작을 관리,처리 합니다.
/// 드래그는 Tile을 제외하고 그 무엇도 드래그 되지 않습니다.
/// 현재 1.상점->grid   2.grid-> grid  
/// </summary>
public class DragManager : Singleton<DragManager> ,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameObject currentDragObject;
    private Camera mainCamera;
    
    private void Awake()
    {
        mainCamera = Camera.main;
    } 
    
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDrag(eventData);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        Drag(eventData);
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        EndDrag(eventData);
    }
    
    
    private void BeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return; // 왼쪽 버튼이 아닐 경우 드래그 시작하지 않음
        //마우스 포인터로 누른 지점의 월드 포지션을 가져옵니다
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        worldPos.z = 0f;

        //이게 누른 지점의 그리드 포지션입니다
        Vector3Int clickedGridPosition = GridManager.Instance.WorldToGridPosition(worldPos);
        Debug.Log(worldPos);
        Debug.Log(clickedGridPosition);
    }
    
    private void Drag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return; // 왼쪽 버튼이 아닐 경우 드래그 시작하지 않음
    }
    
    private void EndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return; // 왼쪽 버튼이 아닐 경우 드래그 시작하지 않음
    }
}
