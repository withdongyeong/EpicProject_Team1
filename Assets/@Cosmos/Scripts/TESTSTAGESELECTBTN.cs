using UnityEngine;
using System.IO;
using Unity.Services.Analytics;
using Steamworks;


public class TESTSTAGESELECTBTN : MonoBehaviour
{
    
    
    //빌딩씬에서 쓰는 스테이지 선택 버튼
    public void OnClick()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");
        AnalyticsManager.Instance.BuildingCompleteEvent();
        StageSelectManager.Instance.StageSelect();
        
        
    }

  


    //타이틀씬에서 쓰는 빌딩 씬으로 이동하는 버튼
    public void GOBUILDSCENE()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");

        SceneLoader.LoadBuilding();
    }

    public void GOTITLESCENE()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");

        GameManager.Instance.LoadTitleFirst();
    }
    
    

    
    //가이드 전투씬으로 갑니다.
    public void ClickYesTuto()
    {
        AnalyticsManager.Instance.TutorialPromptResponseEvent("yes");
        GOTUTO();
    }
    
    public void ClickNoTuto()
    {
        AnalyticsManager.Instance.TutorialPromptResponseEvent("no");
    }
    
    public void GOTUTO()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");

        StageSelectManager.Instance.StageSet("Guide");
        SceneLoader.LoadGuideStage();
    }
    
    
    public void GoStageTuto()
    {
        if (GridManager.Instance.PlacedTileList.Count <= 0)
        {
            DragManager.Instance.GetComponentInChildren<PlacedHandler>().FirstPresent();
        }
        SoundManager.Instance.UISoundClip("ButtonActivate");
        StageSelectManager.Instance.StageSet("Guide");
        SceneLoader.LoadGuideStage();
    }

    public void OpenJournal()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");
        JournalSlotManager.Instance.ToggleJournal();
        
    }

    
    public void GoCreditsScene()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");

        StageSelectManager.Instance.StageSet("Credits");
        SceneLoader.LoadCredits();
    }

    public void OpenTutoChoicePanel(GameObject gameObject)
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");
        if (SaveManager.IsTutorialCompleted == 1)
        {
            // 튜토리얼이 완료된 경우, 바로 빌딩 씬으로 이동
            OpenDifficultySelectPannel();
            return;
        }
        gameObject.SetActive(true);
        SaveManager.SaveIsTutorialCompleted(1); // 튜토리얼 완료 상태로 저장
    }

    public void OpenDifficultySelectPannel()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");
        FindAnyObjectByType<DifficultySelectPannel>(FindObjectsInactive.Include).gameObject.SetActive(true);
    }

    public void OpenSettingsPanel()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");
        SceneLoader.ToggleSetting();
    }

    //게임 시작, 별자리 도감등의 버튼을 눌렀을때 떠있는 다른 패널들을 끄는 역할을 합니다
    public void DisAbleSelf(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }

    //이건 위의 DisAbleSelf로도 못끄는 도감을 끄기 위한 함수입니다
    public void DisAbleJournal()
    {
        JournalSlotManager.Instance.CloseJournal();
    }

    public void ExitGameButton()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");
        // 게임 종료
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
