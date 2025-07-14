using UnityEngine;

/// <summary>
/// 최종 보스
/// </summary>
public class LastBoss : BaseBoss
{
    // [Header("최종 보스 전용 프리팹들")]
    public GameObject staff;              // 지팡이 프리팹 (머리 위 무기용)
    public GameObject sword;
    public GameObject frost;
    public GameObject flame;
    public GameObject explosionPrefab;   // 공격 이펙트용
    public GameObject frostExplosionPrefab;
    public GameObject flameExplosionPrefab;
    public GameObject swordExplosionPrefab;
    private GameObject currentWeapon;
    public GameObject CurrentWeapon => currentWeapon;

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
    }

    /// <summary>
    /// 공격 패턴 초기화 - 패턴 추가
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
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
            AddPattern(new LastBossPattern_Frost1(frostExplosionPrefab, WeakDamage), 0f).
            AddPattern(new LastBossPattern_Frost2(frostExplosionPrefab, WeakDamage), 0f).
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
        if(GameManager.Instance.DifficultyLevel + 1 > SaveManager.GameModeLevel)
        {
            SaveManager.SaveGameModeLevel(GameManager.Instance.DifficultyLevel + 1);
        }
        
        // 기본 사망 처리 호출
        base.Die();
    }
}