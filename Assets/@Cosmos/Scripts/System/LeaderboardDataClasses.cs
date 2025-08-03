using System.Collections.Generic;

/// <summary>
/// 게임 난이도 열거형
/// </summary>
public enum DifficultyType 
{
    Easy,
    Normal, 
    Hard,
    Hell
}

/// <summary>
/// 리더보드 개별 엔트리 데이터
/// 플레이어 이름, 라운드 점수, 순위 정보를 포함
/// </summary>
public class LeaderboardEntry
{
    private string playerName;
    private int roundScore;
    private int rank;
    
    public string PlayerName { get => playerName; }
    public int RoundScore { get => roundScore; }
    public int Rank { get => rank; }
    
    /// <summary>
    /// 리더보드 엔트리 생성자
    /// </summary>
    /// <param name="playerName">플레이어 이름</param>
    /// <param name="roundScore">라운드 점수</param>
    /// <param name="rank">순위</param>
    public LeaderboardEntry(string playerName, int roundScore, int rank)
    {
        this.playerName = playerName;
        this.roundScore = roundScore;
        this.rank = rank;
    }
}

/// <summary>
/// 리더보드 전체 데이터
/// 상위 10명 + 본인 기록(상위 10명 밖일 경우만 별도 표시) 포함
/// </summary>
public class LeaderboardData
{
    private List<LeaderboardEntry> topEntries;
    private LeaderboardEntry playerEntry;
    private bool showSeparatePlayerEntry; // 상위 10명 밖일 경우만 true
    private bool hasPlayerEntry; // 본인 기록 존재 여부 (하이라이트용)
    
    public List<LeaderboardEntry> TopEntries { get => topEntries; }
    public LeaderboardEntry PlayerEntry { get => playerEntry; }
    public bool ShowSeparatePlayerEntry { get => showSeparatePlayerEntry; }
    public bool HasPlayerEntry { get => hasPlayerEntry; }
    
    /// <summary>
    /// 기존 호환성을 위한 생성자 (showSeparatePlayerEntry = hasPlayerEntry로 설정)
    /// </summary>
    public LeaderboardData(List<LeaderboardEntry> topEntries, LeaderboardEntry playerEntry, bool hasPlayerEntry)
        : this(topEntries, playerEntry, hasPlayerEntry, hasPlayerEntry)
    {
    }
    
    /// <summary>
    /// 리더보드 데이터 생성자
    /// </summary>
    /// <param name="topEntries">상위 10명 리스트</param>
    /// <param name="playerEntry">본인 기록</param>
    /// <param name="showSeparatePlayerEntry">본인 기록 별도 표시 여부 (상위 10명 밖일 경우만)</param>
    /// <param name="hasPlayerEntry">본인 기록 존재 여부 (하이라이트 처리용)</param>
    public LeaderboardData(List<LeaderboardEntry> topEntries, LeaderboardEntry playerEntry, bool showSeparatePlayerEntry, bool hasPlayerEntry)
    {
        this.topEntries = topEntries ?? new List<LeaderboardEntry>();
        this.playerEntry = playerEntry;
        this.showSeparatePlayerEntry = showSeparatePlayerEntry;
        this.hasPlayerEntry = hasPlayerEntry;
    }
}