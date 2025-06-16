using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오크 메이지 랜덤 폭발 연쇄 패턴 - 랜덤하게 연쇄 폭발
/// </summary>
public class OrcMagePatternChainExplosion : IBossAttackPattern
{
    private GameObject _groundSpikePrefab;

    public string PatternName => "OrcMagePattern_ChainExplosion";

    public OrcMagePatternChainExplosion(GameObject groundSpikePrefab)
    {
        _groundSpikePrefab = groundSpikePrefab;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _groundSpikePrefab != null && boss.BombManager != null;
    }

    /// <summary>
    /// 랜덤 폭발 연쇄 패턴 실행
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        boss.AttackAnimation();

        // 12개의 랜덤 폭발을 연쇄적으로
        List<Vector3Int> usedPositions = new List<Vector3Int>();

        for (int explosion = 0; explosion < 12; explosion++)
        {
            Vector3Int explosionPos = GenerateRandomPosition(usedPositions);
            usedPositions.Add(explosionPos);

            List<Vector3Int> explosionShape = CreateExplosionShape();

            boss.BombManager.ExecuteFixedBomb(explosionShape, explosionPos, _groundSpikePrefab,
                                              warningDuration: 0.8f, explosionDuration: 0.6f, damage: 18, WarningType.Type1);

            yield return new WaitForSeconds(0.25f); // 빠른 연쇄
        }

        // 마지막에 모든 폭발이 끝날 때까지 대기
        yield return new WaitForSeconds(0.8f);
    }

    /// <summary>
    /// 사용되지 않은 랜덤 위치 생성
    /// </summary>
    private Vector3Int GenerateRandomPosition(List<Vector3Int> usedPositions)
    {
        Vector3Int newPos;
        int attempts = 0;

        do
        {
            newPos = new Vector3Int(
                Random.Range(1, 8), // 1~7 (가장자리 피함)
                Random.Range(1, 8),
                0
            );
            attempts++;
        } 
        while (usedPositions.Contains(newPos) && attempts < 20);

        return newPos;
    }

    /// <summary>
    /// 십자가 모양 폭발 생성
    /// </summary>
    private List<Vector3Int> CreateExplosionShape()
    {
        return new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0),   // 중심
            new Vector3Int(1, 0, 0),   // 오른쪽
            new Vector3Int(-1, 0, 0),  // 왼쪽
            new Vector3Int(0, 1, 0),   // 위
            new Vector3Int(0, -1, 0)   // 아래
        };
    }
}