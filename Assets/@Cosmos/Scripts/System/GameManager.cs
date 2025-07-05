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
        Screen.SetResolution(1600, 900, FullScreenMode.Windowed); // 혹은 FullScreen
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F10))
        {
            ResetGame();
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
