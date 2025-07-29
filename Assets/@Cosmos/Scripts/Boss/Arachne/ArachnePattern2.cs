using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ArachnePattern2 : IBossAttackPattern
{
    private GameObject _poisionAriaPrefab;
    private List<Vector3Int> _singlePointShape;
    private List<Vector3Int> _bigAttackShape;
    private int _damage;

    public string PatternName => "2_2";

    public ArachnePattern2(GameObject poisionAriaPrefab, int Damage)
    {
        _poisionAriaPrefab = poisionAriaPrefab;
        _damage = Damage;

        _singlePointShape = new List<Vector3Int> { new Vector3Int(0, 0, 0) };

        _bigAttackShape = new List<Vector3Int>();
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (x % 2 == 0 && y % 2 == 0) continue;
                _bigAttackShape.Add(new Vector3Int(x - 4, y - 4, 0));
            }
        }
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        float beat = boss.Beat;
        yield return boss.StartCoroutine(SpiderAttack(boss, beat));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler != null &&
               boss.BombHandler.PlayerController != null &&
               _poisionAriaPrefab != null;
    }

    private IEnumerator SpiderAttack(BaseBoss boss, float beat)
    {
        yield return FireSingle(boss, beat);

        for (int i = 0; i < 5; i++)
        {
            yield return FireSingle(boss, beat);
        }

        yield return boss.StartCoroutine(PoisonBigAttack_LShape(boss, beat));
    }

    private IEnumerator FireSingle(BaseBoss boss, float beat)
    {
        boss.BombHandler.ExecuteTargetingBomb(_singlePointShape, _poisionAriaPrefab, 1f, 0.7f, _damage, patternName:PatternName);
        boss.StartCoroutine(PlayDelayedSound("PoisionExplotionActivate", 1f));
        boss.AttackAnimation();
        yield return new WaitForSeconds(beat / 2);
    }

    private IEnumerator PoisonBigAttack_LShape(BaseBoss boss, float beat)
    {
        int splitValue = 5;
        HashSet<Vector3Int> attackShape = new HashSet<Vector3Int>();

        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if ((y < splitValue || x < splitValue) && !(x % 2 == 0 && y % 2 == 0))
                    attackShape.Add(new Vector3Int(x - 4, y - 4, 0));
            }
        }

        boss.BombHandler.ExecuteFixedBomb(attackShape.ToList(), new Vector3Int(4, 4, 0), _poisionAriaPrefab, 1f, 0.7f, _damage, patternName:PatternName);
        boss.StartCoroutine(PlayDelayedSound("PoisionExplotionActivate", 1f));
        yield return new WaitForSeconds(beat / 2);
    }

    private IEnumerator PlayDelayedSound(string soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.ArachneSoundClip(soundName);
    }
}
