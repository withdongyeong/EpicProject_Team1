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
        yield return SlimeFloorPattern(boss);
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _slimeFloorPrefeb != null;
    }

    //대각선 + 중앙 3 * 3
    public IEnumerator SlimeFloorPattern(BaseBoss boss)
    {
        List<Vector3Int> diagonalList = new List<Vector3Int>();
        List<Vector3Int> diagonalAnti = new List<Vector3Int>();

        for (int i = -4; i <= 4; i++)
        {
            if (i == 0)
            {
                diagonalList.Add(new Vector3Int(0, 0, 0));
                continue;
            }
            else
            {
                diagonalList.Add(new Vector3Int(i, i, 0));
                diagonalList.Add(new Vector3Int(i, -i, 0));
            }
        }

        Vector3Int centerPos = new Vector3Int(4, 4, 0);
        boss.BombManager.ExecuteFixedBomb(diagonalList, centerPos, _slimeFloorPrefeb,
                                        warningDuration: 0.8f, explosionDuration: 0.7f, damage: 20);

        yield return 0;
    }
}
