using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class HoverTileInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InfoPanel infoPanel;
    private TileObject tileObject; // TileObject 컴포넌트 참조
    private GameObject combinedStarCell; // 스타셀의 부모 오브젝트
    private bool isInBuilding = true; // 초기화 여부

    private void Awake()
    {
        EventBus.SubscribeSceneLoaded(HandleSceneLoaded);
        infoPanel = FindAnyObjectByType<InfoPanel>(FindObjectsInactive.Include);
        if (infoPanel == null)
        {
            Debug.LogError("InfoPanel 없음.");
        }
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(SceneLoader.IsInBuilding())
        {
            infoPanel = FindAnyObjectByType<InfoPanel>(FindObjectsInactive.Include);
            if (infoPanel == null)
            {
                Debug.LogError("InfoPanel 없음.");
            }
            isInBuilding = true;
        }
        else if (SceneLoader.IsInStage())
        {
            isInBuilding = false;
        }
    }


    public void SetTileObject(TileObject tileObject)
    {
        this.tileObject = tileObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(DragManager.Instance.IsDragging) return;
        Vector3 position;
        // 배치된 타일인지 확인
        if (GetComponent<Cell>() != null)
        {
            position = new Vector3(1750f, 700f, 0); // RectPosition을 이렇게 하면 좀 불안하긴 한데 ..
        }
        else
        {
            position = new Vector3(1750f, 700f, 0);
        }
        ////현재 마우스 포인터 위치
        //Vector3 mousePosition = Input.mousePosition;
        if (infoPanel != null)
        {
            if (tileObject != null)
            {
                if (GetComponent<Cell>() != null)
                {
                    // Cell 컴포넌트가 있는 경우
                    infoPanel.Show(tileObject.GetTileData(), position, false);
                }
                else
                {
                    // Cell 컴포넌트가 없는 경우
                    position = transform.position;
                    infoPanel.Show(tileObject.GetTileData(), position, true);
                }
            }
        }
        if (isInBuilding)
        {
            // 콤바인드스타셀 활성화
            tileObject.ShowStarCell();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel != null)
        {
            infoPanel.Hide();
        }
        // 콤바인드스타셀 비활성화
        tileObject.HideStarCell();
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(HandleSceneLoaded);
    }

}

