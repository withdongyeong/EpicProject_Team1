#if !(UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX || STEAMWORKS_WIN || STEAMWORKS_LIN_OSX)
#define DISABLESTEAMWORKS
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Collections;

#if !DISABLESTEAMWORKS
using Steamworks;
#endif

/// <summary>
/// 스팀 리더보드 API 통신을 담당하는 매니저 클래스
/// 각 난이도별 리더보드 데이터를 가져오고 업로드하는 기능 제공
/// </summary>
public class SteamLeaderboardManager : Singleton<SteamLeaderboardManager>
{
#if !DISABLESTEAMWORKS
    private Dictionary<DifficultyType, string> leaderboardNames;
    private Dictionary<DifficultyType, SteamLeaderboard_t> leaderboards;
    
    // 콜백 핸들러
    private CallResult<LeaderboardFindResult_t> leaderboardFindResult;
    private CallResult<LeaderboardScoresDownloaded_t> leaderboardScoresResult;
    private CallResult<LeaderboardScoreUploaded_t> leaderboardUploadResult;
    
    // 이벤트
    public System.Action<DifficultyType, LeaderboardData> OnLeaderboardLoaded;
    public System.Action<string> OnLeaderboardError;
    public System.Action OnLeaderboardLoadStart;
    
    private DifficultyType currentRequestDifficulty;
    private bool isLoading = false;
    
    public bool IsLoading { get => isLoading; }
    
    private void Awake()
    {
        base.Awake();
        InitializeLeaderboardNames();
        leaderboards = new Dictionary<DifficultyType, SteamLeaderboard_t>();
        
        leaderboardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
        leaderboardScoresResult = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);
        leaderboardUploadResult = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardScoreUploaded);
    }
    
    /// <summary>
    /// 난이도별 리더보드 이름을 초기화
    /// </summary>
    private void InitializeLeaderboardNames()
    {
        leaderboardNames = new Dictionary<DifficultyType, string>()
        {
            { DifficultyType.Easy, "InfiniteMode_Easy" },
            { DifficultyType.Normal, "InfiniteMode_Normal" },
            { DifficultyType.Hard, "InfiniteMode_Hard" },
            { DifficultyType.Hell, "InfiniteMode_Hell" }
        };
    }
    
    /// <summary>
    /// 지정된 난이도의 리더보드 데이터를 비동기로 가져옴
    /// 상위 10명 + 본인 기록(10위 밖일 경우)을 로드
    /// </summary>
    public void LoadLeaderboard(DifficultyType difficulty)
    {
        if (!SteamManager.Initialized)
        {
            OnLeaderboardError?.Invoke("스팀 연결 실패");
            return;
        }
        
        if (isLoading)
        {
            Debug.LogWarning("[SteamLeaderboardManager] 이미 로딩 중입니다.");
            return;
        }
        
        isLoading = true;
        currentRequestDifficulty = difficulty;
        OnLeaderboardLoadStart?.Invoke();
        
        // 리더보드가 캐시되어 있는지 확인
        if (leaderboards.ContainsKey(difficulty))
        {
            RequestLeaderboardScores(difficulty, leaderboards[difficulty]);
        }
        else
        {
            // 리더보드 찾기
            string leaderboardName = leaderboardNames[difficulty];
            SteamAPICall_t handle = SteamUserStats.FindLeaderboard(leaderboardName);
            leaderboardFindResult.Set(handle);
        }
    }
    
    /// <summary>
    /// 현재 플레이어의 점수를 리더보드에 업로드
    /// KeepBest 버그 회피를 위해 수동으로 기존 점수와 비교
    /// </summary>
    public void UploadScore(DifficultyType difficulty, int roundScore)
    {
        if (!SteamManager.Initialized)
        {
            OnLeaderboardError?.Invoke("스팀 연결 실패");
            return;
        }
        
        if (!leaderboards.ContainsKey(difficulty))
        {
            Debug.LogWarning($"[SteamLeaderboardManager] {difficulty} 리더보드를 찾을 수 없습니다.");
            return;
        }
        
        // 기존 점수 확인 후 업로드 결정
        StartCoroutine(CheckAndUploadScore(difficulty, roundScore));
    }
    
    /// <summary>
    /// 기존 점수 확인 후 더 높은 점수일 때만 업로드
    /// </summary>
    private IEnumerator CheckAndUploadScore(DifficultyType difficulty, int newScore)
    {
        // 기존 본인 점수 가져오기
        CallResult<LeaderboardScoresDownloaded_t> playerScoreResult = CallResult<LeaderboardScoresDownloaded_t>.Create(
            (LeaderboardScoresDownloaded_t result, bool ioFailure) =>
            {
                bool shouldUpload = true;
                int existingScore = 0;
                
                if (!ioFailure && result.m_cEntryCount > 0)
                {
                    LeaderboardEntry_t entry;
                    int[] details = new int[0];
                    
                    if (SteamUserStats.GetDownloadedLeaderboardEntry(result.m_hSteamLeaderboardEntries, 0, out entry, details, 0))
                    {
                        existingScore = entry.m_nScore;
                        if (existingScore >= newScore)
                        {
                            shouldUpload = false;
                            Debug.Log($"[SteamLeaderboardManager] 기존 점수({existingScore})가 새 점수({newScore})보다 높거나 같아서 업로드하지 않습니다.");
                        }
                    }
                }
                
                if (shouldUpload)
                {
                    Debug.Log($"[SteamLeaderboardManager] 새 점수({newScore})가 기존 점수({existingScore})보다 높아서 업로드합니다.");
                    ForceUploadScore(difficulty, newScore);
                }
            }
        );
        
        // 본인 점수만 가져오기
        SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntriesForUsers(
            leaderboards[difficulty],
            new CSteamID[] { SteamUser.GetSteamID() },
            1
        );
        
        playerScoreResult.Set(handle);
        yield return null;
    }
    
    /// <summary>
    /// 강제로 점수 업로드 (비교 완료 후)
    /// </summary>
    private void ForceUploadScore(DifficultyType difficulty, int roundScore)
    {
        SteamAPICall_t handle = SteamUserStats.UploadLeaderboardScore(
            leaderboards[difficulty], 
            ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, 
            roundScore, 
            null, 
            0
        );
        
        leaderboardUploadResult.Set(handle);
    }
    
    /// <summary>
    /// 리더보드 찾기 결과 콜백
    /// </summary>
    private void OnLeaderboardFindResult(LeaderboardFindResult_t result, bool ioFailure)
    {
        if (ioFailure)
        {
            isLoading = false;
            OnLeaderboardError?.Invoke("네트워크 연결 실패");
            return;
        }
        
        if (result.m_bLeaderboardFound == 0)
        {
            // 리더보드가 없으면 생성 시도
            Debug.Log($"[SteamLeaderboardManager] {leaderboardNames[currentRequestDifficulty]} 리더보드가 없어서 생성을 시도합니다.");
            CreateLeaderboard(currentRequestDifficulty);
            return;
        }
        
        // 리더보드 캐시에 저장
        leaderboards[currentRequestDifficulty] = result.m_hSteamLeaderboard;
        
        // 점수 요청
        RequestLeaderboardScores(currentRequestDifficulty, result.m_hSteamLeaderboard);
    }
    
    /// <summary>
    /// 새 리더보드 생성
    /// </summary>
    private void CreateLeaderboard(DifficultyType difficulty)
    {
        string leaderboardName = leaderboardNames[difficulty];
        
        // 리더보드 생성 (높은 점수가 좋음, 정수형)
        SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(
            leaderboardName,
            ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending,
            ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric
        );
        
        leaderboardFindResult.Set(handle);
        Debug.Log($"[SteamLeaderboardManager] {leaderboardName} 리더보드 생성 중...");
    }
    
    /// <summary>
    /// 리더보드 점수 요청 (상위 10명 + 본인 기록)
    /// </summary>
    private void RequestLeaderboardScores(DifficultyType difficulty, SteamLeaderboard_t leaderboard)
    {
        // 상위 10명 요청
        SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries(
            leaderboard,
            ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal,
            1, 10
        );
        
        leaderboardScoresResult.Set(handle);
    }
    
    /// <summary>
    /// 리더보드 점수 다운로드 결과 콜백
    /// </summary>
    private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t result, bool ioFailure)
    {
        isLoading = false;
        
        if (ioFailure)
        {
            OnLeaderboardError?.Invoke("네트워크 연결 실패");
            return;
        }
        
        List<LeaderboardEntry> topEntries = new List<LeaderboardEntry>();
        LeaderboardEntry playerEntry = null;
        CSteamID localPlayerID = SteamUser.GetSteamID();
        bool playerInTop10 = false;
        
        // 상위 10명 데이터 파싱
        for (int i = 0; i < result.m_cEntryCount; i++)
        {
            LeaderboardEntry_t entry;
            int[] details = new int[0];
            
            if (SteamUserStats.GetDownloadedLeaderboardEntry(result.m_hSteamLeaderboardEntries, i, out entry, details, 0))
            {
                string playerName = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);
                LeaderboardEntry leaderboardEntry = new LeaderboardEntry(
                    playerName,
                    entry.m_nScore,
                    entry.m_nGlobalRank
                );
                
                topEntries.Add(leaderboardEntry);
                
                // 본인이 상위 10명에 포함되어 있는지 확인
                if (entry.m_steamIDUser == localPlayerID)
                {
                    playerInTop10 = true;
                }
            }
        }
        
        // 항상 본인 기록을 별도로 요청 (하이라이트 처리를 위해)
        StartCoroutine(RequestPlayerEntry(currentRequestDifficulty, leaderboards[currentRequestDifficulty], topEntries, playerInTop10));
    }
    
    /// <summary>
    /// 본인 기록 별도 요청 코루틴 (하이라이트 처리용)
    /// </summary>
    private IEnumerator RequestPlayerEntry(DifficultyType difficulty, SteamLeaderboard_t leaderboard, List<LeaderboardEntry> topEntries, bool playerInTop10)
    {
        CallResult<LeaderboardScoresDownloaded_t> playerScoresResult = CallResult<LeaderboardScoresDownloaded_t>.Create(
            (LeaderboardScoresDownloaded_t result, bool ioFailure) =>
            {
                LeaderboardEntry playerEntry = null;
                bool hasPlayerEntry = false;
                
                if (!ioFailure && result.m_cEntryCount > 0)
                {
                    LeaderboardEntry_t entry;
                    int[] details = new int[0];
                    
                    if (SteamUserStats.GetDownloadedLeaderboardEntry(result.m_hSteamLeaderboardEntries, 0, out entry, details, 0))
                    {
                        string playerName = SteamFriends.GetFriendPersonaName(entry.m_steamIDUser);
                        playerEntry = new LeaderboardEntry(playerName, entry.m_nScore, entry.m_nGlobalRank);
                        hasPlayerEntry = true;
                    }
                }
                
                // 패턴 3: 상위 10명에 포함되면 별도 표시 안함, 밖에 있으면 별도 표시
                bool showSeparatePlayerEntry = hasPlayerEntry && !playerInTop10;
                LeaderboardData data = new LeaderboardData(topEntries, playerEntry, showSeparatePlayerEntry, hasPlayerEntry);
                OnLeaderboardLoaded?.Invoke(difficulty, data);
            }
        );
        
        SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntriesForUsers(
            leaderboard,
            new CSteamID[] { SteamUser.GetSteamID() },
            1
        );
        
        playerScoresResult.Set(handle);
        yield return null;
    }
    
    /// <summary>
    /// 점수 업로드 결과 콜백
    /// </summary>
    private void OnLeaderboardScoreUploaded(LeaderboardScoreUploaded_t result, bool ioFailure)
    {
        if (ioFailure)
        {
            Debug.LogWarning("[SteamLeaderboardManager] 점수 업로드 실패 - 네트워크 오류");
            return;
        }
        
        if (result.m_bSuccess == 1)
        {
            Debug.Log($"[SteamLeaderboardManager] 점수 업로드 성공!");
            Debug.Log($"업로드한 점수: {result.m_nScore}");
            Debug.Log($"이전 순위: {result.m_nGlobalRankPrevious}");
            Debug.Log($"새 순위: {result.m_nGlobalRankNew}");
            Debug.Log($"점수 변경됨: {(result.m_bScoreChanged == 1 ? "예" : "아니오")}");
        }
        else
        {
            Debug.LogWarning("[SteamLeaderboardManager] 점수 업로드 실패");
        }
    }
    
#else
    public bool IsLoading { get => false; }
    
    public void LoadLeaderboard(DifficultyType difficulty)
    {
        OnLeaderboardError?.Invoke("스팀 연결 실패");
    }
    
    public void UploadScore(DifficultyType difficulty, int roundScore)
    {
        Debug.LogWarning("[SteamLeaderboardManager] Steamworks가 비활성화되어 있습니다.");
    }
    
    public System.Action<DifficultyType, LeaderboardData> OnLeaderboardLoaded;
    public System.Action<string> OnLeaderboardError;
    public System.Action OnLeaderboardLoadStart;
#endif
}