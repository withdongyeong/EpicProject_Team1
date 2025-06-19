using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오크 메이지 보스 돌진 패턴 - 오른쪽으로 이동하며 스파이크 공격
/// </summary>
public class OrcMagePatternBossChargeRight : IBossAttackPattern
{
    private GameObject _groundSpikePrefab;

    public string PatternName => "OrcMagePattern_BossChargeRight";

    public OrcMagePatternBossChargeRight(GameObject groundSpikePrefab)
    {
        _groundSpikePrefab = groundSpikePrefab;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _groundSpikePrefab != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 보스 돌진 패턴 실행 - EnemySpawnPosition2 → 랜덤 행 → EnemySpawnPosition
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        // 1. 랜덤 행 선택 (2~6행, 가장자리 피함)
        int targetRow = Random.Range(2, 7);
        Vector3Int startPosition = new Vector3Int(0, targetRow, 0); // 왼쪽 끝
        Vector3Int endPosition = new Vector3Int(8, targetRow, 0);   // 오른쪽 끝

        // 2. EnemySpawnPosition2에서 시작 지점으로 이동 (Walk 애니메이션)
        SoundManager.Instance.OrcMageSoundClip("OrcMage_ScreamActivate");
        boss.SetAnimationTrigger("Walk");
        yield return MoveBossToPosition(boss, startPosition);

        // 3. 도착지에 전조 + 피격 판정
        yield return ExecuteArrivalAttack(boss, startPosition);

        // 4. 행을 따라 오른쪽으로 이동하며 스파이크 공격
        yield return ExecuteChargeAttack(boss, startPosition, endPosition);

        // 5. EnemySpawnPosition으로 이동 (Walk 애니메이션)
        GameObject enemySpawn = GameObject.Find("EnemySpawnPosition");
        if (enemySpawn != null)
        {
            boss.SetAnimationTrigger("Walk");
            yield return MoveBossToWorldPosition(boss, enemySpawn.transform.position);
        }
        else
        {
            Debug.LogWarning("EnemySpawnPosition not found!");
        }

        // 6. 오른쪽 돌진 완료 후 flip
        FlipBoss(boss);
    }

    /// <summary>
    /// 보스를 특정 위치로 이동 (그리드 좌표)
    /// </summary>
    private IEnumerator MoveBossToPosition(BaseBoss boss, Vector3Int targetGridPos)
    {
        Vector3 targetWorldPos = boss.GridSystem.GridToWorldPosition(targetGridPos);
        yield return MoveBossToWorldPosition(boss, targetWorldPos);
    }

    /// <summary>
    /// 보스를 월드 좌표로 이동
    /// </summary>
    private IEnumerator MoveBossToWorldPosition(BaseBoss boss, Vector3 targetWorldPos)
    {
        float moveSpeed = 8f; // 이동 속도

        while (Vector3.Distance(boss.transform.position, targetWorldPos) > 0.1f)
        {
            boss.transform.position = Vector3.MoveTowards(boss.transform.position, targetWorldPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        boss.transform.position = targetWorldPos;
    }

    /// <summary>
    /// 도착 시 공격 (전조 + 피격 판정)
    /// </summary>
    private IEnumerator ExecuteArrivalAttack(BaseBoss boss, Vector3Int position)
    {
        List<Vector3Int> bossArea = GetBossArea(position);
        
        // 전조 → 데미지 (이펙트 없음)
        boss.BombHandler.ExecuteWarningThenDamage(bossArea, position, 0.8f, 30, WarningType.Type2);
        yield return new WaitForSeconds(0.8f);
    }

    /// <summary>
    /// 돌진 공격 실행 - 오른쪽으로 이동하며 스파이크
    /// </summary>
    private IEnumerator ExecuteChargeAttack(BaseBoss boss, Vector3Int start, Vector3Int end)
    {
        // 돌진 애니메이션 시작
        boss.SetAnimationTrigger("Attack1Hand");
        
        int stepCount = 0; // 스텝 카운터
        
        for (int x = start.x; x <= end.x; x++)
        {
            Vector3Int currentPos = new Vector3Int(x, start.y, 0);
            
            // 보스 이동
            SoundManager.Instance.OrcMageSoundClip("OrcMage_RunActivate");
            yield return MoveBossToPosition(boss, currentPos);
            
            // 보스 위치 공격 (전조 + 피격 판정)
            List<Vector3Int> bossArea = GetBossArea(currentPos);
            boss.BombHandler.ExecuteWarningThenDamage(bossArea, currentPos, 0.8f, 25, WarningType.Type2);
            
            // 스파이크 공격 (번갈아가며)
            yield return ExecuteAlternatingSpike(boss, currentPos, stepCount);
            
            stepCount++;
            yield return new WaitForSeconds(0.1f); // 빠른 이동
        }
    }

    /// <summary>
    /// 번갈아가는 스파이크 공격 - | -> \/ -> | -> \/
    /// </summary>
    private IEnumerator ExecuteAlternatingSpike(BaseBoss boss, Vector3Int centerPos, int stepCount)
    {
        Vector3Int[] spikeDirections;
        
        // 스텝에 따라 번갈아가며 패턴 선택
        if (stepCount % 2 == 0)
        {
            // 짝수 스텝: | 패턴 (수직)
            spikeDirections = new Vector3Int[] {
                new Vector3Int(0, 1, 0),   // ↑ 수직
                new Vector3Int(0, -1, 0)   // ↓ 수직
            };
        }
        else
        {
            // 홀수 스텝: \/ 패턴 (대각선)
            spikeDirections = new Vector3Int[] {
                new Vector3Int(-1, 1, 0),  // ↖ 대각선
                new Vector3Int(1, 1, 0),   // ↗ 대각선
                new Vector3Int(-1, -1, 0), // ↙ 대각선  
                new Vector3Int(1, -1, 0)   // ↘ 대각선
            };
        }

        boss.StartCoroutine(boss.PlayOrcExplosionSoundDelayed("OrcMage_SpikeActivate", 0.8f));
        foreach (Vector3Int direction in spikeDirections)
        {
            List<Vector3Int> spikeLine = CreateSpikeLine(centerPos, direction);
            if (spikeLine.Count > 0)
            {
                boss.BombHandler.ExecuteFixedBomb(spikeLine, centerPos, _groundSpikePrefab,
                                                  warningDuration: 0.8f, explosionDuration: 1f, damage: 20, WarningType.Type1);
            }
        }
        
        yield return new WaitForSeconds(0.2f); // 스파이크 발동 대기
    }

    /// <summary>
    /// 특정 방향으로 스파이크 라인 생성 (격자 끝까지)
    /// </summary>
    private List<Vector3Int> CreateSpikeLine(Vector3Int start, Vector3Int direction)
    {
        List<Vector3Int> line = new List<Vector3Int>();
        
        // 격자 끝까지 계속 확장
        for (int i = 1; i <= 8; i++) // 최대 8칸까지 (9x9 그리드)
        {
            Vector3Int worldPos = start + (direction * i);
            if (IsValidGridPosition(worldPos))
            {
                line.Add(direction * i); // 상대 좌표로 저장
            }
            else
            {
                break; // 격자 밖으로 나가면 중단
            }
        }
        
        return line;
    }

    /// <summary>
    /// 보스 영역 계산 (3x3 정사각형)
    /// </summary>
    private List<Vector3Int> GetBossArea(Vector3Int center)
    {
        return new List<Vector3Int>
        {
            new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0), // 아래
            new Vector3Int(-1,  0, 0), new Vector3Int(0,  0, 0), new Vector3Int(1,  0, 0), // 중간
            new Vector3Int(-1,  1, 0), new Vector3Int(0,  1, 0), new Vector3Int(1,  1, 0)  // 위
        };
    }

    /// <summary>
    /// 보스 좌우 반전 (SpriteRenderer flipX 사용)
    /// </summary>
    private void FlipBoss(BaseBoss boss)
    {
        SpriteRenderer spriteRenderer = boss.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX; // 현재 상태 반전
        }
        else
        {
            Debug.LogWarning("SpriteRenderer component not found on boss!");
        }
    }

    /// <summary>
    /// 유효한 그리드 위치 확인
    /// </summary>
    private bool IsValidGridPosition(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < 9 && pos.y >= 0 && pos.y < 9;
    }
}