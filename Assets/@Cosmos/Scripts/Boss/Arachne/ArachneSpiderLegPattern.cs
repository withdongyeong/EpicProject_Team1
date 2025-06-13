using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 수정된 아라크네 거미 다리 패턴 - BombAvoidanceManager 사용
/// </summary>
public class ArachneSpiderLegPattern : IBossAttackPattern
{
    private GameObject _spiderLegPrefab;
    private BombAvoidanceManager _bombManager;
    private List<Vector3Int> _diagonalSlash1Shape;
    private List<Vector3Int> _diagonalSlash2Shape;
    
    public string PatternName => "ArachneSpiderLeg";

    /// <summary>
    /// 아라크네 거미 다리 패턴 생성자
    /// </summary>
    /// <param name="spiderLegPrefab">거미 다리 이펙트 프리팹</param>
    /// <param name="bombManager">폭탄 피하기 매니저</param>
    public ArachneSpiderLegPattern(GameObject spiderLegPrefab, BombAvoidanceManager bombManager)
    {
        _spiderLegPrefab = spiderLegPrefab;
        _bombManager = bombManager;
        
        // ↘ 방향 대각선 (x == y)
        _diagonalSlash1Shape = new List<Vector3Int>();
        for (int i = -2; i <= 2; i++)
        {
            _diagonalSlash1Shape.Add(new Vector3Int(i, i, 0));
        }
        
        // ↙ 방향 대각선 (x == -y)
        _diagonalSlash2Shape = new List<Vector3Int>();
        for (int i = -2; i <= 2; i++)
        {
            _diagonalSlash2Shape.Add(new Vector3Int(i, -i, 0));
        }
    }

    /// <summary>
    /// 패턴 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(SpiderLeg(boss));
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        return boss.Player != null && _spiderLegPrefab != null && _bombManager != null;
    }

    /// <summary>
    /// 거미 다리 패턴 메인 코루틴
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns></returns>
    private IEnumerator SpiderLeg(BaseBoss boss)
    {
        // 첫 번째 대각선 공격 (↘)
        SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");
        _bombManager.ExecuteTargetingBomb(_diagonalSlash1Shape, _spiderLegPrefab, 
                                          warningDuration: 1.0f, explosionDuration: 0.3f, damage: 10);
        
        yield return new WaitForSeconds(0.3f);
        
        // 두 번째 대각선 공격 (↙)
        _bombManager.ExecuteTargetingBomb(_diagonalSlash2Shape, _spiderLegPrefab, 
                                          warningDuration: 0.5f, explosionDuration: 0.3f, damage: 10);
    }
}