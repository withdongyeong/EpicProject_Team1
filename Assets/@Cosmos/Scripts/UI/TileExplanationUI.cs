using TMPro;
using UnityEngine;

public class TileExplanationUI : MonoBehaviour
{
    public GameObject infoPanel;      // 띄울 UI 패널
    public TextMeshProUGUI infoText;      // 패널 안에 표시할 텍스트

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        infoPanel.SetActive(false); // 시작 시 비활성화
    }

    void Update()
    {
        ShowInfoOnHover();
    }

    void ShowInfoOnHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            TileObject tileObject = hit.collider.GetComponent<TileObject>();

            if (tileObject != null) // 대상 오브젝트는 "InfoObject" 태그를 갖도록 설정
            {
                infoPanel.SetActive(true);
                infoPanel.transform.position = Input.mousePosition + new Vector3(10, -10); // 마우스 옆에 표시
                infoText.text = tileObject.Description;
                return;
            }
        }

        // 아무것도 안 가리킬 때
        infoPanel.SetActive(false);
    }
}
