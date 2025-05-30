using UnityEngine;

public class Slime : BaseBoss
{
    [Header("슬라임 전용 프리팹들")]
    public GameObject warningTilePrefab;

    public GameObject SlimeMucus;

    public GameObject warningAriaPrefab;
    public GameObject SlimeActtckTentacle;

    public GameObject SlimeTrapTentacle;
    /// <summary>
    /// 보스 초기화 - 고유한 스탯 설정
    /// </summary>
    protected override void Start()
    {
        // 기본 스탯 설정
        MaxHealth = 200;
        PatternCooldown = 0.5f;

        // 부모 클래스 초기화 호출
        base.Start();
    
    }

    /// <summary>
    /// 공격 패턴 초기화 - 2가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //점액 패턴
        AddAttackPattern(new RapidFirePattern(SlimeMucus, 3, 0.05f));

        //찌르기 촉수 패턴
        AddAttackPattern(new EnemyStraightAttack(warningAriaPrefab, SlimeActtckTentacle, this.transform));

        //바닥 분출 촉수 패턴
        AddAttackPattern(new DiagonalCrossPattern(warningTilePrefab, SlimeTrapTentacle));

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
