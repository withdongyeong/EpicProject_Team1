using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GuardianGolemPattern1 : IBossAttackPattern
{
    private GameObject _guardianGolemRook;
    private bool _isOdd;
    private int _damage;

    public string PatternName => "GuardianGolemPattern1";

    /// <summary>
    /// 가디언 골렘 패턴1 생성자
    /// </summary>
    /// <param name="poisionAriaPrefab">독 이펙트 프리팹</param>
    public GuardianGolemPattern1(GameObject guardianGolemRook, bool IsOdd, int damage)
    {
        _guardianGolemRook = guardianGolemRook;
        _isOdd = IsOdd;
        _damage = damage;
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
        List<Vector3Int> positions = new List<Vector3Int>();
        int Wallcount = boss.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;

        for (int y = 0; y < 9; y++)
        {
            if (_isOdd && y % 2 == 0) continue;   // 홀수만 저장
            if (!_isOdd && y % 2 != 0) continue;  // 짝수만 저장

            for (int x = Wallcount; x <= 8 - Wallcount; x++)
            {
                positions.Add(new Vector3Int(4 - x, 4 - y, 0));
            }
        }

        Vector3Int centerPos = new Vector3Int(4, 4, 0);

        boss.StartCoroutine(AttackSound());

        boss.BombHandler.ExecuteFixedBomb(positions, centerPos, _guardianGolemRook,
                                        warningDuration: 0.8f, explosionDuration: 0.7f, damage: _damage);

        boss.AttackAnimation();

        yield return new WaitForSeconds(0.3f);
    }

    private IEnumerator AttackSound()
    {
        yield return new WaitForSeconds(0.8f); // 소리 재생 후 대기
        SoundManager.Instance.GolemSoundClip("GolemAttackActivate");
    }
}
