using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종 보스 패턴5 - 회전하는 Y자 패턴 (중심 3x3는 항상 공격, 굵기 3의 안전지대, 정확한 Y자 모양)
/// </summary>
public class BigHandPattern5 : IBossAttackPattern
{
    private GameObject _attackEffectPrefab;

    public string PatternName => "패턴5_회전Y자";

    public BigHandPattern5(GameObject attackEffectPrefab)
    {
        _attackEffectPrefab = attackEffectPrefab;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null &&
               _attackEffectPrefab != null &&
               GridManager.Instance != null &&
               boss.BombHandler != null;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");

        Debug.Log("패턴5 시작 - 회전 Y자");

        Vector3Int centerPos = new Vector3Int(4, 4, 0);

        for (int rotationStep = 0; rotationStep < 8; rotationStep++)
        {
            List<Vector3Int> safeZone = GeneratePreciseThickYPattern(rotationStep);
            List<Vector3Int> attackZone = new List<Vector3Int>();

            for (int x = 1; x <= 7; x++)
            {
                for (int y = 1; y <= 7; y++)
                {
                    Vector3Int gridPos = new Vector3Int(x, y, 0);
                    Vector3Int relativePos = gridPos - centerPos;

                    if (IsCenter3x3(gridPos))
                    {
                        attackZone.Add(relativePos);
                    }
                    else if (!safeZone.Contains(relativePos))
                    {
                        attackZone.Add(relativePos);
                    }
                }
            }

            boss.BombHandler.ExecuteFixedBomb(
                attackZone,
                centerPos,
                _attackEffectPrefab,
                warningDuration: 0.6f,
                explosionDuration: 0.8f,
                damage: 18,
                warningType: WarningType.Type1
            );

            yield return new WaitForSeconds(0.6f);
        }

        yield return new WaitForSeconds(0.6f + 0.8f);

        Debug.Log("패턴5 완료");
    }

    private List<Vector3Int> GeneratePreciseThickYPattern(int rotationStep)
    {
        List<Vector3Int> result = new List<Vector3Int>();

        // 중심 기준 Y자 안전지대 정의 (rotation 0 기준)
        List<Vector3Int> basePoints = new List<Vector3Int>
        {
            new Vector3Int(-1, -2, 0), new Vector3Int(0, -2, 0), new Vector3Int(1, -2, 0),
            new Vector3Int(-1, -3, 0), new Vector3Int(0, -3, 0), new Vector3Int(1, -3, 0),

            new Vector3Int(-2, 1, 0), new Vector3Int(-2, 2, 0), new Vector3Int(-3, 2, 0),
            new Vector3Int(-1, 1, 0), new Vector3Int(-2, 0, 0), new Vector3Int(-1, 2, 0),

            new Vector3Int(2, 1, 0), new Vector3Int(2, 2, 0), new Vector3Int(3, 2, 0),
            new Vector3Int(1, 1, 0), new Vector3Int(2, 0, 0), new Vector3Int(1, 2, 0),
        };

        foreach (var point in basePoints)
        {
            Vector3Int rotated = Rotate45(point, rotationStep);
            Vector3Int absolute = rotated + new Vector3Int(4, 4, 0);

            if (IsWithin7x7Grid(absolute) && !IsCenter3x3(absolute))
            {
                if (!result.Contains(rotated))
                    result.Add(rotated);
            }
        }

        return result;
    }


    private Vector3Int Rotate45(Vector3Int pos, int timesCW)
    {
        timesCW = ((timesCW % 8) + 8) % 8;
        for (int i = 0; i < timesCW; i++)
        {
            pos = new Vector3Int(pos.y, -pos.x, 0);
        }
        return pos;
    }

    private bool IsCenter3x3(Vector3Int position)
    {
        return position.x >= 3 && position.x <= 5 &&
               position.y >= 3 && position.y <= 5;
    }

    private bool IsWithin7x7Grid(Vector3Int position)
    {
        return position.x >= 1 && position.x <= 7 &&
               position.y >= 1 && position.y <= 7;
    }

    public void Cleanup()
    {
        Debug.Log("패턴5 정리 완료");
    }
}
