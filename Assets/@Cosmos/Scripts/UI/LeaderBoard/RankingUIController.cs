using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/// <summary>
/// 타이틀 화면의 랭킹 UI를 관리하는 컨트롤러
/// 4개 난이도 버튼과 리더보드 표시를 조합하여 관리
/// </summary>
public class RankingUIController : MonoBehaviour
{
    [Header("난이도 버튼들")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button hellButton;
    
    [Header("리더보드 표시")]
    [SerializeField] private LeaderboardDisplay leaderboardDisplay;
    
    [Header("버튼 스타일링")]
    [SerializeField] private Color selectedTextColor = Color.white;
    [SerializeField] private Color normalTextColor = Color.gray;
    
    private Dictionary<DifficultyType, Button> difficultyButtons;
    private DifficultyType currentSelectedDifficulty;
    private DifficultyType defaultDifficulty;
    
    /// <summary>
    /// 컴포넌트 초기화 및 이벤트 등록
    /// </summary>
    private void Awake()
    {
        InitializeDifficultyButtons();
        SetRandomDefaultDifficulty();
        RegisterEvents();
    }
    
    /// <summary>
    /// 시작 시 기본 난이도 리더보드 로드
    /// </summary>
    private void Start()
    {
        // 스팀 매니저 초기화 확인 후 기본 난이도 로드
        if (SteamLeaderboardManager.Instance != null)
        {
            LoadLeaderboard(defaultDifficulty);
        }
        else
        {
            Debug.LogWarning("[RankingUIController] SteamLeaderboardManager가 초기화되지 않았습니다.");
        }
    }
    
    /// <summary>
    /// 난이도 버튼 딕셔너리 초기화
    /// </summary>
    private void InitializeDifficultyButtons()
    {
        difficultyButtons = new Dictionary<DifficultyType, Button>()
        {
            { DifficultyType.Easy, easyButton },
            { DifficultyType.Normal, normalButton },
            { DifficultyType.Hard, hardButton },
            { DifficultyType.Hell, hellButton }
        };
        
        // 버튼 클릭 이벤트 등록
        if (easyButton != null)
            easyButton.onClick.AddListener(() => OnDifficultyButtonClicked(DifficultyType.Easy));
        if (normalButton != null)
            normalButton.onClick.AddListener(() => OnDifficultyButtonClicked(DifficultyType.Normal));
        if (hardButton != null)
            hardButton.onClick.AddListener(() => OnDifficultyButtonClicked(DifficultyType.Hard));
        if (hellButton != null)
            hellButton.onClick.AddListener(() => OnDifficultyButtonClicked(DifficultyType.Hell));
    }
    
    /// <summary>
    /// 랜덤 기본 난이도 설정
    /// </summary>
    private void SetRandomDefaultDifficulty()
    {
        System.Array difficultyValues = System.Enum.GetValues(typeof(DifficultyType));
        int randomIndex = Random.Range(0, difficultyValues.Length);
        defaultDifficulty = (DifficultyType)difficultyValues.GetValue(randomIndex);
        
        Debug.Log($"[RankingUIController] 기본 난이도로 {defaultDifficulty} 설정됨");
    }
    
    /// <summary>
    /// 스팀 리더보드 매니저 이벤트 등록
    /// </summary>
    private void RegisterEvents()
    {
        if (SteamLeaderboardManager.Instance != null)
        {
            SteamLeaderboardManager.Instance.OnLeaderboardLoaded += OnLeaderboardLoaded;
            SteamLeaderboardManager.Instance.OnLeaderboardError += OnLeaderboardError;
            SteamLeaderboardManager.Instance.OnLeaderboardLoadStart += OnLeaderboardLoadStart;
        }
    }
    
    /// <summary>
    /// 스팀 리더보드 매니저 이벤트 해제
    /// </summary>
    private void UnregisterEvents()
    {
        if (SteamLeaderboardManager.Instance != null)
        {
            SteamLeaderboardManager.Instance.OnLeaderboardLoaded -= OnLeaderboardLoaded;
            SteamLeaderboardManager.Instance.OnLeaderboardError -= OnLeaderboardError;
            SteamLeaderboardManager.Instance.OnLeaderboardLoadStart -= OnLeaderboardLoadStart;
        }
    }
    
    /// <summary>
    /// 난이도 버튼 클릭 이벤트 처리
    /// </summary>
    /// <param name="difficulty">클릭된 난이도</param>
    private void OnDifficultyButtonClicked(DifficultyType difficulty)
    {
        if (SteamLeaderboardManager.Instance.IsLoading)
        {
            Debug.Log("[RankingUIController] 이미 로딩 중입니다.");
            return;
        }
        
        LoadLeaderboard(difficulty);
    }
    
    /// <summary>
    /// 지정된 난이도의 리더보드 로드
    /// </summary>
    /// <param name="difficulty">로드할 난이도</param>
    private void LoadLeaderboard(DifficultyType difficulty)
    {
        currentSelectedDifficulty = difficulty;
        UpdateButtonSelection(difficulty);
        
        if (SteamLeaderboardManager.Instance != null)
        {
            SteamLeaderboardManager.Instance.LoadLeaderboard(difficulty);
        }
    }
    
    /// <summary>
    /// 선택된 버튼 텍스트 색상 업데이트
    /// </summary>
    /// <param name="selectedDifficulty">선택된 난이도</param>
    private void UpdateButtonSelection(DifficultyType selectedDifficulty)
    {
        foreach (var kvp in difficultyButtons)
        {
            if (kvp.Value != null)
            {
                TextMeshProUGUI buttonText = kvp.Value.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.color = (kvp.Key == selectedDifficulty) ? selectedTextColor : normalTextColor;
                }
            }
        }
    }
    
    /// <summary>
    /// 리더보드 로드 시작 이벤트 콜백
    /// </summary>
    private void OnLeaderboardLoadStart()
    {
        if (leaderboardDisplay != null)
        {
            leaderboardDisplay.ShowLoadingState(true);
        }
        
        Debug.Log($"[RankingUIController] {currentSelectedDifficulty} 리더보드 로딩 시작");
    }
    
    /// <summary>
    /// 리더보드 로드 완료 이벤트 콜백
    /// </summary>
    /// <param name="difficulty">로드된 난이도</param>
    /// <param name="data">리더보드 데이터</param>
    private void OnLeaderboardLoaded(DifficultyType difficulty, LeaderboardData data)
    {
        // 현재 선택된 난이도와 일치하는지 확인
        if (difficulty != currentSelectedDifficulty)
        {
            Debug.LogWarning($"[RankingUIController] 로드된 난이도({difficulty})와 현재 선택된 난이도({currentSelectedDifficulty})가 다릅니다.");
            return;
        }
        
        if (leaderboardDisplay != null)
        {
            leaderboardDisplay.DisplayLeaderboard(data, difficulty);
        }
        
        Debug.Log($"[RankingUIController] {difficulty} 리더보드 로드 완료 (상위 {data.TopEntries.Count}명)");
        
        if (data.ShowSeparatePlayerEntry)
        {
            Debug.Log($"[RankingUIController] 본인 기록: {data.PlayerEntry.Rank}위 ({data.PlayerEntry.RoundScore} 라운드)");
        }
    }
    
    /// <summary>
    /// 리더보드 로드 에러 이벤트 콜백
    /// </summary>
    /// <param name="errorMessage">에러 메시지</param>
    private void OnLeaderboardError(string errorMessage)
    {
        if (leaderboardDisplay != null)
        {
            leaderboardDisplay.ShowErrorState(true, errorMessage);
        }
        
        Debug.LogError($"[RankingUIController] 리더보드 로드 에러: {errorMessage}");
    }
    
    /// <summary>
    /// 현재 플레이어의 점수를 업로드 (외부에서 호출용)
    /// </summary>
    /// <param name="difficulty">업로드할 난이도</param>
    /// <param name="roundScore">라운드 점수</param>
    public void UploadPlayerScore(DifficultyType difficulty, int roundScore)
    {
        if (SteamLeaderboardManager.Instance != null)
        {
            SteamLeaderboardManager.Instance.UploadScore(difficulty, roundScore);
            Debug.Log($"[RankingUIController] 점수 업로드: {difficulty} 난이도 {roundScore} 라운드");
        }
    }
    
    /// <summary>
    /// 현재 선택된 난이도의 리더보드 새로고침
    /// </summary>
    public void RefreshCurrentLeaderboard()
    {
        if (!SteamLeaderboardManager.Instance.IsLoading)
        {
            LoadLeaderboard(currentSelectedDifficulty);
        }
    }
    
    /// <summary>
    /// 컴포넌트 정리
    /// </summary>
    private void OnDestroy()
    {
        UnregisterEvents();
        
        // 버튼 이벤트 해제
        if (easyButton != null)
            easyButton.onClick.RemoveAllListeners();
        if (normalButton != null)
            normalButton.onClick.RemoveAllListeners();
        if (hardButton != null)
            hardButton.onClick.RemoveAllListeners();
        if (hellButton != null)
            hellButton.onClick.RemoveAllListeners();
    }
}