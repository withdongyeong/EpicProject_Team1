using UnityEngine;
using UnityEngine.EventSystems;

public class InfoUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TileObject tileObject; // TileObject 컴포넌트 참조
    [SerializeField] private bool isUIElement; // 이 오브젝트가 UI인지 여부
    private InfoPanel infoPanel;

    private void Awake()
    {
        infoPanel = FindAnyObjectByType<InfoPanel>();
    }

    public void SetTileObject(TileObject tileObject)
    {
        this.tileObject = tileObject;
    }

    // UI용: 마우스가 UI 위에 올라갔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isUIElement)
        {
            ShowInfoPanel();
        }
    }

    // UI용: 마우스가 UI에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isUIElement)
        {
            HideInfoPanel();
        }
    }

    // 오브젝트용: 마우스가 오브젝트 위에 올라갔을 때
    private void OnMouseEnter()
    {
        if (!isUIElement)
        {
            ShowInfoPanel();
        }
    }

    // 오브젝트용: 마우스가 오브젝트에서 벗어났을 때
    private void OnMouseExit()
    {
        if (!isUIElement)
        {
            HideInfoPanel();
        }
    }

    private void ShowInfoPanel()
    {
        if (tileObject != null)
        {
            Vector3 position = isUIElement ? Input.mousePosition : transform.position;
            infoPanel.Show(tileObject, position, isUIElement);
        }
    }

    private void HideInfoPanel()
    {
        infoPanel.Hide();
    }
}