using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreantVineWhipPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;
    private int _whipCount;

    public string PatternName => "Diagonal Attack";

    public TreantVineWhipPattern(GameObject warningTilePrefab, GameObject explosionEffectPrefab, int whipCount)
    {
        _warningTilePrefab = warningTilePrefab;
        _explosionEffectPrefab = explosionEffectPrefab;
        _whipCount = whipCount;
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
    /// 덩굴 채찍 공격
    /// </summary>
    private IEnumerator ExecuteDiagonalAttack(BaseBoss boss)
    {
        for(int i = 0; i < _whipCount; i++)
        {
            if (i % 2 == 0)
            {
                boss.StartCoroutine(TopLeftDiagonalAttack(boss));
            }
            else
            {
                boss.StartCoroutine(TopRightDiagonalAttack(boss));
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator TopLeftDiagonalAttack(BaseBoss boss)
    {
        boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);

        List<GameObject> warningTiles = new List<GameObject>();
        List<Vector3> attackPositions = new List<Vector3>();

        // 좌상단-우하단 대각선
        int offset = playerY + playerX;
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

        yield return new WaitForSeconds(0.3f);

        boss.GridSystem.GetXY(boss.Player.transform.position, out int currentX, out int currentY);

        // 좌상단-우하단 대각선 검사
        bool isOnDiagonal2 = (currentY + currentX) == (playerY + playerX);

        if (isOnDiagonal2)
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

    private IEnumerator TopRightDiagonalAttack(BaseBoss boss)
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

        yield return new WaitForSeconds(0.5f);

        boss.GridSystem.GetXY(boss.Player.transform.position, out int currentX, out int currentY);

        // 우상단-좌하단 대각선 검사
        bool isOnDiagonal1 = (currentY - currentX) == (playerY - playerX);

        if (isOnDiagonal1)
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
