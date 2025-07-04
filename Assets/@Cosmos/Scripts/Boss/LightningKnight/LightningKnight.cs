using System.Collections.Generic;
using UnityEngine;

public class LightningKnight : BaseBoss
{
    [Header("번개기사 전용 프리팹들")]
    public GameObject LightningActtck;

    public Vector3 startPosition;

    private List<Vector3Int> PatternA = new List<Vector3Int> { new Vector3Int(7,7,0), new Vector3Int(7, 1, 0), new Vector3Int(1, 1, 0), new Vector3Int(1, 7, 0), new Vector3Int(7, 7, 0) };
    private List<Vector3Int> PatternB = new List<Vector3Int> { new Vector3Int(7, 7, 0), new Vector3Int(1, 7, 0), new Vector3Int(1, 1, 0), new Vector3Int(7, 1, 0), new Vector3Int(7, 7, 0) };
    private List<Vector3Int> PatternC = new List<Vector3Int> { new Vector3Int(7, 4, 0), new Vector3Int(1, 4, 0), new Vector3Int(7, 4, 0) };

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
          .AddPattern(new LightningKnightDashPattern(startPosition, PatternA, LightningActtck), 1f)
          .AddPattern(new LightningKnightPattern3(LightningActtck, new Vector3Int(2, 0, 0)), 0f)
          .AddPattern(new LightningKnightPattern1(LightningActtck), 1f)
          .AddPattern(new LightningKnightPattern2(LightningActtck), 0f)
          .SetGroupInterval(1.0f);

        AddGroup()
           .AddPattern(new LightningKnightDash2Pattern(startPosition, PatternB, LightningActtck), 1f)
           .AddPattern(new LightningKnightPattern2(LightningActtck), 1f)
           .AddPattern(new LightningKnightPattern3(LightningActtck, new Vector3Int(7, 0, 0)), 0f)
           .AddPattern(new LightningKnightPattern1(LightningActtck), 0f)
           .SetGroupInterval(1.0f);

        //AddGroup()
        //    .AddPattern(new LightningKnightDashPattern(startPosition, PatternC), 1f)
        //    .AddPattern(new LightningKnightPattern1(LightningActtck), 1f)
        //    .AddPattern(new LightningKnightPattern3(LightningActtck, new Vector3Int(4, 0, 0)), 1f)
        //    .AddPattern(new LightningKnightPattern2(LightningActtck), 0f)
        //    .SetGroupInterval(1.0f);
    }

    /// <summary>
    /// 보스 전용 전투 데미지 피드백
    /// </summary>
    protected override void DamageFeedback()
    {
        SoundManager.Instance.SlimeSoundClip("SlimeDamageActivate");

        base.DamageFeedback();
    }

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
