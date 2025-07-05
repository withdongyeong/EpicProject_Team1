using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuardianGolemVerticalWavePattern : IBossAttackPattern
{
    private GameObject _guardianGolemRook;
    private int Wallcount;

    public string PatternName => "GuardianGolemTemporaryWallSummonPattern";

    /// <summary>
    /// 임시벽 생성자
    /// </summary>
    public GuardianGolemVerticalWavePattern(GameObject GuardianGolemRook)
    {
        _guardianGolemRook = GuardianGolemRook;
    }

    /// <summary>
    /// 패턴 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(ExecuteRowWaveAttack(boss));
        yield return boss.StartCoroutine(ExecuteSpiderWebAttack(boss));
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler.PlayerController != null && _guardianGolemRook != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 세로 파도 공격
    /// </summary>
    private IEnumerator ExecuteSpiderWebAttack(BaseBoss boss)
    {
        Wallcount = boss.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;

        int RandomPoint = Random.Range(Wallcount, 4);

        for (int x = 8 - Wallcount; x >= Wallcount; x--)
        {
            if (RandomPoint == x) continue;

            // 각 열(세로줄)을 병렬로 실행
            boss.StartCoroutine(ExecuteColumnAttack(boss, x));

            yield return new WaitForSeconds(0.3f); // 공격 전체 딜레이 (적절히 조절)
        }
    }

    private IEnumerator ExecuteColumnAttack(BaseBoss boss, int x)
    {
        boss.AttackAnimation();

        for (int y = 0; y < 9; y++)
        {
            boss.StartCoroutine(AttackSound());

            List<Vector3Int> positions = new List<Vector3Int> { new Vector3Int(0, 0, 0) };

            boss.BombHandler.ExecuteFixedBomb(
                positions,
                new Vector3Int(x, y, 0),
                _guardianGolemRook,
                warningDuration: 0.8f,
                explosionDuration: 0.7f,
                damage: 20
            );
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator ExecuteRowWaveAttack(BaseBoss boss)
    {
        Wallcount = boss.GetComponent<GuardianGolemWallCreationPattern>().DeleteCount;

        int RandomPoint = Random.Range(5, 9);

        for (int y = 0; y < 9; y++)
        {
            if (y == RandomPoint) continue;

            // 각 줄을 병렬적으로 실행
            boss.StartCoroutine(ExecuteRow(boss, y));


            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator ExecuteRow(BaseBoss boss, int y)
    {
        boss.AttackAnimation();

        for (int x = Wallcount; x < 9 - Wallcount; x++)
        {
            boss.StartCoroutine(AttackSound());

            List<Vector3Int> positions = new List<Vector3Int> { new Vector3Int(0, 0, 0) };

            boss.BombHandler.ExecuteFixedBomb(
                positions,
                new Vector3Int(x, y, 0),
                _guardianGolemRook,
                warningDuration: 0.8f,
                explosionDuration: 0.7f,
                damage: 20
            );

            yield return new WaitForSeconds(0.1f); // 좌→우로 순차 폭발
        }
    }

    private IEnumerator AttackSound()
    {
        yield return new WaitForSeconds(0.8f); // 소리 재생 후 대기
        SoundManager.Instance.GolemSoundClip("GolemAttackActivate");
    }

}

