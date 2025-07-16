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

        // 바깥에서 안으로 웨이브 형태로 공격
        for (int radius = 4; radius >= 1; radius--)
        {
            boss.StartCoroutine(SoundPlay());

            // 현재 반지름의 링만 공격 (누적 X)
            foreach (var pos in GetRingLayer(radius))
            {
                if (IsValid(pos))
                {
                    boss.BombHandler.ExecuteFixedBomb(
                        new() { Vector3Int.zero },
                        pos,
                        _explosionPrefab,
                        1f,
                        0.8f,
                        _damage,
                        WarningType.Type1
                    );
                }
            }

            yield return new WaitForSeconds(radius >= 2 ? boss.Beat / 2 : boss.Beat / 4);
        }

        yield return new WaitForSeconds(boss.Beat);
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

    private IEnumerator SoundPlay()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.LastBossSoundClip("LastBossFlameAttackActivate");
    }
}