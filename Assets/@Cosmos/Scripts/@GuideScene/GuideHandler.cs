using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class GuideHandler : MonoBehaviour
{
    public static GuideHandler instance; //걍 싱클톤 해버릴거임 
    public List<GuideQuest> guideQuests;
    public TextMeshProUGUI questText;
    [SerializeField]
    private int currentIndex = 0;
    public int CurrentIndex => currentIndex;
    private GuideQuest currentQuest => guideQuests[currentIndex];
    public GuideQuest CurrentQuest => currentQuest;

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
    }

    private void Start()
    {
        StartCurrentQuest();
        
    }

    private void Update()
    {
        if (currentIndex >= guideQuests.Count) return;
        if (currentQuest != null && currentQuest.IsCompleted())
        {
            CompleteCurrentQuest();
        }
    }

    private void StartCurrentQuest()
    {
        if (currentQuest == null) return;
        Debug.Log($"[튜토리얼] 시작: {currentQuest.instructionText}");
        currentQuest.OnStart();
        questText.text = currentQuest.instructionText;
    }

    private void CompleteCurrentQuest()
    {
        Debug.Log($"[튜토리얼] 완료: {currentQuest.instructionText}");
        currentQuest.OnComplete();
        currentIndex++;

        if (currentIndex < guideQuests.Count)
        {
            StartCurrentQuest();
        }
        else
        {
            Debug.Log("[튜토리얼] 모든 튜토리얼 완료!");
        }
    }

    private void CheckScene(Scene scene, LoadSceneMode mode)
    {
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
