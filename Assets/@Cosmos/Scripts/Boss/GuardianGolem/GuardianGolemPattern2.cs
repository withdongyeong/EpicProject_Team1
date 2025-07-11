using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GuardianGolemPattern2 : IBossAttackPattern
{
    private GameObject _guardianGolemRook;
    public string PatternName => "GuardianGolemPattern2";
    private int _damage;

    public GuardianGolemPattern2(GameObject guardianGolemRook, int damage)
    {
        _guardianGolemRook = guardianGolemRook;
        _damage = damage;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(GuardianGolemPattern(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler != null &&
               boss.BombHandler.PlayerController != null &&
               _guardianGolemRook != null;
    }

    private IEnumerator GuardianGolemPattern(BaseBoss boss)
    {
        float beat = boss.Beat;
        float halfBeat = boss.HalfBeat;
        int deleteCount = boss.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;
        Vector3Int centerPos = new Vector3Int(4, 4, 0);

        // 웨이브 단위로 분리
        List<List<Vector3Int>> waves = new();

        // 대각선 (↘ / ↙)
        for (int y = deleteCount; y <= 8 - deleteCount; y++)
        {
            List<Vector3Int> wave = new();
            for (int x = 0; x < 9; x++)
            {
                if (x == y || x + y == 8)
                    wave.Add(new Vector3Int(4 - x, 4 - y, 0));
            }
            if (wave.Count > 0)
                waves.Add(wave);
        }

        // 상단 바깥쪽
        for (int y = 0; y < deleteCount; y++)
        {
            List<Vector3Int> wave = new();
            for (int x = deleteCount; x <= 8 - deleteCount; x++)
                wave.Add(new Vector3Int(4 - x, 4 - y, 0));
            if (wave.Count > 0)
                waves.Add(wave);
        }

        // 하단 바깥쪽
        for (int y = 8; y > 8 - deleteCount; y--)
        {
            List<Vector3Int> wave = new();
            for (int x = deleteCount; x <= 8 - deleteCount; x++)
                wave.Add(new Vector3Int(4 - x, 4 - y, 0));
            if (wave.Count > 0)
                waves.Add(wave);
        }

        boss.AttackAnimation();

        foreach (var wave in waves)
        {
            boss.BombHandler.ExecuteFixedBomb(
                wave,
                centerPos,
                _guardianGolemRook,
                warningDuration: 1f,
                explosionDuration: 0.7f,
                damage: _damage
            );

            boss.StartCoroutine(DelayedSound());
            yield return new WaitForSeconds(halfBeat);
        }

        float total = halfBeat * waves.Count;
        float rounded = Mathf.Ceil(total / beat) * beat;
        float remainder = rounded - total;
        yield return new WaitForSeconds(remainder);
    }

    private IEnumerator DelayedSound()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.GolemSoundClip("GolemAttackActivate");
    }
}
