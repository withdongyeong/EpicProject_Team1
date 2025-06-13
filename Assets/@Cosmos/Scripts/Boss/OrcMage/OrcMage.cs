using Cosmos.Scripts.Boss.OrcMage;
using UnityEngine;

/// <summary>
/// 오크 메이지 보스 - 기본 구조 (패턴 추가 예정)
/// </summary>
public class OrcMage : BaseBoss
{
    [Header("오크 메이지 전용 프리팹들")]
    public GameObject frogPrefab;        // 개구리 투사체 이펙트

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
    /// 공격 패턴 초기화 - 현재는 비어있음 (패턴 추가 예정)
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        Debug.Log("OrcMage.InitializeAttackPatterns: No patterns registered yet");
        
        // TODO: 패턴들을 하나씩 추가할 예정
        // 패턴 1 : 개구리 투사체
        // 패턴 2 : 마력 돌진
        AddIndividualPattern(new OrcMagePattern1(frogPrefab), 3f);
        
        Debug.Log("OrcMage: Ready for pattern implementation");
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