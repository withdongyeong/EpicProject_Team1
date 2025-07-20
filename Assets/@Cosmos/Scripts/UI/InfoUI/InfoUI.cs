using UnityEngine;
using UnityEngine.EventSystems;

public class InfoUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TileObject tileObject; // TileObject 컴포넌트 참조
    [SerializeField] private bool isUIElement; // 이 오브젝트가 UI인지 여부
    private InfoPanel infoPanel;

    private void Awake()
    {
        infoPanel = FindAnyObjectByType<InfoPanel>(FindObjectsInactive.Include);
        if(infoPanel == null)
        {
            Debug.LogError("InfoPanel not found in the scene. Please ensure it is present.");
        }
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

    private void ShowInfoPanel()
    {
        if (tileObject != null)
        {
            Vector3 position = isUIElement ? transform.position : transform.position;
            infoPanel.Show(tileObject.GetTileData(), position + new Vector3(-300f, 0, 0), isUIElement);
        }
    }

    private void HideInfoPanel()
    {
        infoPanel.Hide();
    }
}