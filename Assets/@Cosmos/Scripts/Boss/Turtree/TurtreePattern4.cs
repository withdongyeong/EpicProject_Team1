using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurtreePattern4 : IBossAttackPattern
{
    private GameObject _treeAttackPrefeb;
    private bool _isDownUP;
    public string PatternName => "TurtreePattern4";

    public TurtreePattern4(GameObject TreeAttackPrefeb, bool isDownUP)
    {
        _treeAttackPrefeb = TreeAttackPrefeb;
        _isDownUP = isDownUP;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null &&
               _treeAttackPrefeb != null &&
               GridManager.Instance != null &&
               boss.BombHandler != null;
    }




    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(RotatingBombAttack(boss, _isDownUP));
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator RotatingBombAttack(BaseBoss boss, bool K)
    {
        List<Vector3Int> points = GetRotating3x3Pattern(Vector3Int.zero);

        List<Vector3Int> BombCenter = new List<Vector3Int>();
        if (_isDownUP) BombCenter = StartPoint1;
        else BombCenter = StartPoint2;

        for (int i = 0; i < 9; i++)
        {
            boss.BombHandler.ExecuteFixedBomb(points, BombCenter[i], _treeAttackPrefeb,
                    warningDuration: 0.8f, explosionDuration: 0.2f, damage: 20);

            if (i == 0 || i == 3) yield return new WaitForSeconds(0.3f);
        }
    }

    private List<Vector3Int> StartPoint1 = new List<Vector3Int>
    {
        new Vector3Int(1,1,0),
        new Vector3Int(1,4,0),
        new Vector3Int(4,4,0),
        new Vector3Int(4,1,0),
        new Vector3Int(1,7,0),
        new Vector3Int(7,1,0),
        new Vector3Int(7,4,0),
        new Vector3Int(4,7,0),
        new Vector3Int(7,7,0)
    };

    private List<Vector3Int> StartPoint2 = new List<Vector3Int>
    {
        new Vector3Int(1,7,0),
        new Vector3Int(1,4,0),
        new Vector3Int(4,4,0),
        new Vector3Int(4,7,0),
        new Vector3Int(1,1,0),
        new Vector3Int(4,1,0),
        new Vector3Int(7,1,0),
        new Vector3Int(7,4,0),
        new Vector3Int(7,7,0)
    };

    /// <summary>
    /// 중심 뺀 3*3
    /// </summary>
    /// <param name="center"></param>
    /// <returns></returns>
    private List<Vector3Int> GetRotating3x3Pattern(Vector3Int center)
    {
        List<Vector3Int> pattern = new List<Vector3Int>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                pattern.Add(new Vector3Int(x, y, 0));
            }
        }

        return pattern;
    }

}
