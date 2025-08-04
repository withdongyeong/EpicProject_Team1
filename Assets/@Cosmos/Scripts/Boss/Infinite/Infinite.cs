using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 최종 보스
/// </summary>
public class Infinite : LastBoss
{
    [Header("1스테이지")] public GameObject SlimeActtckTentacle;
    public GameObject SlimeTrapTentacle;

    [Header("2스테이지")] public GameObject poisionAriaPrefeb;
    public GameObject LToRspiderLeg;
    public GameObject RToLspiderLeg;
    public GameObject SpiderWeb;

    [Header("3스테이지")] public GameObject BombActtck;
    public GameObject Bombball;

    [Header("4-6스테이지")] public GameObject GroundSpike;

    [Header("4-6스테이지")] public GameObject turtreeAttack;

    [Header("7스테이지")] public GameObject ReaperActtack;
    public GameObject DeathArea;

    [Header("8스테이지")] public GameObject LightningActtack;
    public Vector3 startPosition;

    private List<Vector3Int> PatternA = new List<Vector3Int>
    {
        new Vector3Int(7, 7, 0), new Vector3Int(7, 1, 0), new Vector3Int(1, 1, 0), new Vector3Int(1, 7, 0),
        new Vector3Int(7, 7, 0)
    };

    [Header("9스테이지")] public GameObject fingerBottomPrefab;
    public GameObject fingerTopPrefab;
    public GameObject fingerLeftPrefab;
    public GameObject fingerRightPrefab;
    public GameObject palmLeftPrefab;
    public GameObject palmRightPrefab;
    public GameObject attackPrefab;


    /// <summary>
    /// 초기화 - 고유한 스탯 설정
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        /// 무한모드 설정
        MaxHealth = 5500;
        WeakDamage = 50;
        StrongDamage = 50;
        BPM = 120;

        // 무한모드 강화 적용
        ApplyInfiniteModeEnhancement();

        // 난이도 적용
        SetDifficulty();
        // 빙결 불가 설정
        IsHandBoss = true;
    }

    /// <summary>
    /// 무한모드 도전 횟수에 따른 보스 강화 적용
    /// MaxHealth: 500씩 증가
    /// WeakDamage, StrongDamage: 1씩 증가
    /// BPM: 1씩 증가 (최대 130)
    /// </summary>
    private void ApplyInfiniteModeEnhancement()
    {
        if (StageSelectManager.Instance == null) return;

        int infiniteCount = StageSelectManager.Instance.InfiniteModeCount;

        if (infiniteCount > 0)
        {
            // MaxHealth 강화 (500씩 증가)
            MaxHealth += infiniteCount * 500;

            // 데미지 강화 (1씩 증가)
            WeakDamage += infiniteCount;
            StrongDamage += infiniteCount;

            // BPM 강화 (1씩 증가, 최대 130)
            BPM = Mathf.Min(120 + infiniteCount, 130);

            Debug.Log(
                $"[무한모드 {infiniteCount}] 보스 강화 적용 - HP: {MaxHealth}, 약공: {WeakDamage}, 강공: {StrongDamage}, BPM: {BPM}");
        }
    }

    /// <summary>
    /// 공격 패턴 초기화 - 패턴 추가
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        // 1스테이지
        AddGroup()
            .AddPattern(new SlimeFloorPattern1(SlimeTrapTentacle, WeakDamage), Beat)
            .AddPattern(new SlimeFloorPattern2(SlimeTrapTentacle, WeakDamage), Beat)
            .AddPattern(new SlimeFloorPattern3(SlimeTrapTentacle, WeakDamage), Beat)
            .SetGroupInterval(Beat);

        // 2스테이지
        AddGroup()
            .AddPattern(new ArachneSpiderWebPattern(SpiderWeb, 16), Beat)
            .AddPattern(new ArachnePattern1(LToRspiderLeg, RToLspiderLeg, StrongDamage), Beat)
            .AddPattern(new ArachnePattern3(poisionAriaPrefeb, LToRspiderLeg, RToLspiderLeg, StrongDamage, WeakDamage),
                Beat)
            .SetGroupInterval(Beat);

        // 3 스테이지
        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(8, 0, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 4, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(0, 8, 0), WeakDamage), Beat)
            .AddPattern(new BomberBigBombPattern(BombActtck, Bombball, StrongDamage), Beat)
            .SetGroupInterval(Beat);

        // 4 스테이지
        AddGroup()
            .AddPattern(new GuardianGolemVerticalWavePattern(GroundSpike, StrongDamage), Beat)
            .SetGroupInterval(Beat);

        // 5스테이지
        AddGroup()
            .AddPattern(new TurtreePattern1(turtreeAttack, new Vector3Int(8, 4, 0), WeakDamage), Beat)
            .AddPattern(new TurtreePattern2(turtreeAttack, WeakDamage), Beat)
            .AddPattern(new TurtreePattern6(turtreeAttack, StrongDamage), Beat)
            .SetGroupInterval(Beat);

        // 6스테이지
        AddGroup()
            .AddPattern(new OrcMagePatternExpandingSquare(GroundSpike, WeakDamage), Beat)
            .SetGroupInterval(Beat);

        // 7스테이지
        AddGroup()
            .AddPattern(new ReaperShortDeathAreaPattern(DeathArea, Beat * 5), Beat)
            .AddPattern(new ReaperPattern2(ReaperActtack, WeakDamage), Beat)
            .SetGroupInterval(Beat);

        // 8스테이지
        AddGroup()
            .AddPattern(new LightningKnightPattern3(LightningActtack, new Vector3Int(2, 0, 0), WeakDamage), 0f)
            .AddPattern(new LightningKnightPattern1(LightningActtack, WeakDamage), Beat)
            .AddPattern(new LightningKnightPattern2(LightningActtack, WeakDamage), 0f)
            .SetGroupInterval(Beat);

        // 9스테이지
        AddGroup()
            .AddPattern(new BigHandPalmSweepPattern(palmLeftPrefab, palmRightPrefab, attackPrefab, WeakDamage), 0f)
            .SetGroupInterval(Beat);

        // 10스테이지
        AddGroup().AddPattern(new LastBossPattern_StaffEquip(staff), 0f)
            .AddPattern(new LastBossPattern_Staff(explosionPrefab, WeakDamage), Beat)
            .AddPattern(new LastBossPattern_Staff2(explosionPrefab, WeakDamage), Beat)
            .AddPattern(new LastBossPattern_Staff3(explosionPrefab, WeakDamage), Beat)
            .AddPattern(new LastBossPattern_Staff4(explosionPrefab, WeakDamage), Beat)
            .AddPattern(new LastBossPattern_Staff5(explosionPrefab, StrongDamage), 0f).SetGroupInterval(Beat);

        AddGroup().AddPattern(new LastBossPattern_SwordEquip(sword), 0f)
            .AddPattern(new LastBossPattern_Sword1(swordExplosionPrefab, WeakDamage), 0f)
            .AddPattern(new LastBossPattern_Sword1(swordExplosionPrefab, WeakDamage), 0f)
            .AddPattern(new LastBossPattern_Sword1(swordExplosionPrefab, WeakDamage), 0f)
            .AddPattern(new LastBossPattern_Sword1(swordExplosionPrefab, WeakDamage), 0f)
            .AddPattern(new LastBossPattern_Sword2(swordExplosionPrefab, WeakDamage), 0f)
            .AddPattern(new LastBossPattern_Sword3(swordExplosionPrefab, StrongDamage), 0f).SetGroupInterval(Beat);

        AddGroup().AddPattern(new LastBossPattern_FrostEquip(frost), 0f)
            .AddPattern(new LastBossPattern_Frost1(frostExplosionPrefab, wallPrefab, WeakDamage), 0f)
            .AddPattern(new LastBossPattern_Frost2(frostExplosionPrefab, wallPrefab, WeakDamage), Beat)
            .SetGroupInterval(Beat);

        AddGroup().AddPattern(new LastBossPattern_FlameEquip(flame), 0f)
            .AddPattern(new LastBossPattern_Flame1(flameExplosionPrefab, WeakDamage), Beat)
            .AddPattern(new LastBossPattern_Flame2(flameExplosionPrefab, WeakDamage), Beat)
            .AddPattern(new LastBossPattern_Flame1(flameExplosionPrefab, WeakDamage), Beat)
            .AddPattern(new LastBossPattern_Flame2(flameExplosionPrefab, WeakDamage), Beat).SetGroupInterval(Beat);
    }

    protected override void DamageFeedback()
    {
        SoundManager.Instance.LastBossSoundClip("LastBossDamageActivate");

        SetAnimationTrigger("Damaged");
    }

    public void SetWeaponPrefab(GameObject prefab, float offset, bool isOribit)
    {
        if (currentWeapon != null)
        {
            WeaponFade fade = currentWeapon.GetComponent<WeaponFade>();
            if (fade != null)
            {
                fade.StartFadeOutAndDestroy();
            }
            else
            {
                Destroy(currentWeapon);
            }
        }

        if (prefab == null) return;

        currentWeapon = Instantiate(prefab, transform);
        currentWeapon.transform.localPosition = Vector3.zero;

        // 회전 or 공전 컴포넌트는 원하는 대로 부착
        var fadeIn = currentWeapon.AddComponent<WeaponFade>();
        fadeIn.fadeInOnStart = true;

        if (isOribit)
        {
            var orbit = currentWeapon.AddComponent<OrbitAroundBoss>();
            orbit.bossTransform = this.transform;
            orbit.radius = offset;
            orbit.rotationSpeed = 60f;
        }
        else
        {
            currentWeapon.transform.localPosition = new Vector3(0f, offset, 0f);
        }

        // 예: 자체 회전
        var rotate = currentWeapon.AddComponent<RotateOverHead>();
    }



    /// <summary>
    /// 전용 사망 처리 (오버라이드 가능)
    /// </summary>
    protected override void Die()
    {
        // 무한모드 도전 횟수 증가
        StageSelectManager.Instance.IncrementInfiniteModeCount();
        SoundManager.Instance.LastBossSoundClip("LastBossDeadActivate");

        // 일반 난이도 클리어 도전과제
        int difficulty = GameManager.Instance.DifficultyLevel;
        if (difficulty == 2)
        {
            SteamAchievement.Achieve("ACH_STG_HARD");
            Debug.Log("하드클리어");
        }

        if (difficulty == 3)
        {
            SteamAchievement.Achieve("ACH_STG_HELL");
            Debug.Log("헬클리어");
        }

        // 무한모드 클리어 도전과제 및 리더보드 업로드
        if (StageSelectManager.Instance != null)
        {
            int previousBestRound = StageSelectManager.Instance.InfiniteModeCount;
            int currentRound = previousBestRound-1; // 보스 잡는 순간 카운트 올리고 있기 때문에 기록은 -1
            
            if (currentRound >= 1)
            {
                SteamAchievement.Achieve("ACH_INFINITE_1");
                Debug.Log("무한모드 1 클리어");

                // 리더보드에 현재 클리어한 라운드 수 업로드 (이전 최고가 아닌)
                UploadScoreToLeaderboard(currentRound, difficulty);
            }
        }

        if (GameManager.Instance.DifficultyLevel + 1 > SaveManager.GameModeLevel)
        {
            SaveManager.SaveGameModeLevel(GameManager.Instance.DifficultyLevel + 1);
        }

        // 기본 사망 처리 호출
        base.Die();
    }

    /// <summary>
    /// 무한모드 클리어 점수를 리더보드에 업로드
    /// </summary>
    /// <param name="currentRoundCleared">현재 클리어한 라운드 수</param>
    /// <param name="difficultyLevel">게임 난이도 (0=Easy, 1=Normal, 2=Hard, 3=Hell)</param>
    private void UploadScoreToLeaderboard(int currentRoundCleared, int difficultyLevel)
    {
        Debug.Log($"=== 업로드 시도 ===");
        Debug.Log($"현재 클리어한 라운드: {currentRoundCleared}");
        Debug.Log($"이전 최고 기록(InfiniteModeCount): {StageSelectManager.Instance.InfiniteModeCount}");
        Debug.Log($"difficulty: {difficultyLevel}");
        
        if (SteamLeaderboardManager.Instance == null)
        {
            Debug.LogWarning("[Infinite] SteamLeaderboardManager가 없어서 점수 업로드를 건너뜁니다.");
            return;
        }

        // 게임 난이도를 DifficultyType으로 변환
        DifficultyType difficulty = ConvertToDifficultyType(difficultyLevel);

        // 리더보드에 현재 클리어한 라운드 수 업로드
        SteamLeaderboardManager.Instance.UploadScore(difficulty, currentRoundCleared);

        Debug.Log($"[Infinite] 무한모드 점수 업로드: {difficulty} 난이도, {currentRoundCleared} 라운드");
    }

    /// <summary>
    /// 게임 난이도 레벨을 DifficultyType으로 변환
    /// </summary>
    /// <param name="difficultyLevel">게임 난이도 (0~3)</param>
    /// <returns>변환된 DifficultyType</returns>
    private DifficultyType ConvertToDifficultyType(int difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case 0: return DifficultyType.Easy;
            case 1: return DifficultyType.Normal;
            case 2: return DifficultyType.Hard;
            case 3: return DifficultyType.Hell;
            default:
                Debug.LogWarning($"[Infinite] 알 수 없는 난이도: {difficultyLevel}, Easy로 기본 설정");
                return DifficultyType.Easy;
        }
    }
}