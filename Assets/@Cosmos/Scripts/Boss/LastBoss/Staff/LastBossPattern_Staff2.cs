using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossPattern_Staff2 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    public string PatternName => "StaffPattern2";
    public LastBossPattern_Staff2(GameObject explosionPrefab) => _explosionPrefab = explosionPrefab;
    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");
        Vector3Int center = new Vector3Int(4, 4, 0);
        List<Vector3Int> targets = new();

        // 마름모 (맨해튼 거리 1~2)
        for (int dx = -2; dx <= 2; dx++)
        for (int dy = -2; dy <= 2; dy++)
        {
            int dist = Mathf.Abs(dx) + Mathf.Abs(dy);
            if (dist >= 1 && dist <= 2)
            {
                Vector3Int pos = center + new Vector3Int(dx, dy, 0);
                AddIfValid(pos, targets);
            }
        }

        // 외곽 원형 (radius 4.5)
        for (int angle = 0; angle < 360; angle += 20)
        {
            float rad = angle * Mathf.Deg2Rad;
            int x = Mathf.RoundToInt(center.x + 4.5f * Mathf.Cos(rad));
            int y = Mathf.RoundToInt(center.y + 4.5f * Mathf.Sin(rad));
            AddIfValid(new Vector3Int(x, y, 0), targets);
        }

        foreach (var pos in targets)
            boss.BombHandler.ExecuteFixedBomb(new() { Vector3Int.zero }, pos, _explosionPrefab, 0.8f, 1f, 22, WarningType.Type1);

        yield return new WaitForSeconds(1f);
    }

    private void AddIfValid(Vector3Int pos, List<Vector3Int> list)
    {
        if (pos.x >= 0 && pos.x < 9 && pos.y >= 0 && pos.y < 9)
            list.Add(pos);
    }
}