using System;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Analytics;
using Newtonsoft.Json;
using System.Collections.Generic;
using Unity.Services.Core.Environments;

// ✅ 이 스크립트는 Unity Analytics를 사용하여 게임 이벤트를 기록하는 매니저입니다.
// ✅ 이 스크립트는 싱글턴 패턴을 사용하여 게임 전역에서 접근할 수 있는 AnalyticsManager를 구현합니다.

/// <summary>
/// Unity Analytics를 사용하여 게임 이벤트를 기록하는 매니저
/// 보스 패턴 로그 시스템과 연동
/// </summary>
public class AnalyticsManager : Singleton<AnalyticsManager>
{
    private bool _isInitialized = false;
    public bool isAgreed = false; // 데이터 수집 동의 여부


    protected override void Awake()
    {
        base.Awake();
        EventBus.SubscribePlayerDeath(StageFailEvent);
    }
    private void Start()
    {
        isAgreed = SaveManager.IsDataAgreement;
        if(!isAgreed) return;
        CollectStart();
    }


    public async void CollectStart()
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
            Debug.LogError($"--- UGS 테스트 실패 ---");
            Debug.LogError(e.ToString());
        }
    }

    private bool IsInit()
    {
        if (!isAgreed)
        {
            Debug.LogWarning("동의하지 않음 이벤트 전송 불가.");
            return false;
        }
        if (!_isInitialized)
        {
            Debug.LogError("아직 초기화되지 않음! 이벤트 전송 불가."); 
            return false;
        }
        return true;
    }

    /// <summary>
    /// 스테이지 클리어 이벤트 (보스 패턴 로그 포함)
    /// </summary>
    public void StageClearEvent()
    {
        if(!IsInit()) return;
        
        // 1. 필요한 정보를 수집합니다.
        int stageIndex = GameManager.Instance.LogHandler.GetStageIndex();
        int difficulty = GameManager.Instance.DifficultyLevel;
        float stageTime = GameManager.Instance.LogHandler.GetStageTimer();
        string itemsJson = GameManager.Instance.LogHandler.GetPlacedTileCountJson();
        
        // 보스 패턴 관련 데이터 수집
        string hitPatterns = BossPatternLogger.Instance.GetPatternHitJson();
        string killerPattern = "None"; // 클리어했으므로 킬러 패턴 없음
        
        //int damageTaken = 0;
        int healingReceived = GameManager.Instance.LogHandler.GetHealedAmount();
        int protectedDamage = GameManager.Instance.LogHandler.GetProtectionAmount();
        
        // 2. 'stage_clear' 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent stageClearEvent = new CustomEvent("stage_clear")
        {
            { "stage_index", stageIndex },
            { "difficulty", difficulty },
            { "stage_time", stageTime },
            { "used_tiles", itemsJson },
            //{ "damage_taken", damageTaken },
            { "healing_received", healingReceived },
            { "protected_damage", protectedDamage },
            { "hit_patterns", hitPatterns }, 
            { "killer_pattern", killerPattern } // 추가
        };

        // 3. 이벤트를 기록하고 전송합니다.
        AnalyticsService.Instance.RecordEvent(stageClearEvent);
        
        Debug.Log($" 이벤트 전송 성공 : StageClearEvent");
        Debug.Log($"Hit Patterns: {hitPatterns}");
        //Debug.Log(itemsJson);
        
        // 스테이지 클리어 후 보스 패턴 로그 초기화
        BossPatternLogger.Instance.ClearLogs();
        
        //AnalyticsService.Instance.Flush();
    }

    /// <summary>
    /// 스테이지 실패 이벤트 (보스 패턴 로그 포함)
    /// </summary>
    public void StageFailEvent()
    {
        if(!IsInit()) return;
        
        // 1. 필요한 정보를 수집합니다.
        int stageIndex = GameManager.Instance.LogHandler.GetStageIndex();
        int difficulty = GameManager.Instance.DifficultyLevel;
        float stageTime = GameManager.Instance.LogHandler.GetStageTimer();
        string itemsJson = GameManager.Instance.LogHandler.GetPlacedTileCountJson();
        
        // 보스 패턴 관련 데이터 수집
        string hitPatterns = BossPatternLogger.Instance.GetPatternHitJson();
        string killerPattern = BossPatternLogger.Instance.GetKillerPattern();

        //int damageTaken = 0;
        int healingReceived = GameManager.Instance.LogHandler.GetHealedAmount();
        int protectedDamage = GameManager.Instance.LogHandler.GetProtectionAmount();

        // 2. 'stage_fail' 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent stageFailEvent = new CustomEvent("stage_fail")
        {
            { "stage_index", stageIndex },
            { "difficulty", difficulty },
            { "stage_time", stageTime },
            { "used_tiles", itemsJson },
            //{ "damage_taken", damageTaken },
            { "healing_received", healingReceived },
            { "protected_damage", protectedDamage },
            { "hit_patterns", hitPatterns },
            { "killer_pattern", killerPattern } // 추가
        };

        // 3. 이벤트를 기록하고 전송합니다.
        AnalyticsService.Instance.RecordEvent(stageFailEvent);
        
        Debug.Log($"이벤트 전송 성공 : stageFailEvent");
        Debug.Log($"Hit Patterns: {hitPatterns}");
        Debug.Log($"Killer Pattern: {killerPattern}");
        //Debug.Log(itemsJson);
        
        // 스테이지 실패 후 보스 패턴 로그 초기화
        BossPatternLogger.Instance.ClearLogs();
    }
    
    /// <summary>
    /// 세션 클리어 이벤트
    /// </summary>
    public void SessionClearEvent()
    {
        if(!IsInit()) return;
        
        // 1. 필요한 정보를 수집합니다.
        int difficulty = GameManager.Instance.DifficultyLevel;
        float sessionTime = GameManager.Instance.LogHandler.GetSessionPlayTimer();
        
        // 2. 'session_clear' 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent sessionClearEvent = new CustomEvent("session_clear")
        {
            { "difficulty", difficulty },
            { "session_time", sessionTime },
        };
        
        AnalyticsService.Instance.RecordEvent(sessionClearEvent);
    }
    
    /// <summary>
    /// 전투씬 진입(덱 구성 완료) 이벤트
    /// </summary>
    public void BuildingCompleteEvent()
    {
        if(!IsInit()) return;
        
        // 1. 필요한 정보를 수집합니다.
        int stageIndex = GameManager.Instance.LogHandler.GetStageIndex();
        string purchasedTiles = "";
        string placedTiles = "";
        string soldTiles = "";
        int lockCount = -1;
        int rerollCount = -1;
        int enforcedTilesCount = -1;
        int totalStarCount = -1;
        int activatedStarCount = -1;
        
        // 2. 'building_complete' 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent buildingCompleteEvent = new CustomEvent("building_complete")
        {
            { "stage_index", stageIndex },
            { "purchased_tiles", purchasedTiles},
            { "placed_tiles", placedTiles},
            { "sold_tiles", soldTiles},
            { "lock_count", lockCount},
            { "reroll_count", rerollCount},
            { "enforced_tiles_count", enforcedTilesCount},
            { "total_star_count", totalStarCount},
            { "activated_star_count", activatedStarCount},
        };
        
        AnalyticsService.Instance.RecordEvent(buildingCompleteEvent);
    }
    
    /// <summary>
    /// 게임 종료 이벤트
    /// </summary>
    public void GameExitEvent()
    {
        if(!IsInit()) return;
        
        // 1. 필요한 정보를 수집합니다.
        int stageIndex = GameManager.Instance.LogHandler.GetStageIndex();
        string gameState = GameStateManager.Instance.CurrentState.ToString();
        
        // 2.이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent gameExitEvent = new CustomEvent("game_exit")
        {
            { "stage_index", stageIndex },
            { "game_state", gameState },
        };
        
        AnalyticsService.Instance.RecordEvent(gameExitEvent);
    }

    /// <summary>
    /// 타이틀 버튼 누를때 이벤트
    /// </summary>
    public void GoTitleEvent() 
    {
        if(!IsInit()) return;
        
        int difficulty = GameManager.Instance.DifficultyLevel;
        int stageIndex = GameManager.Instance.LogHandler.GetStageIndex();
        string gameState = GameStateManager.Instance.CurrentState.ToString();
        float sessionTime = GameManager.Instance.LogHandler.GetSessionPlayTimer();
        
        string itemsJson = JsonConvert.SerializeObject(GridManager.Instance.GetPlacedTileCount());
        
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

    /// <summary>
    /// 튜토리얼 프롬프트 응답 이벤트
    /// </summary>
    public void TutorialPromptResponseEvent(string response)
    {
        if(!IsInit()) return;

        string initialChoice = response;
        
        // 2. 이벤트를 생성하고 파라미터를 담습니다.
        CustomEvent tutorialPromptResponse = new CustomEvent("tutorial_prompt_response")
        {
            { "initial_choice", initialChoice }
        };
        AnalyticsService.Instance.RecordEvent(tutorialPromptResponse);
    }
    
    
    private void OnDestroy()
    {
        EventBus.UnsubscribePlayerDeath(StageFailEvent);
        
    }
}