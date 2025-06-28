using UnityEngine;

public class GuardianGolem : BaseBoss
{
    [Header("보스 전용 프리팹들")]
    public GameObject GuardianGolemRook; // 그라운드 스파이크 이펙트
    public GameObject TemporaryWall;
    /// <summary>
    /// 오크 메이지 초기화 - 고유한 스탯 설정
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = 800;
        Debug.Log($"OrcMage.Awake: MaxHealth set to {MaxHealth}");
    }

    /// <summary>
    /// 공격 패턴 초기화 - 패턴 추가
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        AddGroup()
            .AddPattern(new GuardianGolemPattern1(GuardianGolemRook, true), 0.8f)
            .AddPattern(new GuardianGolemPattern1(GuardianGolemRook, false), 0.8f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new GuardianGolemPattern2(GuardianGolemRook), 0.8f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new GuardianGolemVerticalWavePattern(GuardianGolemRook), 0.8f)
            .SetGroupInterval(1f);

        Debug.Log("OrcMage.InitializeAttackPatterns: Starting pattern initialization");
    }

    //protected override void DamageFeedback()
    //{
    //    SoundManager.Instance.OrcMageSoundClip("OrcMage_DamageActivate");
    //    base.DamageFeedback();
    //}

    ///// <summary>
    ///// 오크 메이지 전용 사망 처리 (오버라이드 가능)
    ///// </summary>
    //protected override void Die()
    //{
    //    Debug.Log("OrcMage: Casting final spell before death...");

    //    SetAnimationTrigger("Death");
    //    SoundManager.Instance.OrcMageSoundClip("OrcMage_DieActivate");
    //    // 기본 사망 처리 호출
    //    base.Die();
    //}
}
