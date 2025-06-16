using Cosmos.Scripts.Boss.OrcMage;
using UnityEngine;

/// <summary>
/// 오크 메이지 보스 - 그라운드 스파이크 패턴 추가
/// </summary>
public class OrcMage : BaseBoss
{
    [Header("오크 메이지 전용 프리팹들")]
    public GameObject frogPrefab;        // 개구리 투사체 이펙트
    public GameObject groundSpikePrefab; // 그라운드 스파이크 이펙트

    /// <summary>
    /// 오크 메이지 초기화 - 고유한 스탯 설정
    /// </summary>
    protected void Awake()
    {
        // 기본 스탯 설정 (메이지는 체력이 낮지만 강력한 공격)
        MaxHealth = 300;
        Debug.Log($"OrcMage.Awake: MaxHealth set to {MaxHealth}");
    }

    /// <summary>
    /// 공격 패턴 초기화 - 패턴 추가
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        Debug.Log("OrcMage.InitializeAttackPatterns: Starting pattern initialization");
        
        // AddIndividualPattern(new OrcMagePattern1(frogPrefab), 2f);
        AddIndividualPattern(new OrcMagePattern2(groundSpikePrefab), 2f);
            
        Debug.Log($"OrcMage: Pattern system initialized successfully");
    }

    /// <summary>
    /// 오크 메이지 전용 사망 처리 (오버라이드 가능)
    /// </summary>
    protected override void Die()
    {
        Debug.Log("OrcMage: Casting final spell before death...");
        
        // 기본 사망 처리 호출
        base.Die();
        
        // TODO: 추후 특별한 사망 이펙트나 사운드 추가 가능
    }
}