using UnityEngine;

/// <summary>
/// 아라크네 보스 - 새로운 패턴 시스템 적용 (디버그 강화)
/// </summary>
public class Arachne : BaseBoss
{
    [Header("보스 전용 프리팹들")]
    public GameObject warningAria;
    public GameObject poisionAriaPrefeb;
    public GameObject spiderLeg;

    /// <summary>
    /// 보스 초기화 - 고유한 스탯 설정
    /// </summary>
    protected void Awake()
    {
        // 기본 스탯 설정
        MaxHealth = 400;
        Debug.Log($"Arachne.Awake: MaxHealth set to {MaxHealth}");
    }

    /// <summary>
    /// 공격 패턴 초기화 - 새로운 그룹 시스템 사용 (디버그 강화)
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        // 그룹 A: Pattern1 → Pattern3
        Debug.Log("Arachne: Creating Group A (Pattern1 → Pattern3)");
        AddGroup()
            .AddPattern(new ArachnePattern1(spiderLeg), 3f)
            .AddPattern(new ArachnePattern1(spiderLeg), 3f)
            .AddPattern(new ArachnePattern1(spiderLeg), 3f)
            .SetGroupInterval(1f);
        Debug.Log("Arachne: Group A created successfully");
    
        // 개별 패턴: Pattern2 (중간 패턴)
        Debug.Log("Arachne: Adding individual Pattern2");
        AddIndividualPattern(new ArachnePattern2(poisionAriaPrefeb), 1f);
        Debug.Log("Arachne: Individual Pattern2 added successfully");
        
        // 그룹 C: Pattern1 → Pattern2 → Pattern3
        Debug.Log("Arachne: Creating Group C (Pattern1 → Pattern2 → Pattern3)");
        AddGroup()
            .AddPattern(new ArachnePattern3(poisionAriaPrefeb, spiderLeg), 3f)
            .AddPattern(new ArachnePattern3(poisionAriaPrefeb, spiderLeg), 3f)
            .AddPattern(new ArachnePattern3(poisionAriaPrefeb, spiderLeg), 3f)
            .SetGroupInterval(1f);
        Debug.Log("Arachne: Group C created successfully");
    
        Debug.Log($"Arachne: Pattern system initialized successfully");
    }
}