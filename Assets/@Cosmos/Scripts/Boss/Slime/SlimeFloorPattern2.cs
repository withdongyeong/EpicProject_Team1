using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SlimeFloorPattern2 : IBossAttackPattern
{
    private GameObject _slimeFloorPrefeb;
    private int _damage;

    public string PatternName => "SlimeFloorPattern2";

    public SlimeFloorPattern2(GameObject SlimeFloorPrefeb, int damage)
    {
        _slimeFloorPrefeb = SlimeFloorPrefeb;
        _damage = damage;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.AttackAnimation();
        yield return SlimeFloorPattern(boss);
    }

    public bool CanExecute(BaseBoss boss) => _slimeFloorPrefeb != null;

    public IEnumerator SlimeFloorPattern(BaseBoss boss)
    {
        float beat = boss.Beat;
        float halfBeat = boss.HalfBeat;
        Vector3Int centerPos = new Vector3Int(4, 4, 0);
        List<Vector3Int> cells = new();

        for (int x = -4; x <= 0; x++)
        {
            for (int y = -4; y <= 0; y++)
            {
                if ((x == 0 && y == 0) || Mathf.Abs(x) == Mathf.Abs(y)) continue;
                if (Mathf.Abs(y) >= Mathf.Abs(x)) cells.Add(new Vector3Int(x, y, 0));
            }
        }

        for (int x = 0; x <= 4; x++)
        {
            for (int y = -4; y <= 0; y++)
            {
                if ((x == 0 && y == 0) || Mathf.Abs(x) == Mathf.Abs(y)) continue;
                if (Mathf.Abs(y) <= Mathf.Abs(x)) cells.Add(new Vector3Int(x, y, 0));
            }
        }

        for (int x = 0; x <= 4; x++)
        {
            for (int y = 0; y <= 4; y++)
            {
                if ((x == 0 && y == 0) || Mathf.Abs(x) == Mathf.Abs(y)) continue;
                if (Mathf.Abs(y) >= Mathf.Abs(x)) cells.Add(new Vector3Int(x, y, 0));
            }
        }

        for (int x = -4; x <= 0; x++)
        {
            for (int y = 0; y <= 4; y++)
            {
                if ((x == 0 && y == 0) || Mathf.Abs(x) == Mathf.Abs(y)) continue;
                if (Mathf.Abs(y) <= Mathf.Abs(x)) cells.Add(new Vector3Int(x, y, 0));
            }
        }

        cells.Add(Vector3Int.zero);

        Dictionary<int, List<Vector3Int>> layers = new();
        foreach (var cell in cells)
        {
            int d = Mathf.Max(Mathf.Abs(cell.x), Mathf.Abs(cell.y));
            if (!layers.ContainsKey(d)) layers[d] = new();
            layers[d].Add(cell);
        }

        foreach (var kv in layers.OrderBy(p => p.Key))
        {
            boss.BombHandler.ExecuteFixedBomb(kv.Value, centerPos, _slimeFloorPrefeb, 1f, 0.7f, _damage);
            boss.StartCoroutine(SlimeSoundEffect());
            yield return new WaitForSeconds(beat);
        }
    }
    
    public IEnumerator SlimeSoundEffect()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.SlimeSoundClip("PoisonBallActivate");
        yield return new WaitForSeconds(0.1f);
        SoundManager.Instance.SlimeSoundClip("PoisionExplotionActivate");
    }
}
