using UnityEngine;

public class Slime : BaseBoss
{
    [Header("슬라임 전용 프리팹들")]
    public GameObject SlimeActtckTentacle;
    public GameObject SlimeTrapTentacle;

    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = 200;
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //찌르기 촉수 패턴
        AddGroup()
            .AddPattern(new SlimeTentaclePattern(SlimeActtckTentacle, 3), 0.0f)
            .SetGroupInterval(1f);


        AddGroup()
            .AddPattern(new SlimeFloorPattern3(SlimeTrapTentacle), 0.8f)
            .AddPattern(new SlimeFloorPattern1(SlimeTrapTentacle), 0.8f)
            .AddPattern(new SlimeFloorPattern2(SlimeTrapTentacle), 1f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new SlimeFloorPattern2(SlimeTrapTentacle), 0.8f)
            .AddPattern(new SlimeFloorPattern1(SlimeTrapTentacle), 0.8f)
            .AddPattern(new SlimeFloorPattern3(SlimeTrapTentacle), 1f)
            .SetGroupInterval(1f);
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
