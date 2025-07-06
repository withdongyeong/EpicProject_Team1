using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurtreePattern2 : IBossAttackPattern
{
    private GameObject _treeAttackPrefeb;
    private int _damage;

    public string PatternName => "TurtreePattern2";
    public TurtreePattern2(GameObject TreeAttackPrefeb, int Damage)
    {
        _treeAttackPrefeb = TreeAttackPrefeb;
        _damage = Damage;
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
        boss.AttackAnimation();

        for (int x = -3; x <= 3; x++)
        {
            List<Vector3Int> BombPoints = new List<Vector3Int>();

            for (int y = -4; y <= 4; y++)
            {
                if (x == y || x == -y) continue;

                BombPoints.Add(new Vector3Int(x, y, 0));
            }

            boss.AttackAnimation();
            boss.StartCoroutine(TurtreeAttackSound());

            boss.BombHandler.ExecuteFixedBomb(BombPoints, new Vector3Int(4, 4, 0), _treeAttackPrefeb,
              warningDuration: 0.8f, explosionDuration: 2f, damage: _damage);

            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator TurtreeAttackSound()
    {
        yield return new WaitForSeconds(0.8f);
        SoundManager.Instance.TurtreeSoundClip("TurtreeAttackActivate");
    }
}
