using UnityEngine;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    private TileObject currentTileObject; // 현재 표시 중인 TileObject
    private RectTransform rectTransform;
    private Canvas canvas;
    private Camera mainCamera;
    private GameObject nameTextPrefab; // 이름 텍스트
    //private GameObject descriptionTextPrefab; // 설명 텍스트
    private GameObject costTextPrefab; // 비용 텍스트
    private GameObject categoryTextPrefab; // 종류 텍스트
    private InfoTextRenderer textRenderer; //곽민준이 짠 설명 텍스트 및 밑의 구분선 보여주는 스크립트입니다
    

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
        gameObject.SetActive(false); // 초기 비활성화
        nameTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/NameText");
        //descriptionTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/DescriptionText");
        costTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/CostText");
        categoryTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/CategoryText");
        textRenderer = GetComponent<InfoTextRenderer>();
    }

    /// <summary>
    /// TileObject 정보를 설정하고 패널을 표시
    /// </summary>
    public void Show(TileObject tileObject, Vector3 position, bool isUIElement)
    {
        currentTileObject = tileObject;
        gameObject.SetActive(true);

        // 이름 텍스트 설정
        TextMeshProUGUI nameText = Instantiate(nameTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        nameText.text = currentTileObject.GetTileData().TileName;
        switch (currentTileObject.GetTileData().TileGrade)
        {
            case TileGrade.Normal:
                nameText.color = Color.white;
                break;
            case TileGrade.Rare:
                nameText.color = Color.blue;
                break;
            case TileGrade.Epic:
                nameText.color = new Color(0.5f, 0f, 1f); // 보라색
                break;
            case TileGrade.Legendary:
                nameText.color = Color.yellow;
                break;
            default:
                nameText.color = Color.white;
                break;
        }
        // 설명 텍스트 설정
        //TextUIResizer descriptionText = Instantiate(descriptionTextPrefab, transform).GetComponent<TextUIResizer>();
        //descriptionText.SetText(currentTileObject.GetTileData().Description);

        //이제 이거 대신 이거 쓰면 됩니다
        textRenderer.InstantiateDescriptionText(currentTileObject);

        // 비용 텍스트 설정
        TextMeshProUGUI costText = Instantiate(costTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        costText.text = $"{currentTileObject.GetTileData().TileCost} Gold";
        costText.color = Color.yellow; // 비용 텍스트 색상 설정

        // 위치 업데이트
        transform.position = position;
        
    }

    /// <summary>
    /// 패널 숨기기
    /// </summary>
    public void Hide()
    {
        currentTileObject = null;
        // 자식 오브젝트 제거
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        gameObject.SetActive(false);
    }
    
}