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
    public GameObject fistPrefab;
    public GameObject palmLeftPrefab;
    public GameObject palmRightPrefab;
    
    [Header("패턴 간 공유할 상태들")]
    public GameObject LeftHandObject { get; set; }
    public GameObject RightHandObject { get; set; }
    public List<Vector3Int> BlockedPositions { get; set; } = new List<Vector3Int>();
    
    // 주먹 패턴 상태
    public Vector3Int PlannedFistCenterColumn { get; set; }
    public List<Vector3Int> PlannedFistArea { get; set; }
    public Vector3Int FistCenterPosition { get; set; }
    public List<Vector3Int> FistBlockedPositions { get; set; }
    public GameObject FistObject { get; set; }
    
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
        MaxHealth = GlobalSetting.Instance.GetBossBalance(9).maxHP;
        WeakDamage = GlobalSetting.Instance.GetBossBalance(9).weakDamage;
        StrongDamage = GlobalSetting.Instance.GetBossBalance(9).strongDamage;
        // 빙결 불가 설정
        IsHandBoss = true;
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
        .AddPattern(new BigHandPattern2(attackPrefab, WeakDamage), 0f)
        .AddPattern(new BigHandPattern3(attackPrefab, WeakDamage), 0f)
        .AddPattern(new BigHandPattern4(attackPrefab, WeakDamage), 0f)
        .AddPattern(new BigHandPattern5(attackPrefab, WeakDamage), 0f)
        .AddPattern(new BigHandPattern1B(), 0f)
        .SetGroupInterval(1f);
        
        AddGroup()
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .AddPattern(new BigHandFistPrepPattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFistPattern(fistPrefab, attackPrefab, StrongDamage), 0f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .SetGroupInterval(1f);
        
        AddGroup()
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .AddPattern(new BigHandPalmSweepPattern(palmLeftPrefab, palmRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .SetGroupInterval(1f);
        
        AddGroup()
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .AddPattern(new BigHandPalmSweepPattern(palmLeftPrefab, palmRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFistPrepPattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFistPattern(fistPrefab, attackPrefab, StrongDamage), 0f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .SetGroupInterval(1f);
        
        AddGroup()
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .AddPattern(new BigHandPalmSweepPattern(palmLeftPrefab, palmRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFistPrepPattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFistPattern(fistPrefab, attackPrefab, StrongDamage), 0f)
            .AddPattern(new BigHandPalmSweepPattern(palmLeftPrefab, palmRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerPattern(fingerBottomPrefab, fingerTopPrefab, fingerLeftPrefab, fingerRightPrefab, attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandRadialWavePattern(attackPrefab, WeakDamage), 0f)
            .AddPattern(new BigHandFingerReturnPattern(), 0f)
            .SetGroupInterval(1f);
        
        
            
        Debug.Log($"Pattern system initialized successfully");
    }

    protected override void DamageFeedback()
    {
        SoundManager.Instance.BigHandSoundClip("BigHandDamageActivate");

        // SoundManager.Instance.OrcMageSoundClip("OrcMage_DamageActivate");
        base.DamageFeedback();
    }

    /// <summary>
    /// 사망 처리 (오버라이드 가능)
    /// </summary>
    protected override void Die()
    {
        SetAnimationTrigger("Death");
        SoundManager.Instance.BigHandSoundClip("BigHandDeadActivate");

        // 기본 사망 처리 호출
        base.Die();
    }
}