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
    public bool noAnalytics = false; // 테스트용, 실제 배포시 false로 설정해야 합니다.
    
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
        if (noAnalytics) return false; // 테스트용, 실제 배포시 false로 설정해야 합니다.
        if (!_isInitialized)
        {
            Debug.LogError("아직 초기화되지 않음! 이벤트 전송 불가."); 
            return false;
        }
        return true;
    }


    
    
    public void StageClearEvent()
    {
        if(!IsInit()) return;
        
        // 1. 필요한 정보를 수집합니다.
        
        int stageIndex =  GameManager.Instance.LogHandler.GetStageIndex();
        int difficulty = GameManager.Instance.DifficultyLevel;
        float stageTime = GameManager.Instance.LogHandler.GetStageTimer();
        string itemsJson = GameManager.Instance.LogHandler.GetPlacedTileCountJson(); // 아이템 사용 딕셔너리를 JSON 문자열로 변환합니다.
        int damageTaken = 0;
        int healingReceived = 0;
        int protectedDamage = 0;
        string hitPatterns = "null";
        
        
        // 2. 'stage_clear' 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent stageClearEvent = new CustomEvent("stage_clear")
        {
            { "stage_index", stageIndex },
            { "difficulty", difficulty },
            { "stage_time", stageTime },
            { "used_tiles",  itemsJson },
            { "damage_taken", damageTaken },
            { "healing_received", healingReceived },
            { "protected_damage", protectedDamage },
            { "hit_patterns", hitPatterns }
        };

        // 3. 이벤트를 기록하고 전송합니다.
        AnalyticsService.Instance.RecordEvent(stageClearEvent);
        //AnalyticsService.Instance.Flush();
        
        Debug.Log($" 이벤트 전송 성공 : StageClearEvent");
       //Debug.Log(itemsJson);
    }

    public void StageFailEvent()
    {
        if(!IsInit()) return;
        
        // 1. 필요한 정보를 수집합니다.
        int stageIndex =  GameManager.Instance.LogHandler.GetStageIndex();
        int difficulty = GameManager.Instance.DifficultyLevel;
        float stageTime = GameManager.Instance.LogHandler.GetStageTimer();
        string itemsJson = GameManager.Instance.LogHandler.GetPlacedTileCountJson(); // 아이템 사용 딕셔너리를 JSON 문자열로 변환합니다.
        int damageTaken = 0;
        int healingReceived = 0;
        int protectedDamage = 0;
        string hitPatterns = "null";
        
        
        // 2. 'stage_clear' 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent stageFailEvent = new CustomEvent("stage_fail")
        {
            { "stage_index", stageIndex },
            { "difficulty", difficulty },
            { "stage_time", stageTime },
            { "used_tiles",  itemsJson },
            { "damage_taken", damageTaken },
            { "healing_received", healingReceived },
            { "protected_damage", protectedDamage },
            { "hit_patterns", hitPatterns }
        };

        // 3. 이벤트를 기록하고 전송합니다.
        AnalyticsService.Instance.RecordEvent(stageFailEvent);
        //AnalyticsService.Instance.Flush();
        
        Debug.Log($"이벤트 전송 성공 : stageFailEvent");
        //Debug.Log(itemsJson);
    }

    
    // 세션 클리어 이벤트
    public void SessionClearEvent()
    {
        if(!IsInit()) return;
        
        // 1. 필요한 정보를 수집합니다.
        int difficulty = GameManager.Instance.DifficultyLevel;
        float sessionTime = GameManager.Instance.LogHandler.GetSessionPlayTimer();
        
        
        // 2. 'stage_clear' 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent sessionClearEvent = new CustomEvent("session_clear")
        {
            { "difficulty", difficulty },
            { "session_time", sessionTime },
        };
        
        AnalyticsService.Instance.RecordEvent(sessionClearEvent);
    }
    
    
    // 전투씬 진입(덱 구성 완료)
    public void BuildingCompleteEvent()
    {
        if(!IsInit()) return;
        
        // 1. 필요한 정보를 수집합니다.
        float sessionTime = GameManager.Instance.LogHandler.GetSessionPlayTimer();
        string purchasedTiles = ""; //
        string placedTiles = ""; // 
        string soldTiles = ""; //
        int lockCount = -1; // 잠긴 타일 개수
        int rerollCount = -1; // 재롤 횟수
        int enforcedTilesCount = -1; // 강화된 타일 개수 
        int totalStarCount = -1; // 총 별 개수
        int activatedStarCount = -1; // 활성화된 별 개수
        
        
        
        // 2. 'stage_clear' 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent buildingCompleteEvent = new CustomEvent("building_complete")
        {
            { "session_time", sessionTime },
            { "purchased_tiles",  purchasedTiles},
            { "placed_tiles",  placedTiles},
            { "sold_tiles",  soldTiles},
            { "lock_count",  lockCount},
            { "reroll_count",  rerollCount},
            { "enforced_tiles_count",  enforcedTilesCount},
            { "total_star_count",  totalStarCount},
            { "activated_star_count",  activatedStarCount},
        };
        
        AnalyticsService.Instance.RecordEvent(buildingCompleteEvent);
    }
    

    
    // 게임 종료 이벤트
    public void GameExitEvent()
    {
        if(!IsInit()) return;
        
        // 1. 필요한 정보를 수집합니다.
        int stageIndex =  GameManager.Instance.LogHandler.GetStageIndex();
        string gameState = GameStateManager.Instance.CurrentState.ToString();
        
        
        // 2.이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent gameExitEvent = new CustomEvent("game_exit")
        {
            { "stage_index", stageIndex },
            { "game_state", gameState },
        };
        
        AnalyticsService.Instance.RecordEvent(gameExitEvent);
    }

    
    // 타이틀 버튼 누를때
    public void GoTitleEvent() 
    {
        if(!IsInit()) return;
        int difficulty =  GameManager.Instance.DifficultyLevel;
        int stageIndex = GameManager.Instance.LogHandler.GetStageIndex(); // 현재 스테이지 인덱스
        string gameState = GameStateManager.Instance.CurrentState.ToString();
        float sessionTime = GameManager.Instance.LogHandler.GetSessionPlayTimer();
        
        string itemsJson = JsonConvert.SerializeObject(GridManager.Instance.GetPlacedTileCount()); // 아이템 사용 딕셔너리를 JSON 문자열로 변환합니다.
        // 2. 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent goTitle = new CustomEvent("go_title")
        {
            { "difficulty", difficulty },
            { "stage_index", stageIndex },
            { "game_state", gameState },
            { "session_time", sessionTime },
        };
        
        AnalyticsService.Instance.RecordEvent(goTitle);
       
    }

    public void TutorialPromptResponseEvent()
    {
        if(!IsInit()) return;
        string initialChoice = "null";
        
        // 2. 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent tutorialPromptResponse = new CustomEvent("tutorial_prompt_response")
        {
            { "initial_choice", initialChoice }
        };
        AnalyticsService.Instance.RecordEvent(tutorialPromptResponse);
    }
    
    
}