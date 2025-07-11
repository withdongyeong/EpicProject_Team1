using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class BomberSpeardAttack : IBossAttackPattern
{
    private GameObject _bombActtck;
    private GameObject _bombball;
    private Vector3Int _centerPos;
    private int _damage;

    private float beat;
    private float quarterBeat;

    public string PatternName => "LightningKnightSpeardAttack";

    public BomberSpeardAttack(GameObject BombActtck, GameObject Bombball, Vector3Int centerPos, int damage)
    {
        _bombActtck = BombActtck;
        _bombball = Bombball;
        _centerPos = centerPos;
        _damage = damage;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        beat = boss.Beat;
        quarterBeat = boss.QuarterBeat;

        boss.StartCoroutine(lightningAttack(boss, _centerPos));
        yield return new WaitForSeconds(beat);
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler.PlayerController != null && _bombActtck != null && boss.BombHandler != null;
    }

    public IEnumerator BombCreate(BaseBoss boss, Vector3Int centerPos)
    {
        boss.BombHandler.ExecuteFixedBomb(
            new List<Vector3Int>() { Vector3Int.zero },
            centerPos,
            _bombball,
            warningDuration: 0f,
            explosionDuration: 0.3f,
            damage: 0,
            warningType: WarningType.Type3
        );

        yield return new WaitForSeconds(quarterBeat);
    }

    public IEnumerator lightningAttack(BaseBoss boss, Vector3Int centerPos)
    {
        Vector3Int[] directions = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(1, -1, 0)
        };

        for (int dist = 0; dist < 9; dist++)
        {
            HashSet<Vector3Int> uniquePositions = new();

            if (dist == 0)
            {
                uniquePositions.Add(Vector3Int.zero);
            }
            else
            {
                foreach (var dir in directions)
                    uniquePositions.Add(dir * dist);
            }

            boss.BombHandler.ExecuteFixedBomb(
                new List<Vector3Int>(uniquePositions),
                centerPos,
                _bombActtck,
                warningDuration: 1f,
                explosionDuration: 1f,
                damage: _damage,
                WarningType.Type1
            );

            boss.StartCoroutine(BombSound());

            yield return new WaitForSeconds(quarterBeat);
        }

        float total = quarterBeat * 9;
        float rounded = Mathf.Ceil(total / beat) * beat;
        float remainder = rounded - total;

        yield return new WaitForSeconds(remainder);
    }

    public IEnumerator BombSound()
    {
        yield return new WaitForSeconds(quarterBeat * 7);
        SoundManager.Instance.BomberSoundClip("BomberAttackActivate");
    }
}
