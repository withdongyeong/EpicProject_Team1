using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class QuestInfoPanel : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    public Image blackScreen;
    public RectTransform panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subTitleText;
    public TextMeshProUGUI contentText;
    public TextMeshProUGUI questGoalText;
    public Button closeButton;
    
    private RectTransform _rectTransform;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        closeButton.onClick.AddListener(HidePanel);
        _rectTransform = panel.GetComponent<RectTransform>();
    }

    private void Start()
    {
        OffCanvas();
    }


    private void OnCanvas()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
    private void OffCanvas()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
    
    public void ShowPanel(GuideQuest quest)
    {
        TimeScaleManager.Instance.StopTimeScale();
        OnCanvas();
        titleText.text = quest.titleText;
        subTitleText.text = quest.subTitleText;
        contentText.text = quest.contentText;
        questGoalText.text = quest.goalText;
        
        blackScreen.color = new Color(0f, 0f, 0f, 0f);
        blackScreen.DOFade( 0.97f, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InOutQuad);
        
        
        _rectTransform.anchoredPosition = new Vector2(0, -1000); // 화면 밖으로 이동
        _rectTransform.DOAnchorPos(new Vector2(0, 0), 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                panel.anchoredPosition = new Vector2(0, 0);
                Debug.Log("[튜토리얼] 패널 활성화: " + quest.titleText);
            });
        // 블랙 스크린 활성화
        blackScreen.gameObject.SetActive(true);
    }

   
    
    public void HidePanel()
    {
        TimeScaleManager.Instance.ResetTimeScale();
        if (GuideHandler.instance.CurrentQuest.GetType() == typeof(PlaceQuest))
        {
            SceneLoader.LoadGuideBuilding();
            // 게임 격자 다시 상점 자리로 원위치
            GridManager.Instance.transform.position = new Vector3(0, 0, 0);
        }
        blackScreen.color = new Color(0f, 0f, 0f, 0.97f);
        blackScreen.DOFade( 0f, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InOutQuad);
        
        _rectTransform.anchoredPosition = new Vector2(0, 0); // 화면 밖으로 이동
        _rectTransform.DOAnchorPos(new Vector2(0, -1000), 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                panel.anchoredPosition = new Vector2(0, -1000);
                OffCanvas();
                // 블랙 스크린 비활성화
                blackScreen.gameObject.SetActive(false);
                GuideHandler.instance.ShowText();
            });
    }
}
