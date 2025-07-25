using UnityEngine;

/// <summary>
/// 최종 보스
/// </summary>
public class Infinite : BaseBoss
{
    [Header("무한 모드")]
    public GameObject staff;              // 지팡이 프리팹 (머리 위 무기용)
    public GameObject sword;
    public GameObject frost;
    public GameObject flame;
    public GameObject explosionPrefab;   // 공격 이펙트용
    public GameObject frostExplosionPrefab;
    public GameObject flameExplosionPrefab;
    public GameObject swordExplosionPrefab;
    public GameObject wallPrefab;
    private GameObject currentWeapon;
    public GameObject CurrentWeapon => currentWeapon;
    
    [Header("1스테이지")]
    public GameObject SlimeActtckTentacle;
    public GameObject SlimeTrapTentacle;
    
    [Header("2스테이지")]
    public GameObject poisionAriaPrefeb;
    public GameObject LToRspiderLeg;
    public GameObject RToLspiderLeg;
    public GameObject SpiderWeb;
    
    [Header("3스테이지")]
    public GameObject BombActtck;
    public GameObject Bombball;

    [Header("4-6스테이지")]
    public GameObject GroundSpike;

    /// <summary>
    /// 초기화 - 고유한 스탯 설정
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = GlobalSetting.Instance.GetBossBalance(10).maxHP;
        WeakDamage = GlobalSetting.Instance.GetBossBalance(10).weakDamage;
        StrongDamage = GlobalSetting.Instance.GetBossBalance(10).strongDamage;
        BPM = GlobalSetting.Instance.GetBossBpm(10);
        SetDifficulty();
        // 빙결 불가 설정
        IsHandBoss = true;
    }

    /// <summary>
    /// 공격 패턴 초기화 - 패턴 추가
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        // 1스테이지
        AddGroup()
            .AddPattern(new SlimeTentaclePattern(SlimeActtckTentacle, 3), Beat)
            .AddPattern(new SlimeFloorPattern1(SlimeTrapTentacle, WeakDamage), Beat)
            .AddPattern(new SlimeFloorPattern2(SlimeTrapTentacle, WeakDamage), Beat)
            .AddPattern(new SlimeFloorPattern3(SlimeTrapTentacle, WeakDamage), Beat)
            .SetGroupInterval(Beat);
        
        // 2스테이지
        AddGroup()
            .AddPattern(new ArachneSpiderWebPattern(SpiderWeb, 16), Beat)
            .AddPattern(new ArachnePattern1(LToRspiderLeg, RToLspiderLeg, StrongDamage), Beat)
            .AddPattern(new ArachnePattern2(poisionAriaPrefeb, WeakDamage), Beat)
            .AddPattern(new ArachnePattern3(poisionAriaPrefeb, LToRspiderLeg, RToLspiderLeg, StrongDamage,WeakDamage), Beat)
            .SetGroupInterval(Beat);
        
        // 3 스테이지
        AddGroup()
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(8, 0, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(6, 2, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(4, 4, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(2, 6, 0), WeakDamage), Beat)
            .AddPattern(new BomberSpeardAttack(BombActtck, Bombball, new Vector3Int(0, 8, 0), WeakDamage), Beat)
            .AddPattern(new BomberBigBombPattern(BombActtck, Bombball, StrongDamage), Beat)
            .SetGroupInterval(Beat);
        
        // 4 스테이지
        AddGroup()
            .AddPattern(new GuardianGolemVerticalWavePattern(GroundSpike, StrongDamage), Beat)
            .SetGroupInterval(Beat);
        
        // 10스테이지
        AddGroup().
            AddPattern(new LastBossPattern_StaffEquip(staff), 0f).
            AddPattern(new LastBossPattern_Staff(explosionPrefab, WeakDamage), Beat).
            AddPattern(new LastBossPattern_Staff2(explosionPrefab, WeakDamage), Beat).
            AddPattern(new LastBossPattern_Staff3(explosionPrefab, WeakDamage), Beat).
            AddPattern(new LastBossPattern_Staff4(explosionPrefab, WeakDamage), Beat).
            AddPattern(new LastBossPattern_Staff5(explosionPrefab, StrongDamage), 0f).
            SetGroupInterval(Beat);
        
        AddGroup().
            AddPattern(new LastBossPattern_SwordEquip(sword), 0f).
            AddPattern(new LastBossPattern_Sword1(swordExplosionPrefab, WeakDamage), 0f).
            AddPattern(new LastBossPattern_Sword1(swordExplosionPrefab, WeakDamage), 0f).
            AddPattern(new LastBossPattern_Sword1(swordExplosionPrefab, WeakDamage), 0f).
            AddPattern(new LastBossPattern_Sword1(swordExplosionPrefab, WeakDamage), 0f).
            AddPattern(new LastBossPattern_Sword2(swordExplosionPrefab, WeakDamage), 0f).
            AddPattern(new LastBossPattern_Sword3(swordExplosionPrefab, StrongDamage), 0f).
            SetGroupInterval(Beat);
        
        AddGroup().
            AddPattern(new LastBossPattern_FrostEquip(frost), 0f).
            AddPattern(new LastBossPattern_Frost1(frostExplosionPrefab, wallPrefab, WeakDamage), 0f).
            AddPattern(new LastBossPattern_Frost2(frostExplosionPrefab, wallPrefab, WeakDamage), Beat).
            SetGroupInterval(Beat);
        
        AddGroup().
            AddPattern(new LastBossPattern_FlameEquip(flame), 0f).
            AddPattern(new LastBossPattern_Flame1(flameExplosionPrefab, WeakDamage), Beat).
            AddPattern(new LastBossPattern_Flame2(flameExplosionPrefab, WeakDamage), Beat).
            AddPattern(new LastBossPattern_Flame1(flameExplosionPrefab, WeakDamage), Beat).
            AddPattern(new LastBossPattern_Flame2(flameExplosionPrefab, WeakDamage), Beat).
            SetGroupInterval(Beat);
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
        SoundManager.Instance.LastBossSoundClip("LastBossDeadActivate");

        //클리어 도전과제를 달성합니다
        int difficulty = GameManager.Instance.DifficultyLevel;
        if (difficulty == 2)
        {
            SteamAchievement.Achieve("ACH_STG_HARD");
            Debug.Log("하드클리어");
        }

        if(difficulty == 3)
        {
            SteamAchievement.Achieve("ACH_STG_HELL");
            Debug.Log("헬클리어");
        }

        if(GameManager.Instance.DifficultyLevel + 1 > SaveManager.GameModeLevel)
        {
            SaveManager.SaveGameModeLevel(GameManager.Instance.DifficultyLevel + 1);
        }
        
        // 기본 사망 처리 호출
        base.Die();
    }
}