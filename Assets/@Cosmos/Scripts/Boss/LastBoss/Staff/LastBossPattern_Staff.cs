using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종보스 - 지팡이 기반 패턴 (회전 무기 + 마법진 구조 이펙트)
/// </summary>
public class LastBossPattern_Staff : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    private int _damage;

    public string PatternName => "StaffPattern";

    public LastBossPattern_Staff(GameObject explosionPrefab, int damage)
    {
        _explosionPrefab = explosionPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _explosionPrefab != null;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");

        Vector3Int center = new Vector3Int(4, 4, 0);
        List<Vector3Int> ring1 = GetCirclePattern(center, 1);
        List<Vector3Int> ring2 = GetCirclePattern(center, 2);
        List<Vector3Int> ring3 = GetCirclePattern(center, 3);
        List<Vector3Int> ring4 = GetCirclePattern(center, 4);
        List<Vector3Int> ring5 = GetCirclePattern(center, 5);

        yield return ExecuteRing(boss, ring1);
        yield return ExecuteRing(boss, ring2);
        yield return ExecuteRing(boss, ring3);
        yield return ExecuteRing(boss, ring4);
        yield return ExecuteRing(boss, ring5);

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator ExecuteRing(BaseBoss boss, List<Vector3Int> positions)
    {
        foreach (var pos in positions)
        {
            boss.StartCoroutine(PlayAttackSound());

            boss.BombHandler.ExecuteFixedBomb(
                new List<Vector3Int> { Vector3Int.zero },
                pos,
                _explosionPrefab,
                warningDuration: 0.8f,
                explosionDuration: 1f,
                damage: _damage,
                WarningType.Type1
            );
        }

        yield return new WaitForSeconds(0.15f);
    }

    private List<Vector3Int> GetCirclePattern(Vector3Int center, int radius)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        for (int angle = 0; angle < 360; angle += 45)
        {
            float rad = angle * Mathf.Deg2Rad;
            int x = Mathf.RoundToInt(center.x + radius * Mathf.Cos(rad));
            int y = Mathf.RoundToInt(center.y + radius * Mathf.Sin(rad));

            Vector3Int pos = new Vector3Int(x, y, 0);
            if (IsValidGridPosition(pos))
                positions.Add(pos);
        }
        return positions;
    }

    private bool IsValidGridPosition(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < 9 && pos.y >= 0 && pos.y < 9;
    }

    private IEnumerator PlayAttackSound()
    {
        yield return new WaitForSeconds(0.8f);
        SoundManager.Instance.LastBossSoundClip("LastBossStaffAttackActivate");
    }
}
