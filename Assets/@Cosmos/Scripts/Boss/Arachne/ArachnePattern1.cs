using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArachnePattern1 : IBossAttackPattern
{
    private GameObject _lToRspiderLegPrefab;
    private GameObject _rToLspiderLegPrefab;
    private int _damage;

    public string PatternName => "ArachnePattern1";

    public ArachnePattern1(GameObject LToRspiderLegPrefab, GameObject RToLspiderLegPrefab, int Damage)
    {
        _lToRspiderLegPrefab = LToRspiderLegPrefab;
        _rToLspiderLegPrefab = RToLspiderLegPrefab;
        _damage = Damage;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(SpiderSlash1(boss, true));
        yield return boss.StartCoroutine(SpiderSlash2(boss, false));
        // 동시 진행
        boss.StartCoroutine(SpiderSlash1(boss, true));
        yield return boss.StartCoroutine(SpiderSlash2(boss, true));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler != null &&
               boss.BombHandler.PlayerController != null &&
               _lToRspiderLegPrefab != null;
    }

    private IEnumerator SpiderSlash1(BaseBoss boss, bool isTorsion)
    {
        float halfBeat = boss.HalfBeat;

        for (int i = 0; i < 9; i++)
        {
            int centerX = i;
            int centerY = isTorsion ? 8 - i : 8 - i + 1;

            Vector3Int centerPos = new Vector3Int(centerX, centerY, 0);

            List<Vector3Int> EffectslashShape = new();
            List<Vector3Int> slashShape = new();
            for (int j = -2; j <= 2; j++)
            {
                if (j == 0) EffectslashShape.Add(new Vector3Int(j, j, 0));
                else slashShape.Add(new Vector3Int(j, j, 0));
            }

            boss.BombHandler.ExecuteFixedBomb(EffectslashShape, centerPos, _rToLspiderLegPrefab,
                warningDuration: 1f, explosionDuration: halfBeat, damage: _damage);
            
            boss.BombHandler.ExecuteWarningThenDamage(slashShape, centerPos,
                warningDuration: 1f, damage: _damage);

            boss.StartCoroutine(PlayDelayedSound("SpiderLegActivate", 1f));
            boss.AttackAnimation();

            yield return new WaitForSeconds(halfBeat);
        }
    }

    private IEnumerator SpiderSlash2(BaseBoss boss, bool isTorsion)
    {
        float halfBeat = boss.HalfBeat;

        for (int i = 0; i < 9; i++)
        {
            int centerX = 8 - i;
            int centerY = isTorsion ? 8 - i : 8 - i + 1;

            Vector3Int centerPos = new Vector3Int(centerX, centerY, 0);

            List<Vector3Int> EffectslashShape = new();
            List<Vector3Int> slashShape = new();
            for (int j = -2; j <= 3; j++)
            {
                if (j == 0) EffectslashShape.Add(new Vector3Int(j, -j, 0));
                else slashShape.Add(new Vector3Int(j, -j, 0));
            }

            boss.BombHandler.ExecuteFixedBomb(EffectslashShape, centerPos, _lToRspiderLegPrefab,
                warningDuration: 1f, explosionDuration: halfBeat, damage: _damage);

            boss.BombHandler.ExecuteWarningThenDamage(slashShape, centerPos,
                warningDuration: 1f, damage: _damage);

            if (!isTorsion) boss.StartCoroutine(PlayDelayedSound("SpiderLegActivate", 1f));
            boss.AttackAnimation();

            yield return new WaitForSeconds(halfBeat);
        }
    }

    private IEnumerator PlayDelayedSound(string soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.ArachneSoundClip(soundName);
    }
}
