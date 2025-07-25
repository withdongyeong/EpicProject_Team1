using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardianGolemVerticalWavePattern : IBossAttackPattern
{
    private GameObject _guardianGolemRook;
    private int Wallcount;
    private int _damage;

    private float _lastSoundTime = -10f; // ⬅️ 사운드 쿨다운 기준 시간

    public string PatternName => "4_4";

    public GuardianGolemVerticalWavePattern(GameObject GuardianGolemRook, int damage)
    {
        _guardianGolemRook = GuardianGolemRook;
        _damage = damage;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(ExecuteRowWaveAttack(boss));
        yield return boss.StartCoroutine(ExecuteSpiderWebAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler.PlayerController != null &&
               _guardianGolemRook != null &&
               boss.BombHandler != null;
    }

    private IEnumerator ExecuteSpiderWebAttack(BaseBoss boss)
    {
        if (!boss.IsHandBoss)
        {
            Wallcount = boss.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;    
        }
        
        // 공격 가능한 세로줄 범위 계산
        int leftBound = Wallcount;
        int rightBound = 8 - Wallcount;
        int availableColumns = rightBound - leftBound + 1;
        
        // 세로줄이 1줄만 남았을 때는 공격하지 않음 (전체 안전지대)
        if (availableColumns <= 1)
        {
            yield break;
        }
        
        // 안전지대 선택 범위 (가장 왼쪽, 오른쪽 제외)
        int safeRangeLeft = leftBound + 1;
        int safeRangeRight = rightBound - 1;
        int RandomPoint;
        
        // 안전 범위가 유효한지 확인
        if (safeRangeLeft <= safeRangeRight)
        {
            RandomPoint = Random.Range(safeRangeLeft, safeRangeRight + 1);
        }
        else
        {
            // 안전 범위가 없으면 중간값 사용
            RandomPoint = (leftBound + rightBound) / 2;
        }

        for (int x = rightBound; x >= leftBound; x--)
        {
            if (RandomPoint == x) continue;
            boss.StartCoroutine(ExecuteColumnAttack(boss, x));
            yield return new WaitForSeconds(boss.Beat / 2);
        }
    }

    private IEnumerator ExecuteColumnAttack(BaseBoss boss, int x)
    {
        boss.AttackAnimation();

        for (int y = 0; y < 9; y++)
        {
            boss.BombHandler.ExecuteFixedBomb(
                new List<Vector3Int> { Vector3Int.zero },
                new Vector3Int(x, y, 0),
                _guardianGolemRook,
                warningDuration: 1f,
                explosionDuration: 0.7f,
                damage: _damage,
                patternName:PatternName
            );

            boss.StartCoroutine(DelayedAttackSound(1f));
            yield return new WaitForSeconds(boss.Beat / 4);
        }
    }

    private IEnumerator ExecuteRowWaveAttack(BaseBoss boss)
    {
        if (!boss.IsHandBoss)
        {
            Wallcount = boss.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;    
        }
        int RandomPoint = Random.Range(5, 9);

        for (int y = 0; y < 9; y++)
        {
            if (y == RandomPoint) continue;
            boss.StartCoroutine(ExecuteRow(boss, y));
            yield return new WaitForSeconds(boss.Beat / 2);
        }
    }

    private IEnumerator ExecuteRow(BaseBoss boss, int y)
    {
        boss.AttackAnimation();

        for (int x = Wallcount; x < 9 - Wallcount; x++)
        {
            boss.BombHandler.ExecuteFixedBomb(
                new List<Vector3Int> { Vector3Int.zero },
                new Vector3Int(x, y, 0),
                _guardianGolemRook,
                warningDuration: 1f,
                explosionDuration: 0.7f,
                damage: _damage,
                patternName:PatternName
            );

            boss.StartCoroutine(DelayedAttackSound(1f));
            yield return new WaitForSeconds(boss.Beat / 4);
        }
    }

    private IEnumerator DelayedAttackSound(float delay)
    {
        yield return new WaitForSeconds(delay);

        float now = Time.unscaledTime;
        float minInterval = 0.15f; // 중복 방지용 최소 간격

        if (now - _lastSoundTime >= minInterval)
        {
            _lastSoundTime = now;
            SoundManager.Instance.GolemSoundClip("GolemAttackActivate");
        }
    }
}