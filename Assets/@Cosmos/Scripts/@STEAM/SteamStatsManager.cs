using UnityEngine;
using Steamworks;

public class SteamStatsManager : Singleton<SteamStatsManager>
{
    private const string TotalPlayTimeSeconds = "player_playtime_seconds";
    private const string MaxPlayTimeSingleSession = "max_playtime_single_session_seconds";
    private const string TotalPlaySessions = "total_play_sessions";

    [SerializeField]
    private float playtimeTracker = 0f;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (!SteamManager.Initialized) {return;}
        
        playtimeTracker += Time.deltaTime;
        if (playtimeTracker >= 60f)
        {
            // 60초마다 플레이 시간 업데이트
            playtimeTracker -= 60f;
            UpdateTotalPlayTime(60);
        }
    }
    
    
    // --- 통계 관련 메서드 ---
    public void UpdateTotalPlayTime(int secondsToAdd)
    {
        SteamUserStats.GetStat(TotalPlayTimeSeconds, out int currentPlayTime);
        SteamUserStats.SetStat(TotalPlayTimeSeconds, currentPlayTime + secondsToAdd);
        //Debug.Log(TotalPlayTimeSeconds);
    }
    
    
    // --- 서버 전송 메서드 ---
    public void UploadStatsToServer()
    {
        if (!SteamManager.Initialized) { return; }
        
        SteamUserStats.StoreStats();
        Debug.Log("플레이 통계가 서버에 업로드되었습니다.");
    }
}
