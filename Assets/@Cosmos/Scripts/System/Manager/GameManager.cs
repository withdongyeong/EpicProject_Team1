using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private bool isInTutorial = false; // 튜토리얼 진행 중 ?
    public bool IsInTutorial => isInTutorial;

    //현재 적용되고 있는 해금된 별자리들의 레벨입니다.
    private int currentUnlockLevel;

    /// <summary>
    /// 현재 적용되고 있는 해금된 별자리들의 레벨입니다. 
    /// </summary>
    public int CurrentUnlockLevel => currentUnlockLevel;

    protected override void Awake()
    {
        base.Awake();
        EventBus.Init(); // 꼭 한 번만 호출되게
        EventBus.SubscribeSceneLoaded(OnSceneLoaded);
        LoadGameData();
        currentUnlockLevel = SaveManager.UnlockLevel;
        SetResolution(SaveManager.Resolution);
    }
    
    private void Update()
    {
        ShowSetting();
        
        // Alt + Enter 감지 → 비율 깨짐 방지
        if (Input.GetKeyDown(KeyCode.Return) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }


    private void LoadGameData()
    {
        SaveManager.LoadAll();
        // 로드된 데이터를 사용하여 게임 설정을 적용합니다.
    }

    public void SetResolution(string resolution)
    {
        string resolutionStr = resolution;
        string[] res = resolution.Split('x');
        
        if (res.Length == 2 && int.TryParse(res[0], out int width) && int.TryParse(res[1], out int height))
        {
            Screen.SetResolution(width, height, SaveManager.IsFullScreen
                ? FullScreenMode.FullScreenWindow
                : FullScreenMode.Windowed);
        }
        else
        {
            Debug.LogWarning("해상도 파싱 실패: " + resolutionStr);
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow); // fallback
        }
        
        SaveManager.SaveIsFullScreen(Screen.fullScreen);
        SaveManager.SaveResolution(resolutionStr);
    }
    private void ShowSetting(){
        
        if(Input.GetKeyDown(KeyCode.Escape) && (SceneLoader.IsInBuilding() || SceneLoader.IsInTitle() || SceneLoader.IsInStage()))
        {
            SceneLoader.ToggleSetting();
        }    
    }
    //씬이 로드 될 때마다 호출됩니다.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetTutorial();
    }
    // 타이틀로 돌아갈 때 벌어지는 일들
    public void LoadTitle()
    {
        TimeScaleManager.Instance.ResetTimeScale();
        GridManager.Instance.ResetGridCompletely();
        GoldManager.Instance.SetCurrentGold(16);
        LifeManager.Instance.ResetLifeManager();
        StageSelectManager.Instance.ResetManager();
        JournalSlotManager.Instance.SetStoreTileList();
        currentUnlockLevel = SaveManager.UnlockLevel;
        for(int i =0; i<5; i++)
        {
            StoreLockManager.Instance.RemoveStoreLock(i);
        }      
        SceneLoader.LoadTitle();
    }
    //매 씬 로드마다 튜토리얼 여부를 확인합니다
    private void SetTutorial()
    {
        if (SceneLoader.IsInGuide())
        {
            isInTutorial = true;
        }
        else
        {
            isInTutorial = false;
        }
    }
    
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(OnSceneLoaded);
        SceneLoader.LoadSceneWithName("InitializeScene");
    }

}
