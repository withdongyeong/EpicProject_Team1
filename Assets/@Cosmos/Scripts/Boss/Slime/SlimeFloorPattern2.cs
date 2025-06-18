using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlimeFloorPattern2 : IBossAttackPattern
{
    private GameObject _slimeFloorPrefeb;

    public string PatternName => "SlimeFloorPattern2";
    public SlimeFloorPattern2(GameObject SlimeFloorPrefeb)
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

    public IEnumerator SlimeFloorPattern(BaseBoss boss)
    {
        List<Vector3Int> gridWithoutWindmill = new List<Vector3Int>();

        for (int x = -4; x <= 0; x++)
        {
            for (int y = -4; y <= 0; y++)
            {
                if ((x == 0 && y == 0) || (Mathf.Abs(x) == Mathf.Abs(y)))
                    continue;

                if (Mathf.Abs(y) >= Mathf.Abs(x)) // 삼각형 조건
                {
                    gridWithoutWindmill.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        for (int x = 0; x <= 4; x++)
        {
            for (int y = -4; y <= 0; y++)
            {
                if ((x == 0 && y == 0) || (Mathf.Abs(x) == Mathf.Abs(y)))
                    continue;

                if (Mathf.Abs(y) <= Mathf.Abs(x)) // 삼각형 조건
                {
                    gridWithoutWindmill.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        for (int x = 0; x <= 4; x++)
        {
            for (int y = 0; y <= 4; y++)
            {
                if ((x == 0 && y == 0) || (Mathf.Abs(x) == Mathf.Abs(y)))
                    continue;

                if (Mathf.Abs(y) >= Mathf.Abs(x)) // 삼각형 조건
                {
                    gridWithoutWindmill.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        for (int x = -4; x <= 0; x++)
        {
            for (int y = 0; y <= 4; y++)
            {
                if ((x == 0 && y == 0) || (Mathf.Abs(x) == Mathf.Abs(y)))
                    continue;

                if (Mathf.Abs(y) <= Mathf.Abs(x)) // 삼각형 조건
                {
                    gridWithoutWindmill.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        Vector3Int centerPos = new Vector3Int(4, 4, 0);

        boss.BombManager.ExecuteFixedBomb(gridWithoutWindmill, centerPos, _slimeFloorPrefeb,
                                        warningDuration: 0.8f, explosionDuration: 0.7f, damage: 20);

        yield return new WaitForSeconds(0.6f);
        SoundManager.Instance.SlimeSoundClip("PoisonBallActivate");
        yield return new WaitForSeconds(0.4f);
        SoundManager.Instance.SlimeSoundClip("PoisionExplotionActivate");

        yield return 0;
    }
}
