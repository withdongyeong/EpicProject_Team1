using UnityEngine;

public class Treant : BaseBoss
{
    [Header("트랜트 전용 프리팹들")]
    public GameObject WarningTilePrefab;
    public GameObject TreeTrapPrefab;
    
    public GameObject CropsPrefeb;

    public GameObject WarningAriaPrefeb;
    public GameObject TreantWindMagic;

    /// <summary>
    /// 보스 초기화 - 고유한 스탯 설정
    /// </summary>
    protected override void Start()
    {
        // 기본 스탯 설정
        MaxHealth = 200;
        PatternCooldown = 0.3f;

        // 부모 클래스 초기화 호출
        base.Start();

    }

    /// <summary>
    /// 공격 패턴 초기화 - 3가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        //바닥 나무 패턴
        AddAttackPattern(new TreeTrapPattern(WarningTilePrefab, TreeTrapPrefab));

        //작물 던지기 패턴
        AddAttackPattern(new RapidFirePattern(CropsPrefeb, 3, 0.1f));

        //강제 이동 패턴
        AddAttackPattern(new EnemyStraightAttack(WarningAriaPrefeb, TreantWindMagic, this.transform));


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
