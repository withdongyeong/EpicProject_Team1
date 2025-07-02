using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurtreePattern3 : IBossAttackPattern
{
    private GameObject _treeAttackPrefeb;

    public string PatternName => "TurtreePattern3";
    public TurtreePattern3(GameObject TreeAttackPrefeb)
    {
        _treeAttackPrefeb = TreeAttackPrefeb;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        yield return BackAttack(boss);
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _treeAttackPrefeb != null;
    }

    private IEnumerator BackAttack(BaseBoss boss)
    {
        for(int x = -4; x <= 3; x++)
        {
            List<Vector3Int> BombPoints = new List<Vector3Int>();

            for(int y = -4; y <= 4; y++)
            {
                BombPoints.Add(new Vector3Int(x, y, 0));
            }

            boss.BombHandler.ExecuteFixedBomb(BombPoints, new Vector3Int(4, 4, 0), _treeAttackPrefeb,
              warningDuration: 0.8f, explosionDuration: 0.3f, damage: 20);

            yield return new WaitForSeconds(0.1f);
        }
    }
}
