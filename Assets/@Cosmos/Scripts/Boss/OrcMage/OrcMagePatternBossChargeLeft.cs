using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오크 메이지 보스 돌진 패턴 - 왼쪽으로 이동하며 스파이크 공격
/// </summary>
public class OrcMagePatternBossChargeLeft : IBossAttackPattern
{
    private GameObject _groundSpikePrefab;
    private int _damage;
    private float beat;

    public string PatternName => "OrcMagePattern_BossChargeLeft";

    public OrcMagePatternBossChargeLeft(GameObject groundSpikePrefab, int damage)
    {
        _groundSpikePrefab = groundSpikePrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _groundSpikePrefab != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 보스 돌진 패턴 실행 - EnemySpawnPosition → 랜덤 행 → EnemySpawnPosition2
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        beat = boss.Beat;
        // 빙결 불가 설정
        boss.Unstoppable = true;
        
        // 1. 랜덤 행 선택 (2~6행, 가장자리 피함)
        int targetRow = Random.Range(2, 7);
        Vector3Int startPosition = new Vector3Int(8, targetRow, 0); // 오른쪽 끝
        Vector3Int endPosition = new Vector3Int(0, targetRow, 0);   // 왼쪽 끝

        // 2. EnemySpawnPosition에서 시작 지점으로 이동 (Walk 애니메이션)
        boss.SetAnimationTrigger("Walk");
        SoundManager.Instance.OrcMageSoundClip("OrcMage_ScreamActivate");
        yield return MoveBossToPosition(boss, startPosition);

        // 3. 도착지에 전조 + 피격 판정
        yield return ExecuteArrivalAttack(boss, startPosition);

        // 4. 행을 따라 왼쪽으로 돌진하며 웨이브 공격 (이동과 후폭풍 스파이크 포함)
        yield return ExecuteChargeAttackWithWave(boss, startPosition, endPosition);
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
        float duration = 1f;
        float elapsed = 0f;

        Vector3 startPos = boss.transform.position;

        while (elapsed < duration)
        {
            float t = elapsed / duration; // 0~1
            boss.transform.position = Vector3.Lerp(startPos, targetWorldPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 마지막에 정확히 위치 고정
        boss.transform.position = targetWorldPos;
    }

    /// <summary>
    /// 도착 시 공격 (전조 + 피격 판정)
    /// </summary>
    private IEnumerator ExecuteArrivalAttack(BaseBoss boss, Vector3Int position)
    {
        List<Vector3Int> bossArea = GetBossArea(position);
        
        // 전조 → 데미지 (이펙트 없음)
        boss.BombHandler.ExecuteWarningThenDamage(bossArea, position, 1f, _damage, WarningType.Type2);
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// 돌진 공격 실행 - 슈욱 지나가면서 웨이브 전조 + 후폭풍 스파이크
    /// </summary>
    private IEnumerator ExecuteChargeAttackWithWave(BaseBoss boss, Vector3Int start, Vector3Int end)
    {
        boss.SetAnimationTrigger("Attack1Hand");

        // 1단계: 보스 이동과 웨이브 전조를 동시에 시작
        Coroutine chargeMovement = boss.StartCoroutine(ExecuteChargeMovementWithWave(boss, start, end));
        
        // 2단계: 돌진 중간에 후폭풍 스파이크 시작
        yield return new WaitForSeconds(beat * 2);
        boss.StartCoroutine(ExecuteAfterShockSpikes(boss, start, end));
        
        // 돌진 이동이 완료될 때까지 대기
        yield return chargeMovement;

        // 보스는 바로 EnemySpawnPosition2로 이동
        GameObject enemySpawn2 = GameObject.Find("EnemySpawnPosition2");
        if (enemySpawn2 != null)
        {
            boss.SetAnimationTrigger("Walk");
            yield return MoveBossToWorldPosition(boss, enemySpawn2.transform.position);
        }
        else
        {
            Debug.LogWarning("EnemySpawnPosition2 not found!");
        }

        // 좌우 반전
        FlipBoss(boss);
    }

    /// <summary>
    /// 보스 돌진 이동 + 웨이브 전조
    /// </summary>
    private IEnumerator ExecuteChargeMovementWithWave(BaseBoss boss, Vector3Int start, Vector3Int end)
    {
        SoundManager.Instance.OrcMageSoundClip("OrcMage_RunActivate");
        
        // 1비트 대기 후 시작
        yield return new WaitForSeconds(beat);
        
        // 보스 이동 시작
        Vector3 startWorldPos = boss.GridSystem.GridToWorldPosition(start);
        Vector3 endWorldPos = boss.GridSystem.GridToWorldPosition(end);
        
        // 이동 경로의 모든 위치를 미리 계산
        List<Vector3Int> chargePath = new List<Vector3Int>();
        for (int x = start.x; x >= end.x; x--)
        {
            chargePath.Add(new Vector3Int(x, start.y, 0));
        }

        // 웨이브 전조를 순차적으로 표시하면서 보스 이동
        float moveDuration = beat * 2f; // 2비트 동안 이동
        float waveInterval = beat / 4f; // 1/4 비트 간격으로 웨이브
        
        // 웨이브 전조 코루틴 시작
        boss.StartCoroutine(ShowChargeWave(boss, chargePath, waveInterval));
        
        // 보스 이동 (동시에 진행)
        yield return MoveBossToWorldPosition(boss, endWorldPos, moveDuration);
    }

/// <summary>
    /// 이동 방향에 따른 전조 영역 계산 (겹침 방지)
    /// </summary>
    private List<Vector3Int> GetMovementWarningArea(Vector3Int direction)
    {
        List<Vector3Int> warningArea = new List<Vector3Int>();
        
        if (direction.x < 0) // 왼쪽 이동
        {
            warningArea.Add(new Vector3Int(-1, -1, 0));
            warningArea.Add(new Vector3Int(-1,  0, 0));
            warningArea.Add(new Vector3Int(-1,  1, 0));
        }
        else if (direction.x > 0) // 오른쪽 이동
        {
            warningArea.Add(new Vector3Int(1, -1, 0));
            warningArea.Add(new Vector3Int(1,  0, 0));
            warningArea.Add(new Vector3Int(1,  1, 0));
        }
        else if (direction.y < 0) // 아래쪽 이동
        {
            warningArea.Add(new Vector3Int(-1, -1, 0));
            warningArea.Add(new Vector3Int( 0, -1, 0));
            warningArea.Add(new Vector3Int( 1, -1, 0));
        }
        else if (direction.y > 0) // 위쪽 이동
        {
            warningArea.Add(new Vector3Int(-1, 1, 0));
            warningArea.Add(new Vector3Int( 0, 1, 0));
            warningArea.Add(new Vector3Int( 1, 1, 0));
        }
        
        return warningArea;
    }

    /// <summary>
    /// 돌진 경로에 웨이브 전조 표시
    /// </summary>
    private IEnumerator ShowChargeWave(BaseBoss boss, List<Vector3Int> chargePath, float waveInterval)
    {
        for (int i = 0; i < chargePath.Count; i++)
        {
            Vector3Int pos = chargePath[i];
            List<Vector3Int> warningArea;
            
            if (i == 0)
            {
                // 첫 번째 위치는 전체 3x3 영역
                warningArea = GetBossArea(pos);
            }
            else
            {
                // 이후 위치는 이동 방향의 새로운 영역만
                Vector3Int direction = chargePath[i] - chargePath[i-1];
                warningArea = GetMovementWarningArea(direction);
            }
            
            // 웨이브 전조 + 데미지
            boss.BombHandler.ExecuteWarningThenDamage(warningArea, pos, 1f, _damage, WarningType.Type2);
            
            yield return new WaitForSeconds(waveInterval);
        }
    }

    /// <summary>
    /// 보스를 월드 좌표로 이동 (시간 지정)
    /// </summary>
    private IEnumerator MoveBossToWorldPosition(BaseBoss boss, Vector3 targetWorldPos, float duration)
    {
        float elapsed = 0f;
        Vector3 startPos = boss.transform.position;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            boss.transform.position = Vector3.Lerp(startPos, targetWorldPos, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        boss.transform.position = targetWorldPos;
    }

    /// <summary>
    /// 후폭풍 스파이크 공격 - 1비트 단위로 순차 시작
    /// </summary>
    private IEnumerator ExecuteAfterShockSpikes(BaseBoss boss, Vector3Int start, Vector3Int end)
    {
        // 돌진 경로의 각 위치에서 스파이크 공격 시작
        for (int x = start.x; x >= end.x; x--)
        {
            Vector3Int currentPos = new Vector3Int(x, start.y, 0);
            int stepCount = start.x - x; // 스텝 카운트 계산
            
            // 각 위치에서 스파이크 공격을 백그라운드에서 시작
            boss.StartCoroutine(ExecuteWaveSpikeAttack(boss, currentPos, stepCount));
            
            //
            yield return new WaitForSeconds(beat / 2);
        }
    }

    /// <summary>
    /// 웨이브 스파이크 공격 - 1줄씩 순차적으로 뻗어나감
    /// </summary>
    private IEnumerator ExecuteWaveSpikeAttack(BaseBoss boss, Vector3Int centerPos, int stepCount)
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

        // 가운데(보스 위치)에 스파이크 공격 먼저 실행
        List<Vector3Int> centerSpike = new List<Vector3Int> { new Vector3Int(0, 0, 0) };
        boss.BombHandler.ExecuteFixedBomb(centerSpike, centerPos, _groundSpikePrefab,
                                          warningDuration: 1f, explosionDuration: 1f, damage: _damage, WarningType.Type1);
        
        boss.StartCoroutine(boss.PlayOrcExplosionSoundDelayed("OrcMage_SpikeActivate", 1f));

        // 각 방향으로 웨이브 스파이크 발사
        foreach (Vector3Int direction in spikeDirections)
        {
            boss.StartCoroutine(ExecuteDirectionalWaveSpike(boss, centerPos, direction));
        }
        
        yield return null;
    }

    /// <summary>
    /// 특정 방향으로 웨이브 스파이크 발사
    /// </summary>
    private IEnumerator ExecuteDirectionalWaveSpike(BaseBoss boss, Vector3Int centerPos, Vector3Int direction)
    {
        // 해당 방향의 모든 스파이크 위치 계산
        List<Vector3Int> spikeLine = CreateSpikeLine(centerPos, direction);
        
        if (spikeLine.Count == 0) 
        {
            yield break;
        }

        // 1줄씩 웨이브로 뻗어나가기
        for (int wave = 0; wave < spikeLine.Count; wave++)
        {
            List<Vector3Int> currentWave = new List<Vector3Int> { spikeLine[wave] };
            
            boss.BombHandler.ExecuteFixedBomb(currentWave, centerPos, _groundSpikePrefab,
                                              warningDuration: 1f, explosionDuration: 1f, damage: _damage, WarningType.Type1);
            
            // 1/4 비트 간격으로 다음 웨이브
            yield return new WaitForSeconds(beat / 4f);
        }
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