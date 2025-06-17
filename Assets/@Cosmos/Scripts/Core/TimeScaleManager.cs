using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬 변경 시 Time.timeScale 관리
/// </summary>
public class TimeScaleManager : Singleton<TimeScaleManager>
{
    private static TimeScaleManager _instance;
    
    protected override void Awake()
    {
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    /// <summary>
    /// 씬 전환시 타임 스케일 초기화 안전장치
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1.0f;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void StopTimeScale()
    {
        Time.timeScale = 0f;
    }
    /// <summary>
    /// 타임스케일 강제 초기화
    /// </summary>
    public void ResetTimeScale()
    {
        Time.timeScale = 1.0f;
    }
}