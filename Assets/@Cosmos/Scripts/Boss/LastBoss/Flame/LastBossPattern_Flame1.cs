using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossPattern_Flame1 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    private int _damage;
    public string PatternName => "Flame1";

    public LastBossPattern_Flame1(GameObject explosionPrefab, int damage)
    {
        _explosionPrefab = explosionPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");

        for (int radius = 4; radius >= 1; radius--)
        {
            for (int r = 4; r >= radius; r--)
            {
                foreach (var pos in GetRingLayer(r))
                {
                    if (IsValid(pos))
                    {
                        boss.BombHandler.ExecuteFixedBomb(
                            new() { Vector3Int.zero },
                            pos,
                            _explosionPrefab,
                            0.8f,
                            0.8f,
                            _damage,
                            WarningType.Type1
                        );
                    }
                }
            }

            yield return new WaitForSeconds(radius >= 2 ? 0.4f : 0.15f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    private List<Vector3Int> GetRingLayer(int radius)
    {
        List<Vector3Int> result = new();

        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                int dx = Mathf.Abs(x - 4);
                int dy = Mathf.Abs(y - 4);
                if (Mathf.Max(dx, dy) == radius)
                    result.Add(new Vector3Int(x, y, 0));
            }
        }

        return result;
    }

    private bool IsValid(Vector3Int pos) =>
        pos.x >= 0 && pos.x < 9 && pos.y >= 0 && pos.y < 9;
}
