using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReaperPattern2 : IBossAttackPattern
{
    private GameObject _deathAriaPrefeb;

    public string PatternName => "ReaperPattern2";

    public ReaperPattern2(GameObject DeathAriaPrefeb)
    {
        _deathAriaPrefeb = DeathAriaPrefeb;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _deathAriaPrefeb != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 4영역 X - 매 웨이브마다 다른 방향에서 시작
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        List<Vector3Int> PointList = new List<Vector3Int>();

        for (int x = -4; x <= 4; x++)
        {
            for (int y = -4; y <= 4; y++)
            {
                if (x == 0 && y == 0) continue;

                if (x == y || x + y == 0) PointList.Add(new Vector3Int(x, y));

                if( x + y == -5 || x + y == 5) PointList.Add(new Vector3Int(x, y));
                if (x - y == -5 || x - y == 5) PointList.Add(new Vector3Int(x, y));
            }
        }

        boss.StartCoroutine(AttackSoundSound());

        boss.BombHandler.ExecuteFixedBomb(PointList, new Vector3Int(4, 4, 0), _deathAriaPrefeb,
                                  warningDuration: 0.8f, explosionDuration: 1.0f, damage: 20, warningType: WarningType.Type1);

        boss.AttackAnimation();

        yield return 0;
    }

    private IEnumerator AttackSoundSound()
    {
        yield return new WaitForSeconds(0.8f);
        SoundManager.Instance.ReaperSoundClip("ReaperAttackActivate");
    }

}
