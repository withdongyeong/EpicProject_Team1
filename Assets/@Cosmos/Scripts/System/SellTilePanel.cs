using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class SellTilePanel : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    private RectTransform _rectTransform;
    private Image _image;
    private DragManager _dm;
    private TextMeshProUGUI _sellText;
    private bool _isCanSell = false; // 판매 가능 여부
    public bool IsCanSell => _isCanSell;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _dm = DragManager.Instance;
        _sellText = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    
    public void OnDrop(PointerEventData eventData)
    {
        // 드롭된 오브젝트가 SellTilePanel에 드롭되었을 때의 처리
        if (eventData.pointerDrag != null)
        {
            Debug.Log("드롭된 오브젝트: " + eventData.pointerDrag.name);
        }
    }

    public void SellTileObject(TileObject tileObject)
    {
        GoldManager.Instance.ModifyCurrentGold((tileObject.GetTileData().TileCost + 1) / 2);
        EventBus.PublishTileSell(tileObject);
        DragManager.Instance.DestroyObject();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_dm.IsDragging && _dm.GetCurrentDragObject() != null)
        {
            _isCanSell = true;
            _image.color = new Color(1, 0.5f, 0, 0.8f); // 예시로 주황색으로 변경
        }
        else
        {
            _isCanSell = false;
            _image.color = new Color(1, 1, 1, 0.8f); // 원래 색상으로 되돌림
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        _image.color = new Color(1, 1, 1, 0.8f);
        _isCanSell = false; // 마우스가 패널을 벗어나면 판매 가능 여부를 false로 설정
    }

    public void ShowPanel(int tileCost)
    {
        gameObject.SetActive(true);
        _rectTransform.anchoredPosition = Vector2.down * 300f;;
        
        _sellText.text = "판매 골드 : " + ((tileCost + 1) / 2) + "G";
        _rectTransform.DOAnchorPos(Vector2.zero,0.25f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            _rectTransform.anchoredPosition = Vector2.zero;
        });
    }
    
    public void HidePanel()
    {
        _rectTransform.anchoredPosition = Vector2.zero;
        _rectTransform.DOAnchorPos(Vector2.down*300f,0.25f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            _rectTransform.anchoredPosition = Vector2.down * 300f;
        });
    }
}
