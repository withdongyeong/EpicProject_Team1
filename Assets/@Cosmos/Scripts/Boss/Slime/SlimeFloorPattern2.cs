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
        yield return SlimeFloorPattern(boss);
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _slimeFloorPrefeb != null;
    }

    public IEnumerator SlimeFloorPattern(BaseBoss boss)
    {
        List<Vector3Int> fullGridWithoutCenter = new List<Vector3Int>();

        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                // 중앙 3x3은 건너뛰기
                if (x >= 3 && x <= 5 && y >= 3 && y <= 5)
                    continue;

                fullGridWithoutCenter.Add(new Vector3Int(x, y, 0));
            }
        }

        yield return 0;
    }
}
