using System;
using UnityEngine;
using Steamworks;

public class GlobalStatsViewer : MonoBehaviour
{
    // 스팀 API 호출 결과를 처리하기 위한 CallResult 객체
    private CallResult<GlobalStatsReceived_t> m_GlobalStatsReceived;

    // 통계 API 이름을 상수로 관리
    private const string GlobalPlaytimeStatName = "player_playtime_seconds";

    private void OnEnable()
    {
        if (!SteamManager.Initialized) { return; }

        // CallResult를 초기화하고, 콜백이 오면 OnGlobalStatsReceived 함수를 실행하도록 연결
        m_GlobalStatsReceived = CallResult<GlobalStatsReceived_t>.Create(OnGlobalStatsReceived);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
            FetchGlobalPlaytime();
    }

    // 전 세계 총 플레이 시간을 요청하는 메서드 (예: 버튼 클릭 시 호출)
    public void FetchGlobalPlaytime()
    {
        if (!SteamManager.Initialized)
        {
            Debug.LogError("스팀이 초기화되지 않았습니다.");
            return;
        }

        Debug.Log("전 세계 총 플레이 시간 통계를 스팀 서버에 요청합니다...");
        
        // 1. 요청: 스팀 서버에 글로벌 통계를 요청합니다.
        SteamAPICall_t handle = SteamUserStats.RequestGlobalStats(1); // 1일치 이력을 요청 (현재 총합만 필요해도 이력 요청이 필요)

        // 요청 핸들을 CallResult에 등록하여 콜백을 기다립니다.
        m_GlobalStatsReceived.Set(handle);
    }

    // 2. 신호 (콜백): 스팀 서버가 데이터 준비를 마쳤을 때 자동으로 호출되는 메서드
    private void OnGlobalStatsReceived(GlobalStatsReceived_t callback, bool bIOFailure)
    {
        if (bIOFailure || callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("전 세계 통계 수신에 실패했습니다.");
            return;
        }

        Debug.Log("전 세계 통계 데이터를 성공적으로 수신했습니다!");

        // 3. 데이터 가져오기: GetGlobalStat을 호출하여 최종 값을 가져옵니다.
        // 데이터 타입은 long (64비트 정수) 입니다.
        bool success = SteamUserStats.GetGlobalStat(GlobalPlaytimeStatName, out long totalPlaytime);

        if (success)
        {
            // 초 단위를 시간, 분, 초로 변환하여 보기 좋게 출력
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(totalPlaytime);
            string formattedTime = string.Format("{0:D2}시간 {1:D2}분 {2:D2}초", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            
            Debug.Log($"[전 세계 총 플레이 시간]: {totalPlaytime}초 ({formattedTime})");
        }
        else
        {
            Debug.LogError($"'{GlobalPlaytimeStatName}' 통계를 가져오는 데 실패했습니다. API 이름이 정확한지 확인하세요.");
        }
    }
}