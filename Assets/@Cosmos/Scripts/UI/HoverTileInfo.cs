using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTileInfo : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler
{
    private InfoPanel infoPanel;
    private TileObject tileObject; // TileObject 컴포넌트 참조
    private void Awake()
    {
        infoPanel = FindAnyObjectByType<InfoPanel>(FindObjectsInactive.Include); 
        if (infoPanel == null)
        {
            Debug.LogError("InfoPanel 없음.");
        }
    }

    private void Start()
    {
    }


    public void SetTileObject(TileObject tileObject)
    {
        this.tileObject = tileObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //현재 마우스 포인터 위치
        Vector3 mousePosition = Input.mousePosition;
        if (infoPanel != null)
        {
            if(tileObject != null)
                infoPanel.Show(tileObject,mousePosition, true);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel != null)
        {
            infoPanel.Hide();
        }
    }
}
