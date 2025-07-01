using UnityEngine;

public class LightningKnight : BaseBoss
{
    [Header("번개기사 전용 프리팹들")]
    public GameObject LightningActtck;

    public Vector3 startPosition;
    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = 700;

        startPosition = this.transform.position;
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        AddGroup()
            .AddPattern(new LightningKnightPattern3(LightningActtck), 1f)
            .AddPattern(new LightningKnightPattern1(LightningActtck), 1f)
            .SetGroupInterval(0.3f);

        AddGroup()
          .AddPattern(new LightningKnightDashPattern(startPosition, 4), 1f)
          .AddPattern(new LightningKnightPattern1(LightningActtck), 1f)
          .AddPattern(new LightningKnightPattern2(LightningActtck), 1f)
          .SetGroupInterval(0.3f);

        AddGroup()
           .AddPattern(new LightningKnightDashPattern(startPosition, 4), 1f)
           .AddPattern(new LightningKnightPattern3(LightningActtck), 1f)
           .AddPattern(new LightningKnightPattern2(LightningActtck), 1f)
           .AddPattern(new LightningKnightPattern1(LightningActtck), 1f)
           .SetGroupInterval(0.3f);

        AddGroup()
            .AddPattern(new LightningKnightDashPattern(startPosition, 4), 1f)
            .AddPattern(new LightningKnightPattern1(LightningActtck), 1f)
            .AddPattern(new LightningKnightPattern3(LightningActtck), 1f)
            .SetGroupInterval(0.3f);
    }

    ///// <summary>
    ///// 보스 전용 전투 데미지 피드백
    ///// </summary>
    //protected override void DamageFeedback()
    //{
    //    SoundManager.Instance.SlimeSoundClip("SlimeDamageActivate");

    //    base.DamageFeedback();
    //}

    ///// <summary>
    ///// 보스 전용 사망 처리 (오버라이드 가능)
    ///// </summary>
    //protected override void Die()
    //{
    //    SetAnimationTrigger("DeadTrigger");
    //    SoundManager.Instance.SlimeSoundClip("SlimeDeadActivate");
    //    // 기본 사망 처리 호출
    //    base.Die();
    //}
}
