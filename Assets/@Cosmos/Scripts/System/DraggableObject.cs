using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour ,IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 드래그 시작 시 실행될 기본 동작
    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!SceneLoader.IsInBuilding()) return; // 빌딩 씬이 아닐 경우 드래그 시작하지 않음
        if (eventData.button != PointerEventData.InputButton.Left) return; // 왼쪽 버튼이 아닐 경우 드래그 시작하지 않음
        BeginDrag();
        DragManager.Instance.BeginDrag(GetDraggableObject());
        
        
    }

    // 드래그 중 실행될 기본 동작
    public void OnDrag(PointerEventData eventData)
    {
        if(!SceneLoader.IsInBuilding()) return;
        if (eventData.button != PointerEventData.InputButton.Left) return; // 왼쪽 버튼이 아닐 경우 드래그 시작하지 않음
        Drag();
        DragManager.Instance.Drag();
        
    }

    // 드래그 종료 시 실행될 기본 동작
    public void OnEndDrag(PointerEventData eventData)
    {
        if(!SceneLoader.IsInBuilding()) return;
        if (eventData.button != PointerEventData.InputButton.Left) return; // 왼쪽 버튼이 아닐 경우 드래그 시작하지 않음
        EndDrag();
        DragManager.Instance.EndDrag();
        
        
    }

    protected virtual void BeginDrag()
    {
    }
    protected virtual void Drag()
    {
    }
    
    protected virtual void EndDrag()
    {
    }
    
    protected virtual GameObject GetDraggableObject() 
    {
        return gameObject;
    } 
}
