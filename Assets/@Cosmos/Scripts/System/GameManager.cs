using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    //곽민준이 건들였습니다
    private int stageNum = 1;

    protected override void Awake()
    {
        base.Awake();
        stageNum = 1;
        EventBus.Init(); // 꼭 한 번만 호출되게
        EventBus.SubscribeSceneLoaded(OnSceneLoaded);
        EventBus.SubscribeBossDeath(AddStageNum);
    }

    public int StageNum => Instance.stageNum;

    public void AddStageNum()
    {
        stageNum++;
    }
    
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(OnSceneLoaded);
        EventBus.UnsubscribeBossDeath(AddStageNum);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("씬 로드됨: " + scene.name);
    }

}
