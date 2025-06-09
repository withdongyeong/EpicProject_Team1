using UnityEngine;

public class TileInfoUI : MonoBehaviour
{
    private TileObject tileObject;
    private InfoPanel infoPanel;
    private Vector3 panelPos; // 마우스 위치

    private void Start()
    {
        // InfoPanel 컴포넌트 찾기
        infoPanel = FindObjectOfType<InfoPanel>(true);
        if (infoPanel == null)
        {
            Debug.LogError("InfoPanel not found in the scene.");
        }
    }

    private void OnMouseOver()
    {
        //마우스 위치의 월드 포지션
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
        mousePos.z = 0f;

        //마우스 위치의 그리드 포지션
        Vector3Int gridPosition = GridManager.Instance.WorldToGridPosition(mousePos);

        //비어있는지 확인
        if (GridManager.Instance.IsCellAvailable(gridPosition))
        {
            HideInfoPanel();
            return;
        }

        //통합된 셀 스크립트의 타일오브젝트를 가져오기
        CombineCell cC = GridManager.Instance.GetCellData(gridPosition).GetObjectData();
        tileObject = cC.tileObject;

        //패널 위치를 설정하기 위해 오브젝트 기준 위치로 변환
        gridPosition = GridManager.Instance.WorldToGridPosition(cC.tile.transform.position);
        Vector3 offset = new Vector3(4f, 0f, 0f); // 패널 위치 오프셋
        panelPos = GridManager.Instance.GridToWorldPosition(gridPosition) + offset;

        ShowInfoPanel();
    }

    private void OnMouseExit()
    {
        HideInfoPanel();
    }


    private void ShowInfoPanel()
    {
        infoPanel.Show(tileObject, panelPos, false);
    }

    private void HideInfoPanel()
    {
        if (infoPanel != null)
        {
            infoPanel.Hide();
        }
    }
}
