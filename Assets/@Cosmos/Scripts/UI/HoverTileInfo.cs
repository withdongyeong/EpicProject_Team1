using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class HoverTileInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InfoPanel infoPanel;
    private TileObject tileObject; // TileObject 컴포넌트 참조
    private GameObject combinedStarCell; // 스타셀의 부모 오브젝트
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
        if (scene.name == "BuildingScene")
        {
            infoPanel = FindAnyObjectByType<InfoPanel>(FindObjectsInactive.Include);
            if (infoPanel == null)
            {
                Debug.LogError("InfoPanel 없음.");
            }
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
        if (GetComponent<Cell>() != null)
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
            if (tileObject != null)
                infoPanel.Show(tileObject, position, true);
        }
        // 콤바인드스타셀 활성화
        if (tileObject.CombinedStarCell != null)
        {
            tileObject.CombinedStarCell.SetActive(true);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (infoPanel != null)
        {
            infoPanel.Hide();
        }
        // 콤바인드스타셀 비활성화
        if (tileObject.CombinedStarCell != null)
        {
            tileObject.CombinedStarCell.SetActive(false);
        }
    }
    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(HandleSceneLoaded);
    }
}