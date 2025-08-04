using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

/// <summary>
/// 무한모드 재도전 버튼 컨트롤러
/// 버튼 클릭 시 무한모드 카운트 증가 및 스테이지 재시작
/// 호버 시 강화 정보 툴팁 표시
/// </summary>
public class InfiniteButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;

    [SerializeField] private GameObject tooltipUI;

    /// <summary>
    /// 컴포넌트 초기화 및 버튼 이벤트 설정
    /// </summary>
    void Start()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogWarning("[InfiniteButton] Button 컴포넌트를 찾을 수 없습니다.");
            return;
        }

        // 버튼 클릭 이벤트 등록
        button.onClick.AddListener(OnInfiniteButtonClick);

        // 생명력이 0 미만일 경우 버튼 비활성화
        if (LifeManager.Instance != null && LifeManager.Instance.Life < 0)
        {
            button.interactable = false;
        }

        if (tooltipUI != null)
        {
            HideTooltip();
        }
    }

    /// <summary>
    /// 무한모드 버튼 클릭 시 호출되는 메서드
    /// 무한모드 카운트 증가 후 현재 스테이지 재시작
    /// </summary>
    private void OnInfiniteButtonClick()
    {
        if (StageSelectManager.Instance == null)
        {
            Debug.LogWarning("[InfiniteButton] StageSelectManager Instance를 찾을 수 없습니다.");
            return;
        }
    }

    /// <summary>
    /// 마우스 호버 시 툴팁 표시
    /// </summary>
    /// <param name="eventData">포인터 이벤트 데이터</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipUI != null)
        {
            ShowTooltip();
        }
    }

    /// <summary>
    /// 마우스 호버 종료 시 툴팁 숨김
    /// </summary>
    /// <param name="eventData">포인터 이벤트 데이터</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipUI != null)
        {
            HideTooltip();
        }
    }

    /// <summary>
    /// 툴팁 UI 표시 및 내용 업데이트
    /// </summary>
    private void ShowTooltip()
    {
        tooltipUI.SetActive(true);
        LocalizedString InfiniteModeText = new LocalizedString("EpicProject_Table", "UI_Text_InfinityText");
        // 툴팁 텍스트 업데이트
        TextMeshProUGUI tooltipText = tooltipUI.GetComponentInChildren<TextMeshProUGUI>();
        if (tooltipText != null && StageSelectManager.Instance != null)
        {
            InfiniteModeText.StringChanged += (text) =>
            {
                tooltipText.text = text;
            };
        }
    }

    private void InfiniteModeText_StringChanged(string value)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// 툴팁 UI 숨김
    /// </summary>
    private void HideTooltip()
    {
        tooltipUI.SetActive(false);
    }

    /// <summary>
    /// 컴포넌트 파괴 시 이벤트 정리
    /// </summary>
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnInfiniteButtonClick);
        }
    }
}