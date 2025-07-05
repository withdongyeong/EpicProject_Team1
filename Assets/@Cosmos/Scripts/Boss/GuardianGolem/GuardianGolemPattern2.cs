using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GuardianGolemPattern2 : IBossAttackPattern
{
    private GameObject _guardianGolemRook;
    public string PatternName => "GuardianGolemPattern1";

    /// <summary>
    /// 가디언 골렘 패턴1 생성자
    /// </summary>
    /// <param name="poisionAriaPrefab">독 이펙트 프리팹</param>
    public GuardianGolemPattern2(GameObject guardianGolemRook)
    {
        _guardianGolemRook = guardianGolemRook;
    }

    /// <summary>
    /// 패턴 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(GuardianGolemPattern(boss));
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler != null &&
               boss.BombHandler.PlayerController != null &&
               _guardianGolemRook != null;
    }

    private IEnumerator GuardianGolemPattern(BaseBoss boss)
    {
        int deleteCount = boss.gameObject.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;

        List<Vector3Int> positions = new List<Vector3Int>();

        for (int y = deleteCount; y <= 8 - deleteCount; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                // ↘ or ↙ 대각선 조건
                if (x == y || x + y == 8)
                {
                    positions.Add(new Vector3Int(4 - x, 4 - y, 0));
                }
            }
        }

        for (int y = 0; y < deleteCount; y++)
        {
            for (int x = deleteCount; x <= 8 - deleteCount; x++)
            {
                positions.Add(new Vector3Int(4 - x, 4 - y, 0));
            }
        }
        for (int y = 8; y > 8 - deleteCount; y--)
        {
            for (int x = deleteCount; x <= 8 - deleteCount; x++)
            {
                positions.Add(new Vector3Int(4 - x, 4 - y, 0));
            }
        }

        boss.StartCoroutine(AttackSound());

        Vector3Int centerPos = new Vector3Int(4, 4, 0); // 중심은 원하는 기준으로 설정
        boss.BombHandler.ExecuteFixedBomb(positions, centerPos, _guardianGolemRook,
                                        warningDuration: 0.8f, explosionDuration: 0.7f, damage: 20);

        boss.AttackAnimation();

        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator AttackSound()
    {
        yield return new WaitForSeconds(0.8f); // 소리 재생 후 대기
        SoundManager.Instance.GolemSoundClip("GolemAttackActivate");
    }
}
