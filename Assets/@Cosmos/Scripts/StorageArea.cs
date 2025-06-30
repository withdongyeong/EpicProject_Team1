using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageArea : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler
{
    private DragManager _dm;
    private Image _image;
    private bool _isCanStore = false; // 저장 가능 여부
    public bool IsCanStore => _isCanStore;


    private void Awake()
    {
        _dm = DragManager.Instance;
        _image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_dm.IsDragging && _dm.GetCurrentDragObject() != null)
        {
            _isCanStore = true;
            _image.color = new Color(0.8f, 0.8f, 1,0.04f); // 예시로 주황색으로 변경
        }
        else
        {
            _isCanStore = false;
            _image.color = new Color(0.8f, 0.8f, 0, 0); // 원래 색상으로 되돌림
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    { 
        _isCanStore = false;
        _image.color = new Color(1, 1, 1, 0f); // 마우스가 패널을 벗어나면 색상 원래대로
    }
    
    public void StoreTileObject(TileObject tileObject)
    {
        // 저장 로직 구현
        // 예: GridManager.Instance.StoreTile(tileObject);
        Debug.Log($"TileObject {tileObject.name} has been stored.");
        
    }
}
