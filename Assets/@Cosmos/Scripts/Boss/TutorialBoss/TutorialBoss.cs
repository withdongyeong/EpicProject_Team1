using UnityEngine;

public class TutorialBoss : BaseBoss
{
    [Header("튜토리얼 보스 전용 프리팹들")]
    public GameObject TutorialBossAttack;

    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = GlobalSetting.Instance.GetBossBalance(0).maxHP;
        WeakDamage = GlobalSetting.Instance.GetBossBalance(0).weakDamage;
        StrongDamage = GlobalSetting.Instance.GetBossBalance(0).strongDamage;
        
        BPM = GlobalSetting.Instance.GetBossBpm(0);

    }

    /// <summary>
    /// 공격 패턴 초기화 - 1가지 패턴
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        AddGroup()
            .AddPattern(new TutorialBossPattern(TutorialBossAttack, true, WeakDamage), Beat)
            .AddPattern(new TutorialBossPattern(TutorialBossAttack, false, WeakDamage), Beat)
            .SetGroupInterval(Beat);
    }

    /// <summary>
    /// 슬라임 전용 전투 데미지 피드백
    /// </summary>
    protected override void DamageFeedback()
    {
        SoundManager.Instance.SlimeSoundClip("SlimeDamageActivate");

        base.DamageFeedback();
    }

    /// <summary>
    /// 슬라임 전용 사망 처리 (오버라이드 가능)
    /// </summary>
    protected override void Die()
    {
        SetAnimationTrigger("DeadTrigger");
        SoundManager.Instance.SlimeSoundClip("SlimeDeadActivate");
        // 기본 사망 처리 호출
        base.Die();
    }
}