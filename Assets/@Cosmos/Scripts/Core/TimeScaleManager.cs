using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 변경 시 Time.timeScale 관리
/// </summary>
public class TimeScaleManager : Singleton<TimeScaleManager>
{
    private static TimeScaleManager _instance;
    [SerializeField]
    private bool isTimeScaleStopped = false;
    public bool IsTimeScaleStopped => isTimeScaleStopped;
    
    protected override void Awake()
    {
        base.Awake();
        EventBus.SubscribeSceneLoaded(OnSceneLoaded);
    }
    
    /// <summary>
    /// 씬 전환시 타임 스케일 초기화 안전장치
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Additive 모드(설정창 등)에서는 timeScale을 건드리지 않음
        if (mode == LoadSceneMode.Single)
        {
            Time.timeScale = 1.0f;
            AudioListener.pause = false;
        }
        Time.timeScale = 1.0f;
        isTimeScaleStopped = false;
    }
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(OnSceneLoaded);
    }

    public void StopTimeScale()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;  // 오디오도 일시정지
        isTimeScaleStopped = true;
    }
    /// <summary>
    /// 타임스케일 강제 초기화
    /// </summary>
    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
        AudioListener.pause = false;  // 오디오 일시정지 해제
        isTimeScaleStopped = false;
    }
}
