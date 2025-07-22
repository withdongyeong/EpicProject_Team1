using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurtreePattern2 : IBossAttackPattern
{
    private GameObject _treeAttackPrefeb;
    private int _damage;

    public string PatternName => "5_3";

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

        float halfBeat = boss.HalfBeat;
        float lastSoundTime = -999f;

        for (int x = -3; x <= 3; x++)
        {
            List<Vector3Int> BombPoints = new();

            for (int y = -4; y <= 4; y++)
            {
                if (x == y || x == -y) continue; // X자 비우기
                BombPoints.Add(new Vector3Int(x, y, 0));
            }

            if (BombPoints.Count > 0)
            {
                boss.AttackAnimation();

                boss.BombHandler.ExecuteFixedBomb(
                    BombPoints,
                    new Vector3Int(4, 4, 0),
                    _treeAttackPrefeb,
                    warningDuration: 1f,
                    explosionDuration: 2f,
                    damage: _damage,
                    patternName:PatternName
                );

                if (Time.time - lastSoundTime >= halfBeat)
                {
                    boss.StartCoroutine(TurtreeAttackSound());
                    lastSoundTime = Time.time;
                }
            }

            yield return new WaitForSeconds(halfBeat);
        }
    }


    private IEnumerator TurtreeAttackSound()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.TurtreeSoundClip("TurtreeAttackActivate");
    }
}
