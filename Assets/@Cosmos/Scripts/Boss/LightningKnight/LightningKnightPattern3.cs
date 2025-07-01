using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningKnightPattern3 : IBossAttackPattern
{
    private GameObject _lightningActtck;

    public string PatternName => "LightningKnightPattern3";

    public LightningKnightPattern3(GameObject lightningActtck)
    {
        _lightningActtck = lightningActtck;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _lightningActtck != null && boss.BombHandler != null;
    }

/// <summary>
/// 전기 흐르기 패턴
/// </summary>
/// <param name="boss"></param>
/// <returns></returns>
    public IEnumerator Execute(BaseBoss boss)
    {
        boss.AttackAnimation();

        foreach (var ElectroArea in ElectroAreas())
        {
            List <Vector3Int> DamageArea = new List<Vector3Int>();
            DamageArea.Add(ElectroArea);

            boss.BombHandler.ExecuteFixedBomb(DamageArea, new Vector3Int(4, 4, 0), _lightningActtck,
                                      warningDuration: 0.8f, explosionDuration: 1f, damage: 25, WarningType.Type1);

            yield return new WaitForSeconds(0.05f);
        }
    }

    private List<Vector3Int> ElectroAreas()
    {
        List<Vector3Int> AttackPoints = new List<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();

        Vector3Int StartPoint = new Vector3Int(4, 0, 0);
        AttackPoints.Add(StartPoint);
        queue.Enqueue(StartPoint);

        while (queue.Count > 0)
        {
            Vector3Int current = queue.Dequeue();

            int pattern = Random.Range(0, 2); 

            if (pattern == 0)
            {
                Vector3Int next = current + new Vector3Int(-1, 0, 0);

                if (Mathf.Abs(next.x) < 5 && Mathf.Abs(next.y) < 5 && !AttackPoints.Contains(next))
                {
                    AttackPoints.Add(next);
                    queue.Enqueue(next);
                }
            }
            else
            {
                Vector3Int[] verticals = new Vector3Int[]
                {
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0)
                };

                foreach (var dir in verticals)
                {
                    Vector3Int next = current + dir;

                    if (Mathf.Abs(next.x) < 5 && Mathf.Abs(next.y) < 5 && !AttackPoints.Contains(next))
                    {
                        AttackPoints.Add(next);
                        queue.Enqueue(next);
                    }
                }
            }
        }

        return AttackPoints;
    }
}
