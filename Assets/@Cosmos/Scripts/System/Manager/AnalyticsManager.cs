using System;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication; // 🔑 인증을 위해 필수!
using Unity.Services.Analytics;
using Newtonsoft.Json; // 🔑 JSON 직렬화를 위해 필요합니다.
using System.Collections.Generic;
using Unity.Services.Core.Environments;

// ✅ 이 스크립트는 Unity Analytics를 사용하여 게임 이벤트를 기록하는 매니저입니다.
// ✅ 이 스크립트는 싱글턴 패턴을 사용하여 게임 전역에서 접근할 수 있는 AnalyticsManager를 구현합니다.

public class AnalyticsManager : Singleton<AnalyticsManager>
{
    private bool _isInitialized = false;
    
    private async void Start()
    {
        Debug.Log("--- UGS 테스트 시작 ---");
        try
        {
            
            // 1. 초기화 옵션 객체를 생성합니다.
            var options = new InitializationOptions();
            //options.SetEnvironmentName("earlyaccess"); 
            options.SetEnvironmentName("development"); 
            
            await UnityServices.InitializeAsync(options);
            Debug.Log("✅ [1/2] UGS 초기화 성공!");
            
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("✅ [2/2] 익명 인증 성공!");
            }
            else
            {
                Debug.Log("✅ [2/2] 이미 로그인된 상태입니다.");
            }
            AnalyticsService.Instance.StartDataCollection();

            Debug.Log("--- 모든 절차 성공 ---");
            _isInitialized = true;
        }
        catch (System.Exception e)
        {
            // ❌ 실패 시 여기에 모든 오류가 잡힙니다.
            Debug.LogError($"--- UGS 테스트 실패 ---");
            Debug.LogError(e.ToString());
        }
    }

    private bool IsInit()
    {
        if (!_isInitialized)
        {
            Debug.LogError("아직 초기화되지 않음! 이벤트 전송 불가."); 
            return false;
        }
        return true;
    }


    
    
    public void SendStageClearEvent()
    {
        if(!IsInit()) return;
        
        // 1. 필요한 정보를 수집합니다.
        string itemsJson = JsonConvert.SerializeObject(GridManager.Instance.GetPlacedTileCount()); // 아이템 사용 딕셔너리를 JSON 문자열로 변환합니다.
        int stageIndex =  StageSelectManager.Instance.StageNum;
        float stageClearTime = GameManager.Instance.LogHandler.GetStageClearTimer();
        int difficultyLevel = GameManager.Instance.DifficultyLevel;
        
        
        // 2. 'stage_clear' 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent stageClearEvent = new CustomEvent("stage_clear")
        {
            { "difficulty_level", difficultyLevel },
            { "stage_index", stageIndex },
            { "clear_time", stageClearTime },
            { "used_items",  itemsJson}
        };

        // 3. 이벤트를 기록하고 전송합니다.
        AnalyticsService.Instance.RecordEvent(stageClearEvent);
        //AnalyticsService.Instance.Flush();
        
        Debug.Log($"'stage_clear' 이벤트 전송 성공 : StageClearEvent");
       //Debug.Log(itemsJson);
    }

    public void GoTitleEvent() // 게임 종료시 보낼 정보
    {
        if(!IsInit()) return;
        int difficultyLevel = GameManager.Instance.DifficultyLevel;
        int stageIndex = StageSelectManager.Instance.StageNum; // 현재 스테이지 인덱스
        float totalPlayTime = GameManager.Instance.LogHandler.GetTotalPlayTimer();
        
        string itemsJson = JsonConvert.SerializeObject(GridManager.Instance.GetPlacedTileCount()); // 아이템 사용 딕셔너리를 JSON 문자열로 변환합니다.
        // 2. 'stage_clear' 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent goTitle = new CustomEvent("go_title")
        {
            { "difficulty_level", difficultyLevel },
            { "stage_index", stageIndex },
            { "session_clear_time", totalPlayTime },
            { "used_items",  itemsJson}
        };
        AnalyticsService.Instance.RecordEvent(goTitle);
       //
    }
    
    
    
    
}