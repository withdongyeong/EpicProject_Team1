using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// 보스 패턴별 플레이어 피격 데이터를 추적하는 로그 시스템
/// </summary>
public class BossPatternLogger : Singleton<BossPatternLogger>
{
    /// <summary>
    /// 패턴별 피격 데이터 구조체
    /// </summary>
    [System.Serializable]
    public class BossPatternHitData
    {
        public string PatternName;     // "1_1", "1_2" 등
        public int HitCount;          // 해당 패턴으로 맞은 횟수
        
        public BossPatternHitData(string patternName)
        {
            PatternName = patternName;
            HitCount = 0;
        }
    }
    
    // 패턴별 피격 데이터 저장
    private Dictionary<string, BossPatternHitData> patternHitData;
    
    // 플레이어를 죽인 패턴
    private string killerPattern = "";
    
    /// <summary>
    /// 현재 스테이지의 보스 번호 (1부터 시작)
    /// </summary>
    public int CurrentBossNumber { get; private set; } = 1;

    protected override void Awake()
    {
        base.Awake();
        InitializeLogger();
    }

    /// <summary>
    /// 로거 초기화
    /// </summary>
    private void InitializeLogger()
    {
        patternHitData = new Dictionary<string, BossPatternHitData>();
        killerPattern = "";
        
        // 스테이지 번호로부터 보스 번호 설정
        if (StageSelectManager.Instance != null)
        {
            CurrentBossNumber = StageSelectManager.Instance.StageNum;
        }
        
        Debug.Log($"BossPatternLogger initialized for Boss {CurrentBossNumber}");
    }

    /// <summary>
    /// 플레이어 피격 로그 기록
    /// </summary>
    /// <param name="patternName">패턴명 (예: "1_1")</param>
    public void LogPlayerHit(string patternName)
    {
        if (string.IsNullOrEmpty(patternName) || patternName == "Unknown")
        {
            Debug.LogWarning($"BossPatternLogger: Invalid pattern name '{patternName}'");
            return;
        }
        
        // 패턴 데이터가 없으면 생성
        if (!patternHitData.ContainsKey(patternName))
        {
            patternHitData[patternName] = new BossPatternHitData(patternName);
        }
        
        // 피격 횟수 증가
        patternHitData[patternName].HitCount++;
        
        Debug.Log($"Pattern Hit Logged: {patternName} - Total hits: {patternHitData[patternName].HitCount}");
    }

    /// <summary>
    /// 플레이어 사망시 호출 - 마지막으로 피격당한 패턴을 킬러 패턴으로 기록
    /// </summary>
    /// <param name="killerPatternName">플레이어를 죽인 패턴명</param>
    public void LogPlayerDeath(string killerPatternName)
    {
        if (!string.IsNullOrEmpty(killerPatternName) && killerPatternName != "Unknown")
        {
            killerPattern = killerPatternName;
            Debug.Log($"Player killed by pattern: {killerPattern}");
        }
        else
        {
            killerPattern = "Unknown";
            Debug.LogWarning("Player death logged with unknown killer pattern");
        }
    }

    /// <summary>
    /// 패턴별 피격 데이터를 JSON 문자열로 반환 (Analytics용)
    /// </summary>
    /// <returns>JSON 형태의 피격 데이터</returns>
    public string GetPatternHitJson()
    {
        try
        {
            if (patternHitData.Count == 0)
            {
                return "{}";
            }
            
            // Dictionary를 List로 변환하여 JSON 직렬화
            List<BossPatternHitData> hitDataList = new List<BossPatternHitData>();
            foreach (var kvp in patternHitData)
            {
                hitDataList.Add(kvp.Value);
            }
            
            string json = JsonConvert.SerializeObject(hitDataList, Formatting.None);
            Debug.Log($"Pattern hit data JSON: {json}");
            return json;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error serializing pattern hit data: {e.Message}");
            return "{}";
        }
    }

    /// <summary>
    /// 플레이어를 죽인 패턴 반환 (Analytics용)
    /// </summary>
    /// <returns>킬러 패턴명</returns>
    public string GetKillerPattern()
    {
        return string.IsNullOrEmpty(killerPattern) ? "None" : killerPattern;
    }

    /// <summary>
    /// 현재 저장된 모든 패턴 피격 데이터 반환 (디버깅용)
    /// </summary>
    /// <returns>패턴별 피격 데이터 딕셔너리</returns>
    public Dictionary<string, BossPatternHitData> GetAllPatternData()
    {
        return new Dictionary<string, BossPatternHitData>(patternHitData);
    }

    /// <summary>
    /// 로그 데이터 초기화 (새 스테이지 시작시)
    /// </summary>
    public void ClearLogs()
    {
        patternHitData.Clear();
        killerPattern = "";
        Debug.Log("BossPatternLogger: All logs cleared");
    }

    /// <summary>
    /// 현재 보스 번호 설정 (테스트용)
    /// </summary>
    /// <param name="bossNumber">보스 번호</param>
    public void SetCurrentBossNumber(int bossNumber)
    {
        CurrentBossNumber = bossNumber;
        Debug.Log($"BossPatternLogger: Boss number set to {CurrentBossNumber}");
    }

    /// <summary>
    /// 디버그용: 현재 로그 상태 출력
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugPrintLogs()
    {
        Debug.Log("=== Boss Pattern Hit Logs ===");
        Debug.Log($"Boss Number: {CurrentBossNumber}");
        Debug.Log($"Killer Pattern: {GetKillerPattern()}");
        
        if (patternHitData.Count == 0)
        {
            Debug.Log("No pattern hit data recorded");
            return;
        }
        
        foreach (var kvp in patternHitData)
        {
            var data = kvp.Value;
            Debug.Log($"Pattern {data.PatternName}: {data.HitCount} hits");
        }
        
        Debug.Log("==============================");
    }
}