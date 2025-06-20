using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class SlimeFloorPattern1 : IBossAttackPattern
{
    private GameObject _slimeFloorPrefeb;

    public string PatternName => "SlimeFloorPattern1";
    public SlimeFloorPattern1(GameObject SlimeFloorPrefeb)
    {
        _slimeFloorPrefeb = SlimeFloorPrefeb;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.AttackAnimation();
        yield return SlimeFloorPattern(boss);
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _slimeFloorPrefeb != null;
    }

    //대각선
    public IEnumerator SlimeFloorPattern(BaseBoss boss)
    {
        List<Vector3Int> gridWithoutWindmill = new List<Vector3Int>();

        for (int i = -4; i <= 4; i++)
        {
            if (i == 0)
            {
                gridWithoutWindmill.Add(new Vector3Int(0, 0, 0));
                continue;
            }
            else
            {
                gridWithoutWindmill.Add(new Vector3Int(i, i, 0));
                gridWithoutWindmill.Add(new Vector3Int(i, -i, 0));
            }
        }


        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                if ((Mathf.Abs(i) != Mathf.Abs(j)))
                {
                    gridWithoutWindmill.Add(new Vector3Int(i, j, 0));
                }
            }

        }

        Vector3Int centerPos = new Vector3Int(4, 4, 0);



        boss.BombHandler.ExecuteFixedBomb(gridWithoutWindmill, centerPos, _slimeFloorPrefeb,
                                        warningDuration: 0.8f, explosionDuration: 0.7f, damage: 20);

        yield return new WaitForSeconds(0.6f);
        SoundManager.Instance.SlimeSoundClip("PoisonBallActivate");
        yield return new WaitForSeconds(0.4f);
        SoundManager.Instance.SlimeSoundClip("PoisionExplotionActivate");

        yield return 0;
    }
}
