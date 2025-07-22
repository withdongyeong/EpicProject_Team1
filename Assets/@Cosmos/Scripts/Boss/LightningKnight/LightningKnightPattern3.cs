using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightningKnightPattern3 : IBossAttackPattern
{
    private GameObject _lightningActtck;
    private Vector3Int _startPoint;
    private int _damage;
    public string PatternName => "8_5";

    public LightningKnightPattern3(GameObject lightningActtck, Vector3Int StartPoint, int Damage)
    {
        _lightningActtck = lightningActtck;
        _startPoint = StartPoint;
        _damage = Damage;
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
            List<Vector3Int> DamageArea = new List<Vector3Int>();
            DamageArea.Add(ElectroArea);

            boss.StartCoroutine(LightningKnightAttackSound());

            boss.BombHandler.ExecuteFixedBomb(DamageArea, new Vector3Int(4, 4, 0), _lightningActtck,
                                      warningDuration: 1f, explosionDuration: 1f, damage: _damage, warningType:WarningType.Type1, patternName:PatternName);

            yield return new WaitForSeconds(boss.Beat / 8);
        }
    }

    private List<Vector3Int> ElectroAreas()
    {
        List<Vector3Int> AttackPoints = new List<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();

        AttackPoints.Add(_startPoint);
        queue.Enqueue(_startPoint);

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

    private IEnumerator LightningKnightAttackSound()
    {
        yield return new WaitForSeconds(1f); // 사운드 재생을 위한 대기
        SoundManager.Instance.KnightSoundClip("KnightAttackActivate");
    }

}
