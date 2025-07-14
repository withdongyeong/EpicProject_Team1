using UnityEngine;

/// <summary>
/// 아라크네 보스 - 새로운 패턴 시스템 적용 (디버그 강화)
/// </summary>
public class Arachne : BaseBoss
{
    [Header("보스 전용 프리팹들")]
    public GameObject warningAria;
    public GameObject poisionAriaPrefeb;
    public GameObject LToRspiderLeg;
    public GameObject RToLspiderLeg;
    public GameObject SpiderWeb;
    /// <summary>
    /// 보스 초기화 - 고유한 스탯 설정
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = GlobalSetting.Instance.GetBossBalance(2).maxHP;
        WeakDamage = GlobalSetting.Instance.GetBossBalance(2).weakDamage;
        StrongDamage = GlobalSetting.Instance.GetBossBalance(2).strongDamage;
        BPM = GlobalSetting.Instance.GetBossBpm(2);
    }

    /// <summary>
    /// 공격 패턴 초기화 - 새로운 그룹 시스템 사용 (디버그 강화)
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //그룹 A: 거미줄 -> 슬래쉬
        AddGroup()
            .AddPattern(new ArachneSpiderWebPattern(SpiderWeb, 16), Beat)
            .AddPattern(new ArachnePattern1(LToRspiderLeg, RToLspiderLeg, StrongDamage), Beat)
            .SetGroupInterval(Beat);

        //개별 패턴: Pattern2(중간 패턴)
        AddGroup()
          .AddPattern(new ArachneSpiderWebPattern(SpiderWeb, 5), Beat)
          .AddPattern(new ArachnePattern2(poisionAriaPrefeb, WeakDamage), Beat)
          .SetGroupInterval(Beat);

        //그룹 C: 
        AddGroup()
            .AddPattern(new ArachnePattern3(poisionAriaPrefeb, LToRspiderLeg, RToLspiderLeg, WeakDamage), Beat)
            .SetGroupInterval(Beat);
    }


    /// <summary>
    /// 아라크네 전용 전투 데미지 피드백
    /// </summary>
    protected override void DamageFeedback()
    {
        SoundManager.Instance.ArachneSoundClip("ArachneDamageActivate");

        base.DamageFeedback();
    }

    /// <summary>
    /// 아라크네 전용 사망 처리 (오버라이드 가능)
    /// </summary>
    protected override void Die()
    {
        SetAnimationTrigger("DeadTrigger");
        SoundManager.Instance.ArachneSoundClip("ArachneDeadActivate");
        // 기본 사망 처리 호출
        base.Die();
    }
}