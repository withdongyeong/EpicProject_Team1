using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        EventBus.Init(); // 꼭 한 번만 호출되게
        EventBus.SubscribeSceneLoaded(OnSceneLoaded);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F10))
        {
            ResetGame();
        }
    }


    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(OnSceneLoaded);
        SceneLoader.LoadSceneWithName("InitializeScene");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
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

}
