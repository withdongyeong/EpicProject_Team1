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

        boss.StartCoroutine(SoundPlay());

        // Step 0: 중심 한 칸만 공격
        Vector3Int center = new Vector3Int(4, 4, 0);
        boss.BombHandler.ExecuteFixedBomb(
            new() { Vector3Int.zero },
            center,
            _explosionPrefab,
            0.8f,
            0.8f,
            _damage,
            WarningType.Type1
        );

        yield return new WaitForSeconds(0.3f);

        // Step 1~3: 중심부터 확장하면서 누적 공격 (경계 제외)
        for (int radius = 1; radius <= 3; radius++)
        {
            // 0~radius까지 누적 범위
            for (int r = 0; r <= radius; r++)
            {
                foreach (var pos in GetRingLayer(r))
                {
                    if (IsValid(pos))
                    {
                        boss.StartCoroutine(SoundPlay());

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

            yield return new WaitForSeconds(radius <= 2 ? 0.3f : 0.1f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    private List<Vector3Int> GetRingLayer(int radius)
    {
        List<Vector3Int> result = new();

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
        yield return new WaitForSeconds(0.8f);
        SoundManager.Instance.LastBossSoundClip("LastBossFlameAttackActivate");
    }
}
