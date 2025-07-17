using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class InfoPanel : MonoBehaviour
{
    [Header("Grade Border Images (Outer)")]
    [SerializeField] private Sprite normalGradeBorder;
    [SerializeField] private Sprite rareGradeBorder;
    [SerializeField] private Sprite epicGradeBorder;
    [SerializeField] private Sprite legendaryGradeBorder;
    [SerializeField] private Sprite mythicGradeBorder;
    
    [Header("Common Background Image (Inner)")]
    [SerializeField] private Sprite commonBackgroundSprite;
    
    [Header("UI Components")]
    [SerializeField] private Image borderImage; // 겉 테두리 이미지 컴포넌트
    [SerializeField] private Image backgroundImage; // 내부 배경 이미지 컴포넌트 (Tiled)
    [SerializeField] private GameObject textObject; // Text 오브젝트 (InfoTextRenderer가 있는)
    
    private TileInfo currentTileInfo; // 현재 표시 중인 TileInfo
    private RectTransform rectTransform;
    private Canvas canvas;
    private GameObject nameTextPrefab; // 이름 텍스트
    //private GameObject descriptionTextPrefab; // 설명 텍스트
    private InfoTextRenderer textRenderer; //곽민준이 짠 설명 텍스트 및 밑의 구분선 보여주는 스크립트입니다

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        gameObject.SetActive(false); // 초기 비활성화
        nameTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/HeadText");
        //descriptionTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/DescriptionText");
        
        // textObject에서 InfoTextRenderer 컴포넌트 가져오기
        if (textObject != null)
        {
            textRenderer = textObject.GetComponent<InfoTextRenderer>();
        }
        
        // 내부 배경 이미지를 타일 방식으로 설정
        SetupTiledBackground();
    }

    private void SetupTiledBackground()
    {
        if (backgroundImage != null && commonBackgroundSprite != null)
        {
            backgroundImage.sprite = commonBackgroundSprite;
            backgroundImage.type = Image.Type.Tiled; // 타일 방식으로 설정
        }
    }

    /// <summary>
    /// TileObject 정보를 설정하고 패널을 표시
    /// </summary>
    public void Show(TileInfo tileInfo, Vector3 position, bool isUIElement)
    {
        currentTileInfo = tileInfo;
        gameObject.SetActive(true);

        Debug.Log("여까지 들어옴");
        // 등급에 따른 테두리 이미지 설정
        SetBorderByGrade(currentTileInfo.TileGrade);

        // 이름 텍스트 설정 (textObject 하위에 생성)
        Transform headText = Instantiate(nameTextPrefab, textObject.transform).transform;
        TextMeshProUGUI nameText = headText.GetChild(0).GetComponent<TextMeshProUGUI>();
        if(currentTileInfo.TileName == "???")
        {
            nameText.SetText("???");
        }
        else
        {
            LocalizedString localizedString_Name = new LocalizedString("EpicProject_Table", "Tile_TileName_" + currentTileInfo.TileName);
            localizedString_Name.StringChanged += (text) => nameText.text = text;
        }
            
        switch (currentTileInfo.TileGrade)
        {
            case TileGrade.Normal:
                nameText.color = Color.white;
                break;
            case TileGrade.Rare:
                nameText.color = new Color(0.7f, 0.9f, 1f);
                break;
            case TileGrade.Epic:
                nameText.color = new Color(0.9f, 0.7f, 1f); // 보라
                break;
            case TileGrade.Legendary:
                nameText.color = new Color(1f, 0.7f, 0.6f);
                break;
            case TileGrade.Mythic:
                nameText.color = new Color(0.65f, 0.9f, 0.85f);
                break;
            default:
                nameText.color = Color.white;
                break;
        }


        TextMeshProUGUI coolTimeText = headText.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        if(currentTileInfo.TileCoolTime != 0)
        {
            coolTimeText.text = currentTileInfo.TileCoolTime.ToString() + "s";
        }
        else
        {
            coolTimeText.gameObject.transform.parent.gameObject.SetActive(false);
        }
        
        textRenderer.InstantiateDescriptionText(currentTileInfo);

        // 위치 업데이트
        if (isUIElement)
        {
            // 오프셋 설정
            float offsetY = textObject.GetComponent<RectTransform>().sizeDelta.y * 0.5f + 200f;
            Vector2 offset = new Vector2(400f, offsetY);

            // 패널 위치 설정 (anchoredPosition 사용)
            rectTransform.position = (Vector2)position + new Vector2(
                (Input.mousePosition.x > Screen.width * 0.6f ? -1 : 1) * offset.x,
                (Input.mousePosition.y > Screen.height * 0.5f ? -1 : 1) * offset.y
            );
            // 화면 상단을 넘어가지 않도록 조정
            if (rectTransform.position.y > 700) 
            {
                rectTransform.position = new Vector2(rectTransform.position.x, 650);
            }
            // 화면 하단을 넘어가지 않도록 조정
            if (rectTransform.position.y < 450) 
            {
                rectTransform.position = new Vector2(rectTransform.position.x, 450);
            }
        }
        else
        {
            //// 월드 좌표를 화면 좌표로 변환
            //Vector3 tileObjectPosition = tileObject.transform.position;
            //Vector2 screenPoint = Camera.main.WorldToScreenPoint(tileObjectPosition);

            // 오프셋 설정
            Vector2 offset = new Vector2(650f, 150f);

            //// 화면 좌표를 캔버스 로컬 좌표로 변환
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(
            //    canvas.transform as RectTransform,
            //    screenPoint,
            //    canvas.worldCamera,
            //    out Vector2 localPoint
            //);

            // 패널 위치 설정 (anchoredPosition 사용)
            rectTransform.anchoredPosition = new Vector2(
                (Input.mousePosition.x > Screen.width * 0.65f ? -1 : 1) * offset.x,
                offset.y // Y 좌표는 고정 (수평으로만 이동하도록 설정)
            );
        }
        
    }

    private void SetBorderByGrade(TileGrade grade)
    {
        if (borderImage == null) return;

        switch (grade)
        {
            case TileGrade.Normal:
                borderImage.sprite = normalGradeBorder;
                break;
            case TileGrade.Rare:
                borderImage.sprite = rareGradeBorder;
                break;
            case TileGrade.Epic:
                borderImage.sprite = epicGradeBorder;
                break;
            case TileGrade.Legendary:
                borderImage.sprite = legendaryGradeBorder;
                break;
            case TileGrade.Mythic: //신화 등급 처리
                borderImage.sprite = mythicGradeBorder;
                break;
            default:
                borderImage.sprite = normalGradeBorder;
                break;
        }
    }


    /// <summary>
    /// 패널 숨기기
    /// </summary>
    public void Hide()
    {
        currentTileInfo= null;
        // 자식 오브젝트 제거 (textObject의 자식들만 제거)
        if (textObject != null)
        {
            foreach (Transform child in textObject.transform)
            {
                Destroy(child.gameObject);
            }
        }
        gameObject.SetActive(false);
    }
}