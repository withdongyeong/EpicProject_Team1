using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReaperShortDeathAreaPattern : IBossAttackPattern
{
    private GameObject _shortDeathAria;
    private int _deathAriaTime;
    public string PatternName => "ReaperShortDeathAriaPattern";

    /// <summary>
    /// 가디언 골렘 패턴1 생성자
    /// </summary>
    /// <param name="poisionAriaPrefab">독 이펙트 프리팹</param>
    public ReaperShortDeathAreaPattern(GameObject ShortDeathAria, int DeathAriaTime)
    {
        _shortDeathAria = ShortDeathAria;
        _deathAriaTime = DeathAriaTime;
    }

    /// <summary>
    /// 패턴 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(reaperShortDeathAriaPattern(boss));
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
               _shortDeathAria != null;
    }

    private IEnumerator reaperShortDeathAriaPattern(BaseBoss boss)
    {
        for(int i = 0; i < 3; i++)
        {
            List<Vector3Int> webPositions = new List<Vector3Int>();

            // 랜덤 위치 생성 (플레이어 위치 제외)
            int x = Random.Range(1, 8);
            int y = Random.Range(1, 8);

            Vector3Int randomPosition = new Vector3Int(x, y, 0);

            int R = Random.Range(0, 2);

            if (R == 0)
            {
                webPositions.Add(randomPosition + new Vector3Int(-1, 0, 0));
                webPositions.Add(randomPosition);
                webPositions.Add(randomPosition + new Vector3Int(1, 0, 0));
            }

            else if (R == 1)
            {
                webPositions.Add(randomPosition + new Vector3Int(0, -1, 0));
                webPositions.Add(randomPosition);
                webPositions.Add(randomPosition + new Vector3Int(0, 1, 0));
            }

           boss.BombHandler.ExecuteFixedBomb(webPositions, new Vector3Int(0, 0, 0), _shortDeathAria,
                                                  warningDuration: 0.5f, explosionDuration: _deathAriaTime, damage: 0, warningType: WarningType.Type3);

            yield return new WaitForSeconds(0.2f);
        }

        yield return 0;
    }
}
