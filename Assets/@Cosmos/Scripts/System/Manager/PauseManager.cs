using UnityEngine;

/// <summary>
/// 게임 일시정지 상태를 관리하는 매니저
/// 설정창으로 인한 일시정지 상태만 추적
/// </summary>
public class PauseManager : Singleton<PauseManager>
{
    private bool isSettingPaused = false;
    
    /// <summary>
    /// 설정창으로 인한 일시정지 여부
    /// </summary>
    public bool IsSettingPaused => isSettingPaused;
    
    /// <summary>
    /// 설정창 일시정지 상태 설정
    /// </summary>
    /// <param name="paused">일시정지 여부</param>
    public void SetSettingPaused(bool paused)
    {
        isSettingPaused = paused;
    }
}