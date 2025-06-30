using System.Collections;
using System.Collections.Generic;
using Cosmos.Scripts.Boss.OrcMage;
using UnityEngine;

/// <summary>
/// 최종 보스
/// </summary>
public class BigHand : BaseBoss
{
    [Header("전용 프리팹들")]
    public GameObject leftHandPrefab;
    public GameObject rightHandPrefab;
    public GameObject fingerBottomPrefab;
    public GameObject fingerTopPrefab;
    public GameObject fingerLeftPrefab;
    public GameObject fingerRightPrefab;
    public GameObject wallPrefab;
    public GameObject attackPrefab;
    
    [Header("패턴 간 공유할 상태들")]
    public GameObject LeftHandObject { get; set; }
    public GameObject RightHandObject { get; set; }
    public List<Vector3Int> BlockedPositions { get; set; } = new List<Vector3Int>();
    
    // 손가락 패턴 관련 공유 상태
    public Vector3Int LastFingerTipPosition { get; set; }
    public List<Vector3Int> FingerBlockedPositions { get; set; } = new List<Vector3Int>();
    public GameObject FingerObject { get; set; } // 손가락 오브젝트 직접 저장
    public BigHandFingerPattern FingerPatternReference { get; set; }

    /// <summary>
    /// 초기화 - 고유한 스탯 설정
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        // 기본 스탯 설정
        MaxHealth = 2000;
        Debug.Log($"BigHand.Awake: MaxHealth set to {MaxHealth}");
    }

    /// <summary>
    /// 공격 패턴 초기화 - 패턴 추가
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        Debug.Log("InitializeAttackPatterns: Starting pattern initialization");
        
        AddGroup()
        .AddPattern(new BigHandPattern1A(leftHandPrefab, rightHandPrefab, wallPrefab), 0f)
        .AddPattern(new BigHandPattern2(attackPrefab), 0f)
        .AddPattern(new BigHandPattern3(attackPrefab), 0f)
        .AddPattern(new BigHandPattern4(attackPrefab), 0f)
        .AddPattern(new BigHandPattern5(attackPrefab), 0f)
        .AddPattern(new BigHandPattern1B(), 0f)
        .SetGroupInterval(1f);
        
        AddGroup()
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0.5f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0.5f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0.5f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .SetGroupInterval(1f);
            
        Debug.Log($"Pattern system initialized successfully");
    }

    protected override void DamageFeedback()
    {
        // SoundManager.Instance.OrcMageSoundClip("OrcMage_DamageActivate");
        base.DamageFeedback();
    }

    /// <summary>
    /// 사망 처리 (오버라이드 가능)
    /// </summary>
    protected override void Die()
    {
        SetAnimationTrigger("Death");
        // SoundManager.Instance.OrcMageSoundClip("OrcMage_DieActivate");
        // 기본 사망 처리 호출
        base.Die();
    }
}