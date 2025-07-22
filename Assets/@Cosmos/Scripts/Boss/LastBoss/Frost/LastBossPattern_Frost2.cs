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
    public string PatternName => "10_4";

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
        
        float wallDuration = boss.Beat * 5; // 벽 지속시간

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

                // 중심점은 벽 + 이동불가 처리
                boss.BombHandler.ExecuteFixedBomb(
                    new() { Vector3Int.zero },
                    center,
                    _wallPrefab,
                    1f,
                    wallDuration,
                    0,
                    warningType:WarningType.Type3,
                    patternName:PatternName
                );
                
                // 벽이 생기는 시점(1초 후)에 Grid 막기
                boss.StartCoroutine(DelayedGridLock(center, 1f, wallDuration));

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
                            warningType:WarningType.Type1,
                            patternName:PatternName
                        );
                    }
                }
            }

            yield return new WaitForSeconds(boss.Beat);
        }
        
        yield return new WaitForSeconds(boss.Beat * 2);
        boss.Unstoppable = false;
    }

    private IEnumerator DelayedGridLock(Vector3Int pos, float lockDelay, float wallDuration)
    {
        // 벽이 생기는 시점에 Grid 막기
        yield return new WaitForSeconds(lockDelay);
        GridManager.Instance.AddUnmovableGridPosition(pos);
        
        // 벽이 사라지는 시점에 Grid 해제
        yield return new WaitForSeconds(wallDuration);
        GridManager.Instance.RemoveUnmovableGridPosition(pos);
    }

    private Vector3Int GetRandomPosition()
    {
        // 9x9 격자에서 랜덤한 위치 선택
        // 경계에서 너무 가까운 곳은 피하기 위해 1~7 범위 사용
        int x = Random.Range(1, 8);
        int y = Random.Range(1, 8);
        return new Vector3Int(x, y, 0);
    }

    private List<Vector3Int> GetSnowflakeShape(Vector3Int center)
    {
        List<Vector3Int> shape = new() { center }; // 중심점 명시적으로 추가

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