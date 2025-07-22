using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurtreePattern6 : IBossAttackPattern
{
    private GameObject _attackPrefab;
    private int _damage;

    public string PatternName => "5_7";

    public TurtreePattern6(GameObject AttackPrefab, int Damage)
    {
        _attackPrefab = AttackPrefab;
        _damage = Damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _attackPrefab != null && boss.BombHandler != null;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        float quarterBeat = boss.QuarterBeat;

        for (int i = 0; i < 5; i++)
        {
            yield return boss.StartCoroutine(OutskirtBomb(boss, i));
        }
    }


    private IEnumerator OutskirtBomb(BaseBoss boss, int distance)
    {
        boss.AttackAnimation();

        List<Vector3Int> bombPoint = new List<Vector3Int>();

        for (int x = -4; x <= 4; x++)
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

        boss.BombHandler.ExecuteFixedBomb(
            bombPoint,
            new Vector3Int(4, 4, 0),
            _attackPrefab,
            warningDuration: 1f,
            explosionDuration: 2f,
            damage: _damage,
            warningType: WarningType.Type1,
            patternName:PatternName
        );

        yield return new WaitForSeconds(boss.Beat * 2);
    }





    private IEnumerator TurtreeAttackSound()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.TurtreeSoundClip("TurtreeAttackActivate");
    }
}
