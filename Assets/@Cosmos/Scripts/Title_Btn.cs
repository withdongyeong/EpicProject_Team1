using UnityEngine;
using TMPro;
using DG.Tweening;

public class Title_Btn : MonoBehaviour
{
    public GameObject menuUI;
    
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float width = 100f;
    [SerializeField] private float height = 50f;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease easeType = Ease.OutQuad;
    
    private Vector2 originalPos;
    public RectTransform scoreText;
    private Vector2 originalScorePos;


    public TextMeshProUGUI infoText;
   
    private void Awake()
    {
        GameManager.OnGameStateChanged += ShowMenu;
        GameManager.OnGameStateChanged += ShowScoreText;
    }

    private void Start()
    {
        rectTransform = menuUI.GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;
        originalScorePos = scoreText.anchoredPosition;
        HideInfoText();
        scoreText.anchoredPosition = originalScorePos - new Vector2(0, -300f);
    }

    private void ShowMenu(bool isPlaying)
    {
        Vector2 targetPos;
        if (isPlaying)
        {
            targetPos = originalPos + new Vector2(width, height);
            
        }
        else
        {
            targetPos = originalPos;
            
        }

        // 위치 이동
        rectTransform.DOAnchorPos(targetPos, duration).SetEase(easeType);
        // 점수 위치 이동
    }
    
    private void ShowScoreText(bool isPlaying)
    {
        Vector3 targetScorePos;
        if (isPlaying)
        {
            targetScorePos = originalScorePos;
        }
        else
        {
            targetScorePos = originalScorePos - new Vector2(0, -300f);
        }
        scoreText.DOAnchorPos(targetScorePos, duration).SetEase(easeType);
    }

    public void ShowInfoText()
    {
        infoText.enabled = true;
    }
    public void HideInfoText()
    {
        infoText.enabled = false;
    }
}
