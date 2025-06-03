using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 패턴 6: 대각선 공격
/// </summary>
public class Boss1DiagonalAttackPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;

    public string PatternName => "Diagonal Attack";

    public Boss1DiagonalAttackPattern(GameObject warningTilePrefab, GameObject explosionEffectPrefab)
    {
        _warningTilePrefab = warningTilePrefab;
        _explosionEffectPrefab = explosionEffectPrefab;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteDiagonalAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.Player != null && _warningTilePrefab != null;
    }

    /// <summary>
    /// 대각선 공격 실행
    /// </summary>
    private IEnumerator ExecuteDiagonalAttack(BaseBoss boss)
    {
        boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);

        List<GameObject> warningTiles = new List<GameObject>();
        List<Vector3> attackPositions = new List<Vector3>();

        // 우상단-좌하단 대각선
        int offset = playerY - playerX;
        for (int x = 0; x < boss.GridSystem.Width; x++)
        {
            int y = x + offset;
            if (boss.GridSystem.IsValidPosition(x, y))
            {
                Vector3 pos = boss.GridSystem.GetWorldPosition(x, y);
                attackPositions.Add(pos);
                warningTiles.Add(Object.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
            }
        }

        // 좌상단-우하단 대각선
        offset = playerY + playerX;
        for (int x = 0; x < boss.GridSystem.Width; x++)
        {
            int y = offset - x;
            if (boss.GridSystem.IsValidPosition(x, y))
            {
                Vector3 pos = boss.GridSystem.GetWorldPosition(x, y);
                attackPositions.Add(pos);
                warningTiles.Add(Object.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
            }
        }

        yield return new WaitForSeconds(0.8f);

        boss.GridSystem.GetXY(boss.Player.transform.position, out int currentX, out int currentY);

        // 우상단-좌하단 대각선 검사
        bool isOnDiagonal1 = (currentY - currentX) == (playerY - playerX);

        // 좌상단-우하단 대각선 검사
        bool isOnDiagonal2 = (currentY + currentX) == (playerY + playerX);

        if (isOnDiagonal1 || isOnDiagonal2)
        {
            boss.ApplyDamageToPlayer(15);
        }

        foreach (Vector3 pos in attackPositions)
        {
            boss.CreateDamageEffect(pos, _explosionEffectPrefab);
        }

        foreach (GameObject tile in warningTiles)
        {
            Object.Destroy(tile);
        }
    }
}