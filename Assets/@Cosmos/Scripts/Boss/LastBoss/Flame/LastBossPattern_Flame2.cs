using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossPattern_Flame2 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    private int _damage;
    public string PatternName => "Flame2";

    public LastBossPattern_Flame2(GameObject explosionPrefab, int damage)
    {
        _explosionPrefab = explosionPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");

        // 중심에서 바깥으로 웨이브 형태로 확장
        for (int radius = 0; radius <= 3; radius++)
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

            yield return new WaitForSeconds(radius <= 2 ? boss.Beat / 2 : boss.Beat / 4);
        }

        yield return new WaitForSeconds(boss.Beat);
    }

    private List<Vector3Int> GetRingLayer(int radius)
    {
        List<Vector3Int> result = new();

        // radius 0인 경우 중심점만
        if (radius == 0)
        {
            result.Add(new Vector3Int(4, 4, 0));
            return result;
        }

        for (int x = 1; x < 8; x++) // 경계 제외: 1~7
        {
            for (int y = 1; y < 8; y++) // 경계 제외: 1~7
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
        pos.x >= 1 && pos.x <= 7 && pos.y >= 1 && pos.y <= 7;

    private IEnumerator SoundPlay()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.LastBossSoundClip("LastBossFlameAttackActivate");
    }
}