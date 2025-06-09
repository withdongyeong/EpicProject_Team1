using UnityEngine;
using System.Collections.Generic;

public class Arachne : BaseBoss
{
    [Header("보스 전용 프리팹들")]
    public GameObject SpiderWebPrefeb;

    public List<GameObject> SummonSpiders;

    public GameObject spiderSilkPrefeb;

    public GameObject warningAria;
    public GameObject poisionAriaPrefeb;

    public GameObject SpiderLeg;

    public PlayerController PlayerController;

    /// <summary>
    /// 보스 초기화 - 고유한 스탯 설정
    /// </summary>
    protected void Awake()
    {
        // 기본 스탯 설정
        MaxHealth = 300;
        PatternCooldown = 0.5f;
    }

    protected override void Start()
    {
        PlayerController = FindAnyObjectByType<PlayerController>();
        // 부모 클래스 초기화 호출
        base.Start();
    }

    /// <summary>
    /// 공격 패턴 초기화 - 5가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        // 패턴 1: 거미줄
        AddAttackPattern(new ArachneSpiderWebPattern(SpiderWebPrefeb, 3));

        // 패턴 2: 종자 거미 공격
        AddAttackPattern(new ArachneSummonSpiderPattern(SummonSpiders, 5));

        // 패턴 3: 거미줄 잡기
        AddAttackPattern(new ArachneSpiderSilkPattern(spiderSilkPrefeb, 1));

        // 패턴 4: 독 분출
        AddAttackPattern(new ArachnePoisionAriaPattern(warningAria, poisionAriaPrefeb, PlayerController));

        //패턴 5: 다리 공격
        AddAttackPattern(new ArachneSpiderLegPattern(warningAria, SpiderLeg, PlayerController));

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
