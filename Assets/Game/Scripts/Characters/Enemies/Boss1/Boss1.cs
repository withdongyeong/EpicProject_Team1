using UnityEngine;

/// <summary>
/// Boss1
/// </summary>
public class Boss1 : BaseBoss
{
    [Header("보스 전용 프리팹들")]
    public GameObject projectilePrefab;
    public GameObject warningTilePrefab;
    public GameObject explosionEffectPrefab;
    public GameObject meteorPrefab;
    public GameObject magicSwordPrefab;
    
    /// <summary>
    /// 보스 초기화 - 고유한 스탯 설정
    /// </summary>
    protected override void Start()
    {
        // 기본 스탯 설정
        MaxHealth = 200;
        PatternCooldown = 0.6f;
        
        // 부모 클래스 초기화 호출
        base.Start();
    }
    
    /// <summary>
    /// 공격 패턴 초기화 - 7가지 패턴 모두 등록
    /// </summary>
    protected override void InitializeAttackPatterns()
    {
        // 패턴 1: 플레이어 위치 3x3 범위 공격
        AddAttackPattern(new Boss1AreaAttackPattern(warningTilePrefab, explosionEffectPrefab, magicSwordPrefab));
        
        // 패턴 2: 그리드 절반 공격
        AddAttackPattern(new Boss1HalfGridAttackPattern(warningTilePrefab, explosionEffectPrefab, meteorPrefab));
        
        // 패턴 3: 연속 투사체 발사
        AddAttackPattern(new Boss1RapidFirePattern(projectilePrefab, 4, 0.4f));
        
        // 패턴 4: 십자 공격
        AddAttackPattern(new Boss1CrossAttackPattern(warningTilePrefab, explosionEffectPrefab));
        
        // 패턴 5: 연속 영역 공격
        AddAttackPattern(new Boss1MultiAreaAttackPattern(warningTilePrefab, explosionEffectPrefab, magicSwordPrefab, 3, 0.3f));
        
        // 패턴 6: 대각선 공격
        AddAttackPattern(new Boss1DiagonalAttackPattern(warningTilePrefab, explosionEffectPrefab));
        
        // 패턴 7: 대각선 후 십자 공격
        AddAttackPattern(new Boss1DiagonalCrossPattern(warningTilePrefab, explosionEffectPrefab));
        
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