using UnityEngine;
using System.IO;


public class TESTSTAGESELECTBTN : MonoBehaviour
{
    

    //빌딩씬에서 쓰는 스테이지 선택 버튼
    public void OnClick()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");

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

        GameManager.Instance.LoadTitle();
    }

    
    //가이드 전투씬으로 갑니다.
    public void GOTUTO()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");

        StageSelectManager.Instance.StageSet("Guide");
        SceneLoader.LoadGuideStage();
    }

    
    public void GoCreditsScene()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");

        StageSelectManager.Instance.StageSet("Credits");
        SceneLoader.LoadCredits();
    }

    public void OpenTutoChoicePanel(GameObject gameObject)
    {
        if(SaveManager.IsTutorialCompleted == 1)
        {
            // 튜토리얼이 완료된 경우, 바로 빌딩 씬으로 이동
            GOBUILDSCENE();
            return;
        }
        gameObject.SetActive(true);
        SaveManager.SaveIsTutorialCompleted(1);
    }
}
