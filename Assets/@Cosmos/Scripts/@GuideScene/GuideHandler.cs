using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class GuideHandler : MonoBehaviour
{
    public static GuideHandler instance; //걍 싱클톤 해버릴거임 
    public List<GuideQuest> guideQuests;
    public TextMeshProUGUI questText;
    public Canvas questCanvas;
    [SerializeField]
    private int currentIndex = 0;
    public int CurrentIndex => currentIndex;
    private GuideQuest currentQuest => guideQuests[currentIndex];
    public GuideQuest CurrentQuest => currentQuest;
    
    public bool assignQuest = false; //가이드 퀘스트 완료 여부
    
    private QuestInfoPanel questInfoPanel;
    
    public Button fightButton;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        EventBus.SubscribeSceneLoaded(CheckScene);
        
        questInfoPanel = FindAnyObjectByType<QuestInfoPanel>();
    }

    private void Start()
    {
        StartCoroutine(StartQuestInit());
    }
    
    private void Update()
    {
        if (currentIndex >= guideQuests.Count || !assignQuest) return;
        if (currentQuest != null && currentQuest.IsCompleted())
        {
            CompleteCurrentQuest();
        }
    }

    private IEnumerator StartQuestInit() //최초 첫 가이드 용 
    {
        questText.rectTransform.anchoredPosition = new Vector2(-1000, 0);
        yield return new WaitForSeconds(2f);
        StartCurrentQuest();
    }
    
    private IEnumerator StartQuest() //나머지
    {
        HideText();
        yield return new WaitForSeconds(1.5f);
        StartCurrentQuest();
    }

    public void ShowText()
    {
        questText.rectTransform.anchoredPosition = new Vector2(-1000, -50);
        //questText.text = currentQuest.instructionText;
        questText.rectTransform.DOAnchorPos(new Vector2(80,-50), 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutBack)
            .OnComplete(() => {
                questText.rectTransform.anchoredPosition = new Vector2(80, -50);
                assignQuest = true; //가이드 퀘스트 활성화
            });
    }
    
    private void HideText()
    {
        questText.rectTransform.anchoredPosition = new Vector2(80, -50);
        questText.rectTransform.DOAnchorPos(new Vector2(-1000, -50), 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InBack)
            .OnComplete(() => {
                questText.rectTransform.anchoredPosition = new Vector2(-1000, -50);
                questText.text = currentQuest.instructionText;
            });
    }
    

    private void StartCurrentQuest()
    {
        if (currentQuest == null) return;
        Debug.Log($"[튜토리얼] 시작: {currentQuest.instructionText}");
        currentQuest.OnStart();
        questInfoPanel.ShowPanel(currentQuest);
        //questText.text = currentQuest.instructionText;
    }

    private void CompleteCurrentQuest()
    {
        assignQuest = false; //가이드 퀘스트 활성화
        Debug.Log($"[튜토리얼] 완료: {currentQuest.instructionText}");
        currentQuest.OnComplete();
        currentIndex++;
        
        if (currentIndex < guideQuests.Count)
        {
            StartCoroutine(StartQuest());
        }
        else
        {
            Debug.Log("[튜토리얼] 모든 튜토리얼 완료!");
        }
    }
    

    private void CheckScene(Scene scene, LoadSceneMode mode)
    {
        if (SceneLoader.IsCurrentScene("GuideBuildingScene"))
        {
            fightButton = GameObject.Find("StageBtn").GetComponent<Button>();
            if (currentQuest.GetType() != typeof(FightQuest))
            {
                fightButton.interactable = false;
            }
        }
        
        if (!SceneLoader.IsCurrentScene("GuideStageScene") && !SceneLoader.IsCurrentScene("GuideBuildingScene"))
        {
            Destroy(gameObject);
        }
        
    }
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(CheckScene);
    }
}
