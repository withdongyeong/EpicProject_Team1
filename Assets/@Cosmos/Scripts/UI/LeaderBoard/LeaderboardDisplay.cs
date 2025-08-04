using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Localization;

/// <summary>
/// 리더보드 데이터를 UI에 표시하는 컴포넌트
/// 상위 10명 + 본인 기록(상위 10명 밖일 경우) 표시 담당
/// </summary>
public class LeaderboardDisplay : MonoBehaviour
{
    [Header("리더보드 UI 요소")]
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [SerializeField] private GameObject playerEntryPrefab; // 본인 기록용 (다른 스타일)
    [SerializeField] private Image leaderboardFrame; // 전체 리더보드 프레임
    
    [Header("상태 표시")]
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private TextMeshProUGUI errorText;
    
    [Header("본인 기록 별도 표시 영역")]
    [SerializeField] private GameObject playerEntrySection;
    [SerializeField] private Transform playerEntryContainer;
    
    [Header("본인 기록 하이라이트 색상")]
    [SerializeField] private Color playerBackgroundColor = new Color(1f, 1f, 0.8f, 1f); // 연한 노란색
    [SerializeField] private Color playerTextColor = new Color(0.8f, 0.2f, 0.2f, 1f); // 빨간색
    
    [Header("난이도별 프레임 배경 이미지")]
    [SerializeField] private Sprite easyFrameSprite;
    [SerializeField] private Sprite normalFrameSprite;
    [SerializeField] private Sprite hardFrameSprite;
    [SerializeField] private Sprite hellFrameSprite;
    
    private DifficultyType currentDifficulty;
    
    private List<GameObject> currentEntryObjects;
    private GameObject currentPlayerEntryObject;
    
    public bool IsInitialized { get; private set; }
    private void Awake()
    {
        currentEntryObjects = new List<GameObject>();
        InitializeDisplay();
    }
    
    /// <summary>
    /// 디스플레이 초기화 및 유효성 검사
    /// </summary>
    private void InitializeDisplay()
    {
        // 필수 컴포넌트 유효성 검사
        if (leaderboardContainer == null)
        {
            Debug.LogError("[LeaderboardDisplay] leaderboardContainer가 설정되지 않았습니다.");
            return;
        }
        
        if (leaderboardEntryPrefab == null)
        {
            Debug.LogError("[LeaderboardDisplay] leaderboardEntryPrefab이 설정되지 않았습니다.");
            return;
        }
        
        // 본인 기록용 프리팹이 없으면 일반 프리팹 사용
        if (playerEntryPrefab == null)
        {
            playerEntryPrefab = leaderboardEntryPrefab;
            Debug.LogWarning("[LeaderboardDisplay] playerEntryPrefab이 설정되지 않아 일반 프리팹을 사용합니다.");
        }
        
        // 본인 기록 컨테이너가 없으면 일반 컨테이너 사용
        if (playerEntryContainer == null)
        {
            playerEntryContainer = leaderboardContainer;
            Debug.LogWarning("[LeaderboardDisplay] playerEntryContainer가 설정되지 않아 일반 컨테이너를 사용합니다.");
        }
        
        IsInitialized = true;
        
        // 초기 상태 설정
        ShowLoadingState(false);
        ShowErrorState(false, "");
        ShowPlayerEntrySection(false);
    }
    
    /// <summary>
    /// 리더보드 데이터를 UI에 표시
    /// </summary>
    /// <param name="data">표시할 리더보드 데이터</param>
    /// <param name="difficulty">현재 선택된 난이도</param>
    public void DisplayLeaderboard(LeaderboardData data, DifficultyType difficulty)
    {
        if (!IsInitialized)
        {
            Debug.LogError("[LeaderboardDisplay] 컴포넌트가 초기화되지 않았습니다.");
            return;
        }
        
        if (data == null)
        {
            ShowErrorState(true, "리더보드 데이터가 없습니다");
            return;
        }
        
        currentDifficulty = difficulty;
        
        ShowLoadingState(false);
        ShowErrorState(false, "");
        
        // 기존 UI 정리
        ClearCurrentDisplay();
        
        // 프레임 배경 이미지 설정
        SetLeaderboardFrameBackground(difficulty);
        
        // 상위 10명 표시
        DisplayTopEntries(data);
        
        // 본인 기록 별도 표시 (상위 10명 밖일 경우)
        if (data.ShowSeparatePlayerEntry && data.PlayerEntry != null)
        {
            DisplayPlayerEntry(data.PlayerEntry);
        }
        else
        {
            ShowPlayerEntrySection(false);
        }
    }
    
    /// <summary>
    /// 리더보드 데이터를 UI에 표시 (기존 호환성)
    /// </summary>
    /// <param name="data">표시할 리더보드 데이터</param>
    public void DisplayLeaderboard(LeaderboardData data)
    {
        DisplayLeaderboard(data, currentDifficulty);
    }
    
    /// <summary>
    /// 로딩 상태 표시
    /// </summary>
    public void ShowLoadingState(bool show)
    {
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(show);
        }
        
        if (show)
        {
            ClearCurrentDisplay();
            ShowErrorState(false, "");
            ShowPlayerEntrySection(false);
        }
    }
    
    /// <summary>
    /// 에러 상태 표시
    /// </summary>
    /// <param name="show">에러 표시 여부</param>
    /// <param name="message">에러 메시지</param>
    public void ShowErrorState(bool show, string message)
    {
        if (errorText != null)
        {
            errorText.gameObject.SetActive(show);
            if (show)
            {
                errorText.text = message;
            }
        }
        
        if (show)
        {
            ShowLoadingState(false);
            ClearCurrentDisplay();
            ShowPlayerEntrySection(false);
        }
    }
    
    /// <summary>
    /// 상위 10명 엔트리 표시
    /// </summary>
    /// <param name="data">리더보드 데이터</param>
    private void DisplayTopEntries(LeaderboardData data)
    {
        Debug.Log($"[LeaderboardDisplay] DisplayTopEntries 호출됨 - 엔트리 수: {data.TopEntries.Count}");
        Debug.Log($"[LeaderboardDisplay] leaderboardContainer: {(leaderboardContainer != null ? leaderboardContainer.name : "NULL")}");
    
        for (int i = 0; i < data.TopEntries.Count; i++)
        {
            LeaderboardEntry entry = data.TopEntries[i];
        
            // 엔트리 생성 전 로그
            Debug.Log($"[LeaderboardDisplay] {i+1}번째 엔트리 생성 시작: {entry.PlayerName}, {entry.RoundScore}라운드, {entry.Rank}위");
            Debug.Log($"[LeaderboardDisplay] 부모 Transform: {leaderboardContainer.name} (Position: {leaderboardContainer.position})");
        
            GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
        
            // 엔트리 생성 후 로그
            Debug.Log($"[LeaderboardDisplay] 엔트리 오브젝트 생성됨: {entryObj.name} (Position: {entryObj.transform.position})");
            Debug.Log($"[LeaderboardDisplay] 실제 부모: {entryObj.transform.parent.name}");
        
            // 엔트리 데이터 설정
            SetupEntryUI(entryObj, entry, IsPlayerEntry(entry, data));
        
            currentEntryObjects.Add(entryObj);
        }
    
        Debug.Log($"[LeaderboardDisplay] 총 {currentEntryObjects.Count}개 엔트리 생성 완료");
    }
    
    /// <summary>
    /// 본인 기록 별도 표시
    /// </summary>
    /// <param name="playerEntry">본인 기록 데이터</param>
    private void DisplayPlayerEntry(LeaderboardEntry playerEntry)
    {
        Debug.Log($"[LeaderboardDisplay] DisplayPlayerEntry 호출됨: {playerEntry.PlayerName}, {playerEntry.RoundScore}라운드, {playerEntry.Rank}위");
        Debug.Log($"[LeaderboardDisplay] playerEntryContainer: {(playerEntryContainer != null ? playerEntryContainer.name : "NULL")}");
    
        ShowPlayerEntrySection(true);
    
        Debug.Log($"[LeaderboardDisplay] 본인 기록 엔트리 생성 시작 - 부모: {playerEntryContainer.name}");
    
        currentPlayerEntryObject = Instantiate(playerEntryPrefab, playerEntryContainer);
    
        Debug.Log($"[LeaderboardDisplay] 본인 기록 엔트리 생성됨: {currentPlayerEntryObject.name}");
        Debug.Log($"[LeaderboardDisplay] 실제 부모: {currentPlayerEntryObject.transform.parent.name}");
    
        SetupEntryUI(currentPlayerEntryObject, playerEntry, true);
    }
    
    /// <summary>
    /// 엔트리 UI 설정
    /// </summary>
    /// <param name="entryObj">설정할 엔트리 오브젝트</param>
    /// <param name="entry">엔트리 데이터</param>
    /// <param name="isPlayer">본인 기록 여부 (하이라이트용)</param>
    private void SetupEntryUI(GameObject entryObj, LeaderboardEntry entry, bool isPlayer)
    {
        TextMeshProUGUI rankText = new TextMeshProUGUI();
        LocalizedString localizedRankText = new LocalizedString("EpicProject_Table", "UI_Text_RankingView");

        localizedRankText.StringChanged += (Text) =>
        {
            rankText.text = Text.Replace("{0}", entry.Rank.ToString());
        };


        // 순위 텍스트 설정 - Entry 자식 아래에서 찾기
        SetTextComponent(entryObj, "Entry/RankText", rankText.text);
    
        // 플레이어 이름 텍스트 설정
        SetTextComponent(entryObj, "Entry/NameText", entry.PlayerName);
    
        // 라운드 점수 텍스트 설정
        SetTextComponent(entryObj, "Entry/ScoreText", $"{entry.RoundScore}");
    
        // 본인 기록 하이라이트 처리
        if (isPlayer)
        {
            ApplyPlayerHighlight(entryObj);
        }
    }
    
    /// <summary>
    /// 리더보드 프레임 배경 이미지 설정
    /// </summary>
    /// <param name="difficulty">현재 선택된 난이도</param>
    private void SetLeaderboardFrameBackground(DifficultyType difficulty)
    {
        if (leaderboardFrame == null) return;
        
        Sprite frameSprite = GetFrameSpriteByDifficulty(difficulty);
        if (frameSprite != null)
        {
            leaderboardFrame.sprite = frameSprite;
        }
    }
    
    /// <summary>
    /// 난이도에 따른 프레임 스프라이트 반환
    /// </summary>
    /// <param name="difficulty">난이도</param>
    /// <returns>해당 난이도의 프레임 스프라이트</returns>
    private Sprite GetFrameSpriteByDifficulty(DifficultyType difficulty)
    {
        switch (difficulty)
        {
            case DifficultyType.Easy: return easyFrameSprite;
            case DifficultyType.Normal: return normalFrameSprite;
            case DifficultyType.Hard: return hardFrameSprite;
            case DifficultyType.Hell: return hellFrameSprite;
            default: return null;
        }
    }
    /// <summary>
    /// 지정된 자식 오브젝트의 텍스트 컴포넌트에 텍스트 설정
    /// </summary>
    /// <param name="parent">부모 오브젝트</param>
    /// <param name="childName">자식 오브젝트 이름</param>
    /// <param name="text">설정할 텍스트</param>
    private void SetTextComponent(GameObject parent, string childName, string text)
    {
        Transform child = parent.transform.Find(childName);
        if (child != null)
        {
            TextMeshProUGUI textComponent = child.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = text;
                Debug.Log($"[LeaderboardDisplay] 텍스트 설정 성공: {childName} = {text}");
            }
            else
            {
                Debug.LogWarning($"[LeaderboardDisplay] {childName}에서 TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning($"[LeaderboardDisplay] {childName} 자식 오브젝트를 찾을 수 없습니다. 부모: {parent.name}");
        
            // 디버깅용 - 실제 자식 오브젝트들 출력
            Debug.Log($"[LeaderboardDisplay] {parent.name}의 자식 오브젝트들:");
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                Debug.Log($"  - {parent.transform.GetChild(i).name}");
            }
        }
    }
    
    /// <summary>
    /// 본인 기록인지 확인
    /// </summary>
    /// <param name="entry">확인할 엔트리</param>
    /// <param name="data">전체 리더보드 데이터</param>
    /// <returns>본인 기록 여부</returns>
    private bool IsPlayerEntry(LeaderboardEntry entry, LeaderboardData data)
    {
        return data.HasPlayerEntry && 
               data.PlayerEntry != null && 
               entry.Rank == data.PlayerEntry.Rank;
    }
    
    /// <summary>
    /// 본인 기록 하이라이트 적용
    /// </summary>
    /// <param name="entryObj">하이라이트 적용할 오브젝트</param>
    private void ApplyPlayerHighlight(GameObject entryObj)
    {
        // Entry 자식 오브젝트에서 배경 이미지 찾기
        Transform entryTransform = entryObj.transform.Find("Entry");
        if (entryTransform != null)
        {
            // 배경 이미지 색상 변경
            Image backgroundImage = entryTransform.GetComponent<Image>();
            if (backgroundImage != null)
            {
                backgroundImage.color = playerBackgroundColor;
            }
        
            // Entry 하위의 모든 텍스트 색상 변경
            TextMeshProUGUI[] texts = entryTransform.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var text in texts)
            {
                text.color = playerTextColor;
                Debug.Log($"[LeaderboardDisplay] 본인 기록 하이라이트 적용: {text.name}");
            }
        }
        else
        {
            Debug.LogWarning("[LeaderboardDisplay] Entry 자식 오브젝트를 찾을 수 없습니다.");
        }
    }
    
    /// <summary>
    /// 본인 기록 섹션 표시/숨김
    /// </summary>
    /// <param name="show">표시 여부</param>
    private void ShowPlayerEntrySection(bool show)
    {
        if (playerEntrySection != null)
        {
            playerEntrySection.SetActive(show);
        }
    }
    
    /// <summary>
    /// 현재 표시된 리더보드 UI 정리
    /// </summary>
    private void ClearCurrentDisplay()
    {
        // 상위 10명 엔트리 정리
        foreach (GameObject entryObj in currentEntryObjects)
        {
            if (entryObj != null)
            {
                Destroy(entryObj);
            }
        }
        currentEntryObjects.Clear();
        
        // 본인 기록 엔트리 정리
        if (currentPlayerEntryObject != null)
        {
            Destroy(currentPlayerEntryObject);
            currentPlayerEntryObject = null;
        }
    }
    
    /// <summary>
    /// 컴포넌트 정리
    /// </summary>
    private void OnDestroy()
    {
        ClearCurrentDisplay();
    }
}