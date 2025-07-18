using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArachnePattern3 : IBossAttackPattern
{
    private GameObject _poisionAriaPrefab;
    private GameObject _lToRspiderLeg;
    private GameObject _rToLspiderLeg;
    private int _strongDamage;
    private int _weakDamage;

    public string PatternName => "ArachnePattern3";

    public ArachnePattern3(GameObject poisionAriaPrefab, GameObject LToRspiderLegPrefab, GameObject RToLspiderLegPrefab, int strongDamage, int weakDamage)
    {
        _poisionAriaPrefab = poisionAriaPrefab;
        _lToRspiderLeg = LToRspiderLegPrefab;
        _rToLspiderLeg = RToLspiderLegPrefab;
        _strongDamage = strongDamage;
        _weakDamage = weakDamage;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        float beat = boss.Beat;
        float halfBeat = boss.HalfBeat;
        yield return boss.StartCoroutine(SpiderLeg(boss, beat));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler != null &&
               boss.BombHandler.PlayerController != null &&
               _poisionAriaPrefab != null &&
               _lToRspiderLeg != null;
    }

    private IEnumerator SpiderLeg(BaseBoss boss, float beat)
    {
        for (int i = 0; i < 4; i++)
        {
            boss.StartCoroutine(ExecuteAreaAttack(boss));
            yield return new WaitForSeconds(beat);
        }

        yield return new WaitForSeconds(beat);

        boss.StartCoroutine(SpiderLeg_DiagonalSlash1(boss));
        yield return new WaitForSeconds(beat / 2);

        boss.StartCoroutine(SpiderLeg_DiagonalSlash2(boss));
        yield return new WaitForSeconds(beat / 2);
    }

    private IEnumerator ExecuteAreaAttack(BaseBoss boss)
    {
        SoundManager.Instance.ArachneSoundClip("PoisonBallActivate");

        Vector2Int[] excludeDirections = {
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1)
        };

        Vector2Int exclude = excludeDirections[Random.Range(0, excludeDirections.Length)];

        List<Vector3Int> attackShape = new List<Vector3Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ((x == exclude.x && y == exclude.y) || (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)) continue;
                attackShape.Add(new Vector3Int(x, y, 0));
            }
        }

        boss.BombHandler.ExecuteTargetingBomb(attackShape, _poisionAriaPrefab, 1f, 0.7f, _weakDamage);
        boss.AttackAnimation();
        boss.StartCoroutine(PlayDelayedSound("PoisionExplotionActivate", 1f));

        yield return null;
    }

    private IEnumerator SpiderLeg_DiagonalSlash1(BaseBoss boss)
    {
        List<Vector3Int> effect = new();
        List<Vector3Int> damage = new();

        for (int i = -2; i <= 2; i++)
        {
            if (i == 0) effect.Add(new Vector3Int(i, i, 0));
            else damage.Add(new Vector3Int(i, i, 0));
        }

        Vector3Int player = new(boss.BombHandler.PlayerController.CurrentX, boss.BombHandler.PlayerController.CurrentY, 0);

        boss.BombHandler.ExecuteFixedBomb(effect, player, _rToLspiderLeg, 1f, 0.3f, _strongDamage);
        boss.BombHandler.ExecuteWarningThenDamage(damage, player, 1f, _strongDamage);
        boss.AttackAnimation();
        boss.StartCoroutine(PlayDelayedSound("SpiderLegActivate", 1f));

        yield return null;
    }

    private IEnumerator SpiderLeg_DiagonalSlash2(BaseBoss boss)
    {
        List<Vector3Int> effect = new();
        List<Vector3Int> damage = new();

        for (int i = -2; i <= 2; i++)
        {
            if (i == 0) effect.Add(new Vector3Int(i, -i, 0));
            else damage.Add(new Vector3Int(i, -i, 0));
        }

        Vector3Int player = new(boss.BombHandler.PlayerController.CurrentX, boss.BombHandler.PlayerController.CurrentY, 0);

        boss.BombHandler.ExecuteFixedBomb(effect, player, _lToRspiderLeg, 1f, 0.3f, _strongDamage);
        boss.BombHandler.ExecuteWarningThenDamage(damage, player, 1f, _strongDamage);
        boss.AttackAnimation();
        boss.StartCoroutine(PlayDelayedSound("SpiderLegActivate", 1f));

        yield return null;
    }

    private IEnumerator PlayDelayedSound(string soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.ArachneSoundClip(soundName);
    }
}
