using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurtreePattern6 : IBossAttackPattern
{
    private GameObject _attackPrefab;

    public string PatternName => "TurtreePattern6";

    public TurtreePattern6(GameObject AttackPrefab)
    {
        _attackPrefab = AttackPrefab;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _attackPrefab != null && boss.BombHandler != null;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        for(int i = 0; i < 5; i++)
        {
            yield return outskirtBomb(boss, i);
        }
    }

    public IEnumerator outskirtBomb(BaseBoss boss, int distance)
    {
        boss.AttackAnimation();

        List<Vector3Int> bombPoint = new List<Vector3Int>();

        for (int x = -4; x <=4; x++)
        {
            for (int y = -4; y <= 4; y++)
            {
                if (Mathf.Abs(x) == distance || Mathf.Abs(y) == distance)
                {
                    bombPoint.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        boss.StartCoroutine(TurtreeAttackSound());

        boss.BombHandler.ExecuteFixedBomb(bombPoint, new Vector3Int(4, 4, 0), _attackPrefab,
                                             warningDuration: 0.8f, explosionDuration: 2f, damage: 20, WarningType.Type1);

        yield return new WaitForSeconds(0.6f);
    }


    private IEnumerator TurtreeAttackSound()
    {
        yield return new WaitForSeconds(0.8f);
        SoundManager.Instance.TurtreeSoundClip("TurtreeAttackActivate");
    }
}
