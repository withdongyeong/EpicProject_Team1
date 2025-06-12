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
        textRenderer.InstantiateDescriptionText(currentTileObject.GetTileData().Description);

        //// 비용 텍스트 설정
        //costText.text = $"Cost: {currentTileObject.GetTileData().TileCost}";
        //// 종류 텍스트 설정
        //string category = "무기";
        //switch(currentTileObject.GetTileData().TileCategory)
        //{
        //    case TileCategory.Weapon:
        //        category = "무기";
        //        break;
        //    case TileCategory.MagicCircle:
        //        category = "마법진";
        //        break;
        //    case TileCategory.Armor:
        //        category = "방어구";
        //        break;
        //    case TileCategory.Consumable:
        //        category = "소모품";
        //        break;
        //    case TileCategory.Accessory:
        //        category = "장신구";
        //        break;
        //    case TileCategory.Summon:
        //        category = "소환수";
        //        break;
        //    default:
        //        category = "기타";
        //        break;
        //}
        //categoryText.text = category;

        // 위치 업데이트
        UpdatePosition(position, isUIElement);
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

    private void UpdatePosition(Vector3 position, bool isUIElement)
    {
        Vector2 mousePos;

        if (isUIElement)
        {
            // UI 요소: 스크린 좌표를 캔버스 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                position,
                canvas.worldCamera,
                out mousePos
            );
        }
        else
        {
            // 3D/2D 오브젝트: 월드 좌표를 캔버스 좌표로 변환
            Vector2 screenPos = mainCamera.WorldToScreenPoint(position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                canvas.worldCamera,
                out mousePos
            );
        }

        // 정보 UI 위치 설정 (오프셋 적용)
        //Vector2 offset = new Vector2(20f, 50f);
        Vector2 newPos = mousePos;

        // 캔버스 경계 내로 제한
        Rect canvasRect = (canvas.transform as RectTransform).rect;
        newPos.x = Mathf.Clamp(newPos.x, canvasRect.xMin, canvasRect.xMax - rectTransform.rect.width);
        newPos.y = Mathf.Clamp(newPos.y, canvasRect.yMin + rectTransform.rect.height, canvasRect.yMax);

        rectTransform.anchoredPosition = newPos;
    }
}