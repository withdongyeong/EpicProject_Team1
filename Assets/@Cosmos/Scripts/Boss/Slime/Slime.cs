using UnityEngine;

public class Slime : BaseBoss
{
    [Header("슬라임 전용 프리팹들")]
    public GameObject SlimeActtckTentacle;
    public GameObject SlimeTrapTentacle;

    private void Awake()
    {
        // 기본 스탯 설정
        MaxHealth = 400;
    }

    /// <summary>
    /// 보스 초기화 - 고유한 스탯 설정
    /// </summary>
    protected override void Start()
    {
        // 부모 클래스 초기화 호출
        base.Start();
    
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //찌르기 촉수 패턴
        AddGroup()
            .AddPattern(new SlimeTentaclePattern(SlimeActtckTentacle, 5), 0.0f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new SlimeFloorPattern1(SlimeTrapTentacle), 1f)
            .AddPattern(new SlimeFloorPattern2(SlimeTrapTentacle), 1f)
            .AddPattern(new SlimeFloorPattern3(SlimeTrapTentacle), 1f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new SlimeFloorPattern2(SlimeTrapTentacle), 1f)
            .AddPattern(new SlimeFloorPattern3(SlimeTrapTentacle), 1f)
            .AddPattern(new SlimeFloorPattern1(SlimeTrapTentacle), 1f)
            .SetGroupInterval(1f);

        AddGroup()
            .AddPattern(new SlimeFloorPattern3(SlimeTrapTentacle), 1f)
            .AddPattern(new SlimeFloorPattern1(SlimeTrapTentacle), 1f)
            .AddPattern(new SlimeFloorPattern2(SlimeTrapTentacle), 1f)
            .SetGroupInterval(1f);

        Debug.Log($"{GetType().Name}: {GetAttackPatterns().Count} attack patterns initialized");
    }

    /// <summary>
    /// 등록된 공격 패턴 목록 반환 (디버그용)
    /// </summary>
    private System.Collections.Generic.List<IBossAttackPattern> GetAttackPatterns()
    {
        return new System.Collections.Generic.List<IBossAttackPattern>();
    }
}
