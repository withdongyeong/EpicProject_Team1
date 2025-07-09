using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private bool isInTutorial = false; // 튜토리얼 진행 중 ?
    public bool IsInTutorial => isInTutorial;


    protected override void Awake()
    {
        base.Awake();
        EventBus.Init(); // 꼭 한 번만 호출되게
        EventBus.SubscribeSceneLoaded(OnSceneLoaded);
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow); // 혹은 FullScreen
    }

    private void Update()
    {
        //테스트 코드 업적 초기화
        if (Input.GetKeyDown(KeyCode.P)) {
            SteamUserStats.ResetAllStats(true);
            SteamUserStats.StoreStats();
        }

        ShowSetting();
        
        // Alt + Enter 감지 → 비율 깨짐 방지
        if (Input.GetKeyDown(KeyCode.Return) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }


    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(OnSceneLoaded);
        SceneLoader.LoadSceneWithName("InitializeScene");
    }
    
    private void ShowSetting(){
        if(Input.GetKeyDown(KeyCode.Escape) && (SceneLoader.IsInBuilding() || SceneLoader.IsInTitle()))
        {
            SceneLoader.ToggleSetting();
        }    
    }
      

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetTutorial();
    }

    private void ResetGame()
    {
        EventBus.Reset();
        Destroy(SoundManager.Instance.gameObject);
        Destroy(DragManager.Instance.gameObject);
        Destroy(GridManager.Instance.gameObject);
        Destroy(GameStateManager.Instance.gameObject);
        Destroy(TimeScaleManager.Instance.gameObject);
        Destroy(GoldManager.Instance.gameObject);
        Destroy(StageSelectManager.Instance.gameObject);
        Destroy(GlobalSetting.Instance.gameObject);
        Destroy(Instance.gameObject);
    }


    public void LoadTitle()
    {
        TimeScaleManager.Instance.ResetTimeScale();
        GridManager.Instance.ResetGridCompletely();
        GoldManager.Instance.SetCurrentGold(16);
        LifeManager.Instance.ResetLifeManager();
        StageSelectManager.Instance.ResetManager();
        for(int i =0; i<5; i++)
        {
            StoreLockManager.Instance.RemoveStoreLock(i);
        }      
        SceneLoader.LoadTitle();
    }
    public void SetTutorial()
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

}
