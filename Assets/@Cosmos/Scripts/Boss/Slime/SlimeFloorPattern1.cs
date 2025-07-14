using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SlimeFloorPattern1 : IBossAttackPattern
{
    private GameObject _slimeFloorPrefeb;
    private int _damage;

    public string PatternName => "SlimeFloorPattern1";

    public SlimeFloorPattern1(GameObject SlimeFloorPrefeb, int damage)
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
        Vector3Int centerPos = new Vector3Int(4, 4, 0);

        // 기존 모양을 그대로 구성
        List<Vector3Int> originalPattern = new();

        for (int i = -4; i <= 4; i++)
        {
            if (i == 0) continue;
            originalPattern.Add(new Vector3Int(i, i, 0));
            originalPattern.Add(new Vector3Int(i, -i, 0));
        }

        for (int i = -2; i <= 2; i++)
        {
            for (int j = -2; j <= 2; j++)
            {
                if (Mathf.Abs(i) != Mathf.Abs(j))
                    originalPattern.Add(new Vector3Int(i, j, 0));
            }
        }

        // 중심으로부터 거리(정확히는 Manhattan 거리) 기준으로 그룹핑
        Dictionary<int, List<Vector3Int>> layered = new();
        foreach (var cell in originalPattern)
        {
            int dist = Mathf.Max(Mathf.Abs(cell.x), Mathf.Abs(cell.y)); // Chebyshev 거리
            if (!layered.ContainsKey(dist))
                layered[dist] = new List<Vector3Int>();
            layered[dist].Add(cell);
        }

        // 중심 (0,0) 포함 (옵션)
        layered[0] = new() { new Vector3Int(0, 0, 0) };

        foreach (var layer in layered.OrderBy(k => k.Key))
        {
            boss.BombHandler.ExecuteFixedBomb(
                layer.Value,
                centerPos,
                _slimeFloorPrefeb,
                warningDuration: 1f,
                explosionDuration: 0.7f,
                damage: _damage
            );
            boss.StartCoroutine(SlimeSoundEffect());
            yield return new WaitForSeconds(beat/2);
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
