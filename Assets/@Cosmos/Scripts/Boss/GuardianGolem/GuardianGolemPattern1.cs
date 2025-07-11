using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GuardianGolemPattern1 : IBossAttackPattern
{
    private GameObject _guardianGolemRook;
    private bool _isOdd;
    private int _damage;

    public string PatternName => "GuardianGolemPattern1";

    public GuardianGolemPattern1(GameObject guardianGolemRook, bool IsOdd, int damage)
    {
        _guardianGolemRook = guardianGolemRook;
        _isOdd = IsOdd;
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

        int wallCount = boss.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;
        Vector3Int centerPos = new Vector3Int(4, 4, 0);

        List<List<Vector3Int>> waves = new();

        for (int y = 0; y < 9; y++)
        {
            if (_isOdd && y % 2 == 0) continue;
            if (!_isOdd && y % 2 != 0) continue;

            List<Vector3Int> row = new();
            for (int x = wallCount; x <= 8 - wallCount; x++)
            {
                row.Add(new Vector3Int(4 - x, 4 - y, 0));
            }
            if (row.Count > 0)
                waves.Add(row);
        }

        boss.AttackAnimation();

        for (int i = 0; i < waves.Count; i++)
        {
            boss.BombHandler.ExecuteFixedBomb(
                waves[i],
                centerPos,
                _guardianGolemRook,
                warningDuration: 1f,
                explosionDuration: 0.7f,
                damage: _damage
            );

            boss.StartCoroutine(DelayedSound(1f));
            yield return new WaitForSeconds(halfBeat);
        }

        // 정렬
        float total = halfBeat * waves.Count;
        float rounded = Mathf.Ceil(total / beat) * beat;
        float remainder = rounded - total;
        yield return new WaitForSeconds(remainder);
    }

    private IEnumerator DelayedSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.GolemSoundClip("GolemAttackActivate");
    }
}
