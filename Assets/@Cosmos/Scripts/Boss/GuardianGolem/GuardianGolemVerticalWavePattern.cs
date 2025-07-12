using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardianGolemVerticalWavePattern : IBossAttackPattern
{
    private GameObject _guardianGolemRook;
    private int Wallcount;
    private int _damage;

    private float _lastSoundTime = -10f; // ⬅️ 사운드 쿨다운 기준 시간

    public string PatternName => "GuardianGolemTemporaryWallSummonPattern";

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
        Wallcount = boss.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;
        int RandomPoint = Random.Range(Wallcount, 4);

        for (int x = 8 - Wallcount; x >= Wallcount; x--)
        {
            if (RandomPoint == x) continue;
            boss.StartCoroutine(ExecuteColumnAttack(boss, x));
            yield return new WaitForSeconds(boss.Beat);
        }
    }

    private IEnumerator ExecuteColumnAttack(BaseBoss boss, int x)
    {
        boss.AttackAnimation();
        float halfBeat = boss.HalfBeat;

        for (int y = 0; y < 9; y++)
        {
            boss.BombHandler.ExecuteFixedBomb(
                new List<Vector3Int> { Vector3Int.zero },
                new Vector3Int(x, y, 0),
                _guardianGolemRook,
                warningDuration: 1f,
                explosionDuration: 0.7f,
                damage: _damage
            );

            boss.StartCoroutine(DelayedAttackSound(1f));
            yield return new WaitForSeconds(halfBeat);
        }
    }

    private IEnumerator ExecuteRowWaveAttack(BaseBoss boss)
    {
        Wallcount = boss.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;
        int RandomPoint = Random.Range(5, 9);

        for (int y = 0; y < 9; y++)
        {
            if (y == RandomPoint) continue;
            boss.StartCoroutine(ExecuteRow(boss, y));
            yield return new WaitForSeconds(boss.Beat);
        }
    }

    private IEnumerator ExecuteRow(BaseBoss boss, int y)
    {
        boss.AttackAnimation();
        float halfBeat = boss.HalfBeat;

        for (int x = Wallcount; x < 9 - Wallcount; x++)
        {
            boss.BombHandler.ExecuteFixedBomb(
                new List<Vector3Int> { Vector3Int.zero },
                new Vector3Int(x, y, 0),
                _guardianGolemRook,
                warningDuration: 1f,
                explosionDuration: 0.7f,
                damage: _damage
            );

            boss.StartCoroutine(DelayedAttackSound(1f));
            yield return new WaitForSeconds(halfBeat);
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
