using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종보스 - Sword2: 대각선 방향 단일 공격 (3번 패턴의 검 1개 버전)
/// </summary>
public class LastBossPattern_Sword2 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    private int _damage;
    public string PatternName => "Sword2";

    public LastBossPattern_Sword2(GameObject explosionPrefab, int damage)
    {
        _explosionPrefab = explosionPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");

        var (startPos, direction) = GetRandomDiagonalLine();

        // 1. 검 모양 1회 공격 (시작점에서)
        List<Vector3Int> swordShape = GetSwordShape(startPos, direction);
        foreach (var pos in swordShape)
        {
            if (IsValid(pos))
            {
                boss.StartCoroutine(SoundPlay());

                boss.BombHandler.ExecuteFixedBomb(
                    new() { Vector3Int.zero },
                    pos,
                    _explosionPrefab,
                    1f,
                    1f,
                    _damage,
                    WarningType.Type1
                );
            }
        }

        yield return new WaitForSeconds(boss.Beat/2);

        // 2. 대각선으로 연속 공격 (굵기 3)
        int maxLength = GetMaxDiagonalLength();
        
        for (int step = 1; step <= maxLength; step++)
        {
            Vector3Int currentPos = startPos + direction * step;
            
            if (IsValid(currentPos))
            {
                List<Vector3Int> thickLine = GetThickDiagonalLine(currentPos, direction);
                
                foreach (var pos in thickLine)
                {
                    if (IsValid(pos))
                    {
                        boss.StartCoroutine(SoundPlay());

                        boss.BombHandler.ExecuteFixedBomb(
                            new() { Vector3Int.zero },
                            pos,
                            _explosionPrefab,
                            1f,
                            1f,
                            _damage,
                            WarningType.Type1
                        );
                    }
                }
            }

            yield return new WaitForSeconds(boss.Beat/8);
        }

        yield return new WaitForSeconds(boss.Beat/2);
    }

    private (Vector3Int startPos, Vector3Int direction) GetRandomDiagonalLine()
    {
        // 4개의 대각선 중 랜덤 선택
        List<(Vector3Int startPos, Vector3Int direction)> diagonalLines = new()
        {
            (new Vector3Int(1, 7, 0), new Vector3Int(1, -1, 0)),  // 왼쪽아래 -> 오른쪽위
            (new Vector3Int(1, 1, 0), new Vector3Int(1, 1, 0)),   // 왼쪽위 -> 오른쪽아래
            (new Vector3Int(7, 1, 0), new Vector3Int(-1, 1, 0)),  // 오른쪽위 -> 왼쪽아래
            (new Vector3Int(7, 7, 0), new Vector3Int(-1, -1, 0))  // 오른쪽아래 -> 왼쪽위
        };

        return diagonalLines[Random.Range(0, diagonalLines.Count)];
    }

    private List<Vector3Int> GetSwordShape(Vector3Int tip, Vector3Int dir)
    {
        List<Vector3Int> shape = new() { tip };

        // 대각선 방향에 따른 칼날 (3칸)
        for (int i = 1; i <= 3; i++)
        {
            Vector3Int bladePos = tip + dir * i;
            shape.Add(bladePos);
        }

        // 대각선 방향에 따른 가드 (십자 모양으로 확장)
        Vector3Int perpendicular1, perpendicular2;
        if (dir.x > 0 && dir.y > 0) // 우하향
        {
            perpendicular1 = new Vector3Int(1, 0, 0);   // 오른쪽
            perpendicular2 = new Vector3Int(0, 1, 0);   // 위
        }
        else if (dir.x > 0 && dir.y < 0) // 우상향
        {
            perpendicular1 = new Vector3Int(1, 0, 0);   // 오른쪽
            perpendicular2 = new Vector3Int(0, -1, 0);  // 아래
        }
        else if (dir.x < 0 && dir.y > 0) // 좌하향
        {
            perpendicular1 = new Vector3Int(-1, 0, 0);  // 왼쪽
            perpendicular2 = new Vector3Int(0, 1, 0);   // 위
        }
        else // 좌상향
        {
            perpendicular1 = new Vector3Int(-1, 0, 0);  // 왼쪽
            perpendicular2 = new Vector3Int(0, -1, 0);  // 아래
        }

        // 가드를 칼날 시작 부분에 십자 모양으로 추가
        Vector3Int guardCenter = tip + dir;
        shape.Add(guardCenter + perpendicular1);
        shape.Add(guardCenter + perpendicular2);
        shape.Add(guardCenter - perpendicular1);
        shape.Add(guardCenter - perpendicular2);

        // 손잡이 (팁 반대 방향으로 3칸, 십자 모양)
        for (int i = 1; i <= 3; i++)
        {
            Vector3Int handlePos = tip - dir * i;
            shape.Add(handlePos);
            
            // 손잡이 중간 부분에 십자 확장
            if (i == 2)
            {
                shape.Add(handlePos + perpendicular1);
                shape.Add(handlePos + perpendicular2);
                shape.Add(handlePos - perpendicular1);
                shape.Add(handlePos - perpendicular2);
            }
        }

        return shape;
    }

    private List<Vector3Int> GetThickDiagonalLine(Vector3Int center, Vector3Int dir)
    {
        List<Vector3Int> result = new() { center };

        // 대각선 방향의 3칸 굵기 처리
        if (dir.x != 0 && dir.y != 0)
        {
            // 대각선의 수직/수평 방향으로 확장
            result.Add(center + new Vector3Int(dir.x, 0, 0));
            result.Add(center + new Vector3Int(0, dir.y, 0));
        }

        return result;
    }

    private int GetMaxDiagonalLength()
    {
        // 9x9 격자에서 대각선의 최대 길이는 8
        return 8;
    }

    private bool IsValid(Vector3Int pos) =>
        pos.x >= 0 && pos.x < 9 && pos.y >= 0 && pos.y < 9;

    private IEnumerator SoundPlay()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.LastBossSoundClip("LastBossSwordAttackActivate");
    }
}