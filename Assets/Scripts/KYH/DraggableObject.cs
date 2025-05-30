using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.iOS;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 offset;
    private float zCoord;
    private Camera cam;
    

    private void Awake()
    {
        cam = Camera.main;
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        zCoord = cam.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPos();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = GetMouseWorldPos() + offset;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        
        // 드래그 종료 시 추가 작업이 필요하면 여기에 작성
    }
    
    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zCoord; // 카메라와의 거리 유지
        return cam.ScreenToWorldPoint(mousePoint);
    }
}
