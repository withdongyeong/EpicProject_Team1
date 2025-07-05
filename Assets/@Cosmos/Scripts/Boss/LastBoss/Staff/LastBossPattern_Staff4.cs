using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossPattern_Staff4 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    public string PatternName => "StaffPattern4";
    public LastBossPattern_Staff4(GameObject explosionPrefab) => _explosionPrefab = explosionPrefab;
    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");
        List<Vector3Int> positions = new();

        for (int x = 0; x < 9; x++)
        for (int y = 0; y < 9; y++)
        {
            if (x == 4 || y == 4) continue; // 십자형 공간 제외
            if ((x + y) % 2 == 0)           // 일정 밀도로
                positions.Add(new Vector3Int(x, y, 0));
        }

        foreach (var pos in positions)
        {
            boss.BombHandler.ExecuteFixedBomb(new() { Vector3Int.zero }, pos, _explosionPrefab, 0.8f, 1f, 18, WarningType.Type1);
        }

        yield return new WaitForSeconds(1f);
    }
}