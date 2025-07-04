using System.Collections;
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
    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = 2000;
        Debug.Log($"OrcMage.Awake: MaxHealth set to {MaxHealth}");
    }

    /// <summary>
    /// 공격 패턴 초기화 - 패턴 추가
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        Debug.Log("OrcMage.InitializeAttackPatterns: Starting pattern initialization");
        
        // AddIndividualPattern(new OrcMagePattern1(frogPrefab), 2f);
        AddIndividualPattern(new OrcMagePatternWave(groundSpikePrefab), 2f);
        AddIndividualPattern(new OrcMagePatternExpandingSquare(groundSpikePrefab), 2f);
        AddIndividualPattern(new OrcMagePatternChainExplosion(groundSpikePrefab), 2f);
        // 기본기
        AddIndividualPattern(new OrcMagePatternSpiral(groundSpikePrefab), 2f);
        // 돌진 패턴은 항상 시작 - 끝 매칭
        AddGroup()
            .AddPattern(new OrcMagePatternBossChargeLeft(groundSpikePrefab), 2f)
            .AddPattern(new OrcMagePatternWave(groundSpikePrefab), 2f)
            .AddPattern(new OrcMagePatternExpandingSquare(groundSpikePrefab), 2f)
            .AddPattern(new OrcMagePatternBossChargeRight(groundSpikePrefab), 2f)
            .SetGroupInterval(2f);
            
        Debug.Log($"OrcMage: Pattern system initialized successfully");
    }

    protected override void DamageFeedback()
    {
        SoundManager.Instance.OrcMageSoundClip("OrcMage_DamageActivate");
        base.DamageFeedback();
    }

    /// <summary>
    /// 오크 메이지 전용 사망 처리 (오버라이드 가능)
    /// </summary>
    protected override void Die()
    {
        Debug.Log("OrcMage: Casting final spell before death...");
        
        SetAnimationTrigger("Death");
        SoundManager.Instance.OrcMageSoundClip("OrcMage_DieActivate");
        // 기본 사망 처리 호출
        base.Die();
    }
}