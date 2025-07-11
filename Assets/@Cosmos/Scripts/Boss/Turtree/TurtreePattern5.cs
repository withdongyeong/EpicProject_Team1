using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TurtreePattern5 : IBossAttackPattern
{
    private GameObject _attackPrefab;
    private int _damage;
    
    public string PatternName => "TurtreePattern5";

    public TurtreePattern5(GameObject AttackPrefab, int Damage)
    {
        _attackPrefab = AttackPrefab;
        _damage = Damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _attackPrefab != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 나선형 패턴 실행 - 중심에서 바깥으로 나선 (전체 그리드 채우기)
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        boss.AttackAnimation();

        Vector3Int center = new Vector3Int(4, 4, 0);
        List<Vector3Int> spiralPositions = GenerateSpiralPositions(center);

        float halfBeat = boss.HalfBeat;
        List<Vector3Int> singlePoint = new List<Vector3Int> { Vector3Int.zero };

        for (int i = 0; i < spiralPositions.Count; i += 4)
        {
            for (int j = 0; j < 4 && i + j < spiralPositions.Count; j++)
            {
                boss.BombHandler.ExecuteFixedBomb(
                    singlePoint,
                    spiralPositions[i + j],
                    _attackPrefab,
                    warningDuration: 1f,
                    explosionDuration: 2f,
                    damage: _damage,
                    warningType: WarningType.Type1
                );
            }

            boss.StartCoroutine(TurtreeAttackSound());

            yield return new WaitForSeconds(halfBeat);
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
            for (int repeat = 0; repeat < 4; repeat++)
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

    private IEnumerator TurtreeAttackSound()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.TurtreeSoundClip("TurtreeAttackActivate");
    }
}