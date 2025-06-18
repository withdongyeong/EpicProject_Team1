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
        List<Vector3Int> diagonalMain = new List<Vector3Int>();
        List<Vector3Int> diagonalAnti = new List<Vector3Int>();
        List<Vector3Int> centerArea = new List<Vector3Int>();

        for (int i = -4; i < 4; i++)
        {
            diagonalMain.Add(new Vector3Int(i, i, 0));
            diagonalAnti.Add(new Vector3Int(i, 8 - i, 0));
        }

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                centerArea.Add(new Vector3Int(x, y, 0));
            }
        }

        List<Vector3Int> all = new List<Vector3Int>();
        all.AddRange(diagonalMain);
        all.AddRange(diagonalAnti);
        all.AddRange(centerArea);

        List<Vector3Int> uniqueOnly = all
            .GroupBy(pos => pos)
            .Where(g => g.Count() == 1)
            .Select(g => g.Key)
            .ToList();

        Vector3Int centerPos = new Vector3Int(4, 4, 0);

        boss.BombManager.ExecuteFixedBomb(all.ToList(), centerPos, _slimeFloorPrefeb,
                                        warningDuration: 0.8f, explosionDuration: 0.7f, damage: 20);

        yield return 0;
    }
}
