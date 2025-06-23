using Unity.VisualScripting;
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
    
    
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(OnSceneLoaded);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("씬 로드됨: " + scene.name);
    }

}
