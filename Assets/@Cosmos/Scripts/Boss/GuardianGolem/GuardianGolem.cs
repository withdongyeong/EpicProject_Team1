using UnityEngine;

public class GuardianGolem : BaseBoss
{
    [Header("보스 전용 프리팹들")]
    public GameObject GuardianGolemRook; // 그라운드 스파이크 이펙트
    /// <summary>
    /// 오크 메이지 초기화 - 고유한 스탯 설정
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = GlobalSetting.Instance.GetBossBalance(4).maxHP;
        WeakDamage = GlobalSetting.Instance.GetBossBalance(4).weakDamage;
        StrongDamage = GlobalSetting.Instance.GetBossBalance(4).strongDamage;
        BPM = GlobalSetting.Instance.GetBossBpm(4);
    }

    /// <summary>
    /// 공격 패턴 초기화 - 패턴 추가
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        AddGroup()
            .AddPattern(new GuardianGolemPattern1(GuardianGolemRook, true, WeakDamage), Beat)
            .AddPattern(new GuardianGolemPattern1(GuardianGolemRook, false, WeakDamage), Beat)
            .SetGroupInterval(Beat);
        
        AddGroup()
            .AddPattern(new GuardianGolemPattern2(GuardianGolemRook, WeakDamage), Beat)
            .SetGroupInterval(Beat);

        AddGroup()
            .AddPattern(new GuardianGolemVerticalWavePattern(GuardianGolemRook, StrongDamage), Beat)
            .SetGroupInterval(Beat);

        Debug.Log("OrcMage.InitializeAttackPatterns: Starting pattern initialization");
    }

    protected override void DamageFeedback()
    {
        SoundManager.Instance.GolemSoundClip("GolemDamageActivate");
        base.DamageFeedback();
    }

    /// <summary>
    /// 골렘 전용 사망 처리 (오버라이드 가능)
    /// </summary>
    protected override void Die()
    {
        SoundManager.Instance.GolemSoundClip("GolemDeadActivate");
        SetAnimationTrigger("DeathTrigger");
        LifeManager.Instance.AddLife(1);
        // 기본 사망 처리 호출
        base.Die();
    }
}
