using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GuardianGolemPattern2 : IBossAttackPattern
{
    private GameObject _guardianGolemRook;
    public string PatternName => "4_2";
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

        // 가운데 세로줄(x=4)에서 2칸마다 격자무늬 안전영역 제거
        List<Vector3Int> safePositions = new List<Vector3Int>();
        for (int y = 0; y < 9; y++)
        {
            if (y % 2 == 0) // 짝수 행에서 가운데 세로줄 안전영역
            {
                safePositions.Add(new Vector3Int(0, 4 - y, 0)); // 중심좌표 기준
            }
        }

        // 안전영역에 해당하는 공격 위치들을 제거
        foreach (var safePos in safePositions)
        {
            allAttackPositions.Remove(safePos);
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
                damage: _damage,
                patternName:PatternName
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