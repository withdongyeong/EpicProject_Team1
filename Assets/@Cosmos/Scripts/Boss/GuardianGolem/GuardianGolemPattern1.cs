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

        int wallCount = boss.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;
        Vector3Int centerPos = new Vector3Int(4, 4, 0);

        // 모든 공격 위치를 하나의 리스트로 통합
        List<Vector3Int> allAttackPositions = new List<Vector3Int>();

        for (int y = 0; y < 9; y++)
        {
            if (_isOdd && y % 2 == 0) continue;
            if (!_isOdd && y % 2 != 0) continue;

            for (int x = wallCount; x <= 8 - wallCount; x++)
            {
                allAttackPositions.Add(new Vector3Int(4 - x, 4 - y, 0));
            }
        }

        boss.AttackAnimation();

        // 모든 위치를 동시에 공격
        if (allAttackPositions.Count > 0)
        {
            boss.BombHandler.ExecuteFixedBomb(
                allAttackPositions,
                centerPos,
                _guardianGolemRook,
                warningDuration: 1f,
                explosionDuration: 0.7f,
                damage: _damage
            );

            boss.StartCoroutine(DelayedSound(1f));
            yield return new WaitForSeconds(beat * 2); // 2비트 대기
        }
    }

    private IEnumerator DelayedSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.GolemSoundClip("GolemAttackActivate");
    }
}