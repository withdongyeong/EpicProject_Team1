using System.Collections;
using Cosmos.Scripts.Boss.OrcMage;
using UnityEngine;

/// <summary>
/// 최종 보스
/// </summary>
public class LastBoss : BaseBoss
{
    // [Header("최종 보스 전용 프리팹들")]
    // public GameObject leftHandPrefab;
    // public GameObject rightHandPrefab;

    /// <summary>
    /// 초기화 - 고유한 스탯 설정
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
        Debug.Log("OrcMageLastBoss.InitializeAttackPatterns: Starting pattern initialization");
        
        // AddIndividualPattern(new BigHandPattern1A(leftHandPrefab, rightHandPrefab), 2f);
            
        Debug.Log($"LastBoss: Pattern system initialized successfully");
    }

    protected override void DamageFeedback()
    {
        // SoundManager.Instance.OrcMageSoundClip("OrcMage_DamageActivate");
        base.DamageFeedback();
    }

    /// <summary>
    /// 오크 메이지 전용 사망 처리 (오버라이드 가능)
    /// </summary>
    protected override void Die()
    {
        SetAnimationTrigger("Death");
        // SoundManager.Instance.OrcMageSoundClip("OrcMage_DieActivate");
        // 기본 사망 처리 호출
        base.Die();
    }
}