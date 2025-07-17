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
    private bool _isSoundCoolTime = false;
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
        HashSet<Vector3Int> usedPositions = new HashSet<Vector3Int>();
        
        // 가운데 먼저 공격
        List<Vector3Int> centerAttack = new List<Vector3Int> { center };
        usedPositions.Add(center);
        
        List<Vector3Int> ring1 = GetUniqueCirclePattern(center, 1, usedPositions);
        List<Vector3Int> ring2 = GetUniqueCirclePattern(center, 2, usedPositions);
        List<Vector3Int> ring3 = GetUniqueCirclePattern(center, 3, usedPositions);
        List<Vector3Int> ring4 = GetUniqueCirclePattern(center, 4, usedPositions);
        List<Vector3Int> ring5 = GetUniqueCirclePattern(center, 5, usedPositions);

        yield return ExecuteRing(boss, centerAttack);
        yield return ExecuteRing(boss, ring1);
        yield return ExecuteRing(boss, ring2);
        yield return ExecuteRing(boss, ring3);
        yield return ExecuteRing(boss, ring4);
        yield return ExecuteRing(boss, ring5);

        yield return new WaitForSeconds(boss.Beat);
    }

    private IEnumerator ExecuteRing(BaseBoss boss, List<Vector3Int> positions)
    {
        boss.StartCoroutine(PlayAttackSound(boss, boss.Beat / 4));

        foreach (var pos in positions)
        {
            boss.BombHandler.ExecuteFixedBomb(
                new List<Vector3Int> { Vector3Int.zero },
                pos,
                _explosionPrefab,
                warningDuration: 1f,
                explosionDuration: 1f,
                damage: _damage,
                WarningType.Type1
            );
        }

        yield return new WaitForSeconds(boss.Beat/4);
    }

    private List<Vector3Int> GetUniqueCirclePattern(Vector3Int center, int radius, HashSet<Vector3Int> usedPositions)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        for (int angle = 0; angle < 360; angle += 45)
        {
            float rad = angle * Mathf.Deg2Rad;
            int x = Mathf.RoundToInt(center.x + radius * Mathf.Cos(rad));
            int y = Mathf.RoundToInt(center.y + radius * Mathf.Sin(rad));

            Vector3Int pos = new Vector3Int(x, y, 0);
            if (IsValidGridPosition(pos) && !usedPositions.Contains(pos))
            {
                positions.Add(pos);
                usedPositions.Add(pos);
            }
        }
        return positions;
    }

    private bool IsValidGridPosition(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < 9 && pos.y >= 0 && pos.y < 9;
    }

    public IEnumerator PlayAttackSound(BaseBoss boss, float coolTime)
    {
        if (_isSoundCoolTime)
        {
            yield break; // 쿨타임 중이면 실행하지 않음
        }
        boss.StartCoroutine(SoundPlay());
        boss.StartCoroutine(SetSoundCoolTime(coolTime));
    }

    public IEnumerator SoundPlay()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.LastBossSoundClip("LastBossStaffAttackActivate");
    }

    public IEnumerator SetSoundCoolTime(float isCoolTime)
    {
        _isSoundCoolTime = true;
        yield return new WaitForSeconds(isCoolTime);
        _isSoundCoolTime = false;
    }
}