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
        int deleteCount = boss.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;
        Vector3Int centerPos = new Vector3Int(4, 4, 0);

        // 모든 공격 위치를 하나의 리스트로 통합
        List<Vector3Int> allAttackPositions = new List<Vector3Int>();

        // 대각선 (↘ / ↙)
        for (int y = deleteCount; y <= 8 - deleteCount; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (x == y || x + y == 8)
                    allAttackPositions.Add(new Vector3Int(4 - x, 4 - y, 0));
            }
        }

        // 상단 바깥쪽
        for (int y = 0; y < deleteCount; y++)
        {
            for (int x = deleteCount; x <= 8 - deleteCount; x++)
                allAttackPositions.Add(new Vector3Int(4 - x, 4 - y, 0));
        }

        // 하단 바깥쪽
        for (int y = 8; y > 8 - deleteCount; y--)
        {
            for (int x = deleteCount; x <= 8 - deleteCount; x++)
                allAttackPositions.Add(new Vector3Int(4 - x, 4 - y, 0));
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

            boss.StartCoroutine(DelayedSound());
            yield return new WaitForSeconds(beat * 2); // 2비트 대기
        }
    }

    private IEnumerator DelayedSound()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.GolemSoundClip("GolemAttackActivate");
    }
}