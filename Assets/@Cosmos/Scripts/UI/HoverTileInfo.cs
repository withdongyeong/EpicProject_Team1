using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverTileInfo : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler
{
    private InfoPanel infoPanel;
    private TileObject tileObject; // TileObject 컴포넌트 참조
    private GameObject combinedStarCell; // 스타셀의 부모 오브젝트

    private void Awake()
    {
        infoPanel = FindAnyObjectByType<InfoPanel>(FindObjectsInactive.Include); 
        if (infoPanel == null)
        {
            Debug.LogError("InfoPanel 없음.");
        }
    }

    public void SetTileObject(TileObject tileObject)
    {
        this.tileObject = tileObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 position;
        // 배치된 타일인지 확인
        if(GetComponent<Cell>() != null) 
        {
            position = new Vector3(1230f, 600f, 0);
        }
        else
        {
            position = new Vector3(880f, 700f, 0);
        }

            ////현재 마우스 포인터 위치
            //Vector3 mousePosition = Input.mousePosition;
        if (infoPanel != null)
        {
            if(tileObject != null)
                infoPanel.Show(tileObject, position, true);
        }

        if(tileObject.CombinedStarCell != null)
        {
            tileObject.CombinedStarCell.SetActive(true); // 스타셀의 부모 오브젝트 활성화
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel != null)
        {
            infoPanel.Hide();
        }

        if (tileObject.CombinedStarCell != null)
        {
            tileObject.CombinedStarCell.SetActive(false); // 스타셀의 부모 오브젝트 비활성화
        }
    }
}
