using UnityEngine;
using TMPro;

public class InfoPanel : MonoBehaviour
{
    private TileObject currentTileObject; // 현재 표시 중인 TileObject
    private RectTransform rectTransform;
    private Canvas canvas;
    private Camera mainCamera;
    [SerializeField] private TextMeshProUGUI nameText; // 이름 텍스트

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        mainCamera = Camera.main;
        gameObject.SetActive(false); // 초기 비활성화
    }

    /// <summary>
    /// TileObject 정보를 설정하고 패널을 표시
    /// </summary>
    public void Show(TileObject tileObject, Vector3 position, bool isUIElement)
    {
        currentTileObject = tileObject;
        gameObject.SetActive(true);

        // TileObject 정보 설정
        nameText.text = currentTileObject.GetTileData().TileName;

        // 위치 업데이트
        UpdatePosition(position, isUIElement);
    }

    /// <summary>
    /// 패널 숨기기
    /// </summary>
    public void Hide()
    {
        currentTileObject = null;
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
        Vector2 offset = new Vector2(20f, 50f);
        Vector2 newPos = mousePos + offset;

        // 캔버스 경계 내로 제한
        Rect canvasRect = (canvas.transform as RectTransform).rect;
        newPos.x = Mathf.Clamp(newPos.x, canvasRect.xMin, canvasRect.xMax - rectTransform.rect.width);
        newPos.y = Mathf.Clamp(newPos.y, canvasRect.yMin + rectTransform.rect.height, canvasRect.yMax);

        rectTransform.anchoredPosition = newPos;
    }
}