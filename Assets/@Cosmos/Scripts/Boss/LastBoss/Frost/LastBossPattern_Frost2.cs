using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종보스 - Frost2: 랜덤한 지역에 눈꽃 모양으로 얼음 벽 생성 (중심점은 이동불가)
/// </summary>
public class LastBossPattern_Frost2 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    private GameObject _wallPrefab;
    private int _damage;
    public string PatternName => "Frost2";

    public LastBossPattern_Frost2(GameObject explosionPrefab, GameObject wallPrefab, int damage)
    {
        _explosionPrefab = explosionPrefab;
        _wallPrefab = wallPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null && _wallPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");

        HashSet<Vector3Int> usedCenters = new HashSet<Vector3Int>();
        List<Vector3Int> unmovablePositions = new List<Vector3Int>();

        for (int i = 0; i < 10; i++)
        {
            Vector3Int center = GetRandomPosition();
            
            // 중심점이 이미 사용되었다면 다른 위치 찾기
            int attempts = 0;
            while (usedCenters.Contains(center) && attempts < 20)
            {
                center = GetRandomPosition();
                attempts++;
            }
            
            if (!usedCenters.Contains(center))
            {
                usedCenters.Add(center);
                
                boss.StartCoroutine(SoundPlay());

                List<Vector3Int> snowflakeShape = GetSnowflakeShape(center);

                // 중심점은 이동불가 처리
                boss.BombHandler.ShowWarningOnly(
                    center,
                    1f,
                    WarningType.Type3
                );
                GridManager.Instance.AddUnmovableGridPosition(center);
                unmovablePositions.Add(center);

                // 나머지 눈꽃 부분은 일반 공격
                foreach (var pos in snowflakeShape)
                {
                    if (IsValid(pos) && pos != center) // 중심점 제외
                    {
                        boss.BombHandler.ExecuteFixedBomb(
                            new() { Vector3Int.zero },
                            pos,
                            _explosionPrefab,
                            1f,
                            0.8f,
                            _damage,
                            WarningType.Type1
                        );
                    }
                }
            }

            yield return new WaitForSeconds(boss.Beat / 2); // 기존 beat/4 * 2
        }

        yield return new WaitForSeconds(boss.Beat);

        // 패턴 종료 시 모든 이동불가 위치 해제
        foreach (var pos in unmovablePositions)
        {
            GridManager.Instance.RemoveUnmovableGridPosition(pos);
        }
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

        // 십자 방향 (거리 2)
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

    private IEnumerator SoundPlay()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.LastBossSoundClip("LastBossFrostAttackActivate");
    }
}