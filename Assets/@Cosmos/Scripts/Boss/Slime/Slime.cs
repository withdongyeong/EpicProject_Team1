using UnityEngine;

public class Slime : BaseBoss
{
    [Header("슬라임 전용 프리팹들")]
    public GameObject SlimeActtckTentacle;
    public GameObject SlimeTrapTentacle;

    [Header("슬라임 BPM")]
    public int customBpm = 80;
    
    
    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = GlobalSetting.Instance.GetBossBalance(1).maxHP;
        WeakDamage = GlobalSetting.Instance.GetBossBalance(1).weakDamage;
        StrongDamage = GlobalSetting.Instance.GetBossBalance(1).strongDamage;
        
        // bpm 설정
        bpm = customBpm;
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        // //찌르기 촉수 패턴
        // AddGroup()
        //     .AddPattern(new SlimeTentaclePattern(SlimeActtckTentacle, 3), Beat)
        //     .SetGroupInterval(Beat);


        AddGroup()
            .AddPattern(new SlimeFloorPattern3(SlimeTrapTentacle, WeakDamage), Beat)
            .AddPattern(new SlimeFloorPattern1(SlimeTrapTentacle, WeakDamage), Beat)
            .AddPattern(new SlimeFloorPattern2(SlimeTrapTentacle, WeakDamage), Beat)
            .SetGroupInterval(Beat);

        AddGroup()
            .AddPattern(new SlimeFloorPattern2(SlimeTrapTentacle, WeakDamage), Beat)
            .AddPattern(new SlimeFloorPattern1(SlimeTrapTentacle, WeakDamage), Beat)
            .AddPattern(new SlimeFloorPattern3(SlimeTrapTentacle, WeakDamage), Beat)
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
