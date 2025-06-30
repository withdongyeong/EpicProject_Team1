using UnityEngine;

public class LightningKnight : BaseBoss
{
    [Header("번개기사 전용 프리팹들")]
    public GameObject LightningActtck;
    public GameObject Lightningball;
    public GameObject LightningFlow;

    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = 1000;
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //벽력일섬 - 가운데를 지나가면서 위 아래로 전기 통하기
        AddGroup()
          .AddPattern(new LightningKnightWavePattern(LightningActtck), 1f)
          .SetGroupInterval(1f);

        //번개 소환 3종류
        AddGroup()
            .AddPattern(new LightningKnightWavePattern(LightningActtck), 1f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new LightningKnightWavePattern(LightningActtck), 1f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new LightningKnightWavePattern(LightningActtck), 1f)
            .SetGroupInterval(1f);


        //번개 구슬 소환
        AddGroup()
            .AddPattern(new LightningKnightSpeardAttack(LightningActtck, Lightningball, new Vector3Int(2, 2, 0)), 0.1f)
            .AddPattern(new LightningKnightSpeardAttack(LightningActtck, Lightningball, new Vector3Int(6, 2, 0)), 0.1f)
            .AddPattern(new LightningKnightSpeardAttack(LightningActtck, Lightningball, new Vector3Int(6, 6, 0)), 0.1f)
            .AddPattern(new LightningKnightSpeardAttack(LightningActtck, Lightningball, new Vector3Int(2, 6, 0)), 0.1f)
            .SetGroupInterval(2f);
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
