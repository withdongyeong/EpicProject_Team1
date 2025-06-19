using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오크 메이지 나선형 패턴 - 중심에서 바깥으로 나선
/// </summary>
public class OrcMagePatternSpiral : IBossAttackPattern
{
    private GameObject _groundSpikePrefab;

    public string PatternName => "OrcMagePattern_Spiral";

    public OrcMagePatternSpiral(GameObject groundSpikePrefab)
    {
        _groundSpikePrefab = groundSpikePrefab;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _groundSpikePrefab != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 나선형 패턴 실행 - 중심에서 바깥으로 나선 (전체 그리드 채우기)
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack1");

        Vector3Int center = new Vector3Int(4, 4, 0);
        List<Vector3Int> spiralPositions = GenerateSpiralPositions(center);

        // 나선 순서대로 공격 (4개씩 묶어서)
        for (int i = 0; i < spiralPositions.Count; i += 4)
        {
            List<Vector3Int> singlePoint = new List<Vector3Int> { Vector3Int.zero };
            
            boss.StartCoroutine(boss.PlayOrcExplosionSoundDelayed("OrcMage_SpikeActivate", 0.8f));
            // 4개씩 그룹 동시 실행
            for (int j = 0; j < 4 && i + j < spiralPositions.Count; j++)
            {
                boss.BombHandler.ExecuteFixedBomb(singlePoint, spiralPositions[i + j], _groundSpikePrefab,
                                                  warningDuration: 0.8f, explosionDuration: 1f, damage: 20, WarningType.Type1);
            }

            // 첫 번째 그룹만 0.3초, 나머지는 0.15초로 빠르게
            if (i == 0)
            {
                yield return new WaitForSeconds(0.3f); // 첫 번째만 여유
            }
            else
            {
                yield return new WaitForSeconds(0.15f); // 나머지는 빠르게
            }
        }
    }

    /// <summary>
    /// 나선형 위치 생성 - 전체 그리드 채우기
    /// </summary>
    private List<Vector3Int> GenerateSpiralPositions(Vector3Int center)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        bool[,] visited = new bool[9, 9]; // 9x9 그리드
        
        // 나선형 방향: 오른쪽 → 아래 → 왼쪽 → 위 → 반복
        Vector3Int[] directions = {
            new Vector3Int(1, 0, 0),   // 오른쪽
            new Vector3Int(0, -1, 0),  // 아래
            new Vector3Int(-1, 0, 0),  // 왼쪽
            new Vector3Int(0, 1, 0)    // 위
        };

        Vector3Int current = center;
        positions.Add(current);
        visited[current.x, current.y] = true;

        int steps = 1;
        int directionIndex = 0;

        // 전체 그리드를 채울 때까지 반복
        while (positions.Count < 81) // 9x9 = 81칸
        {
            for (int repeat = 0; repeat < 2; repeat++) // 같은 방향으로 2번
            {
                for (int step = 0; step < steps; step++)
                {
                    Vector3Int next = current + directions[directionIndex];
                    
                    if (IsValidPosition(next) && !visited[next.x, next.y])
                    {
                        current = next;
                        positions.Add(current);
                        visited[current.x, current.y] = true;
                    }
                    else
                    {
                        // 더 이상 진행할 수 없으면 종료
                        return positions;
                    }
                }
                directionIndex = (directionIndex + 1) % 4;
            }
            steps++;
        }

        return positions;
    }

    /// <summary>
    /// 유효한 그리드 위치인지 확인
    /// </summary>
    private bool IsValidPosition(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < 9 && pos.y >= 0 && pos.y < 9;
    }
}