using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종보스 - Frost2: 랜덤한 지역에 눈꽃 모양으로 얼음 공격을 10회 쏟아붓기
/// </summary>
public class LastBossPattern_Frost2 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    public string PatternName => "Frost2";

    public LastBossPattern_Frost2(GameObject explosionPrefab) => _explosionPrefab = explosionPrefab;
    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");

        for (int i = 0; i < 10; i++)
        {
            Vector3Int center = GetRandomPosition();
            List<Vector3Int> snowflakeShape = GetSnowflakeShape(center);

            foreach (var pos in snowflakeShape)
            {
                if (IsValid(pos))
                {
                    boss.BombHandler.ExecuteFixedBomb(
                        new() { Vector3Int.zero },
                        pos,
                        _explosionPrefab,
                        0.8f,
                        0.8f,
                        20,
                        WarningType.Type1
                    );
                }
            }

            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    private Vector3Int GetRandomPosition()
    {
        // 9x9 격자 기준, 주변 공격을 고려해 가장자리 제외 (1~7)
        int x = Random.Range(1, 8);
        int y = Random.Range(1, 8);
        return new Vector3Int(x, y, 0);
    }

    private List<Vector3Int> GetSnowflakeShape(Vector3Int center)
    {
        List<Vector3Int> shape = new() { center };

        // 십자 방향 (거리 1, 2)
        Vector3Int[] crossDirs = {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };

        foreach (var dir in crossDirs)
        {
            shape.Add(center + dir * 2);    // 거리 2
        }

        // 대각선 방향 (거리 1)
        Vector3Int[] diagonalDirs = {
            new Vector3Int(1, 1, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(-1, -1, 0)
        };

        foreach (var dir in diagonalDirs)
            shape.Add(center + dir);

        return shape;
    }


    private bool IsValid(Vector3Int pos) =>
        pos.x >= 0 && pos.x < 9 && pos.y >= 0 && pos.y < 9;
}
