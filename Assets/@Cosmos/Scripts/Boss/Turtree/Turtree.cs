using UnityEngine;

public class Turtree : BaseBoss
{
    [Header("보스 전용 프리팹들")]
    public GameObject AttackPrefeb;

    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = GlobalSetting.Instance.GetBossBalance(5).maxHP;
        WeakDamage = GlobalSetting.Instance.GetBossBalance(5).weakDamage;
        StrongDamage = GlobalSetting.Instance.GetBossBalance(5).strongDamage;
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        AddGroup()
            .AddPattern(new TurtreePattern1(AttackPrefeb, new Vector3Int(8, 4, 0), WeakDamage), 1f)
            .AddPattern(new TurtreePattern2(AttackPrefeb, WeakDamage), 1f)
             .AddPattern(new TurtreePattern3(AttackPrefeb, WeakDamage), 1f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new TurtreePattern4(AttackPrefeb, true, WeakDamage), 0.3f)
            .AddPattern(new TurtreePattern1_1(AttackPrefeb, new Vector3Int(4, 8, 0), WeakDamage), 1f)
            .AddPattern(new TurtreePattern4(AttackPrefeb, false, WeakDamage), 0.3f)
             .AddPattern(new TurtreePattern3(AttackPrefeb, WeakDamage), 1f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new TurtreePattern1(AttackPrefeb, new Vector3Int(8, 8, 0), WeakDamage), 1f)
            .AddPattern(new TurtreePattern6(AttackPrefeb, WeakDamage), 1f)
            .AddPattern(new TurtreePattern5(AttackPrefeb, StrongDamage), 1f)
            .AddPattern(new TurtreePattern2(AttackPrefeb, WeakDamage), 1f)
            .SetGroupInterval(1f);
    }

    protected override void DamageFeedback()
    {
        SoundManager.Instance.TurtreeSoundClip("TurtreeDamageActivate");
        base.DamageFeedback();
    }

    /// <summary>
    /// 골렘 전용 사망 처리 (오버라이드 가능)
    /// </summary>
    protected override void Die()
    {
        SoundManager.Instance.TurtreeSoundClip("TurtreeDeadActivate");
        SetAnimationTrigger("DeathTrigger");
        // 기본 사망 처리 호출
        base.Die();
    }

}
