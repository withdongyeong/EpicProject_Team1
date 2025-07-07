using UnityEngine;
using System.IO;

[System.Serializable]
public class TutorialData
{
    public bool isCompleted;
}

public class TESTSTAGESELECTBTN : MonoBehaviour
{
    private string filePath;

    void Awake()
    {
        // JSON 파일 경로 설정
        filePath = Path.Combine(Application.persistentDataPath, "tutorialData.json");
    }

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

    //가이드 전투씬으로 갑니다.
    public void GoCreditsScene()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");

        StageSelectManager.Instance.StageSet("Credits");
        SceneLoader.LoadCredits();
    }

    public void OpenTutoChoicePanel(GameObject gameObject)
    {
        if(IsTutorialCompleted())
        {
            // 튜토리얼이 완료된 경우, 바로 빌딩 씬으로 이동
            GOBUILDSCENE();
            return;
        }
        gameObject.SetActive(true);
        CompleteTutorial(); // 튜토리얼 완료 표시
    }

    // 튜토리얼 완료 시 호출
    private void CompleteTutorial()
    {
        TutorialData data = new TutorialData { isCompleted = true };
        string json = JsonUtility.ToJson(data, true); // true로 예쁘게 포맷
        File.WriteAllText(filePath, json);
        Debug.Log("튜토리얼 완료 상태 저장됨: " + filePath);
    }

    // 튜토리얼 완료 여부 확인
    private bool IsTutorialCompleted()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            TutorialData data = JsonUtility.FromJson<TutorialData>(json);
            return data.isCompleted;
        }
        return false; // 파일이 없으면 미완료
    }

    // 저장된 튜토리얼 데이터 초기화
    private void ResetTutorial()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("튜토리얼 진행 상태 초기화됨");
        }
    }
}
