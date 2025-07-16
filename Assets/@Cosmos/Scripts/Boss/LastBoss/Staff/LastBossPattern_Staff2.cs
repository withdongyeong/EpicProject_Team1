using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossPattern_Staff2 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    private int _damage;
    private bool _isSoundCoolTime = false;
    public string PatternName => "StaffPattern2";

    public LastBossPattern_Staff2(GameObject explosionPrefab, int damage)
    {
        _explosionPrefab = explosionPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");
        Vector3Int center = new Vector3Int(4, 4, 0);
        List<Vector3Int> targets = new();

        // 중앙 공격 먼저 추가
        targets.Add(center);

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

        boss.StartCoroutine(PlayAttackSound(boss, 0));

        foreach (var pos in targets)
        {
            boss.BombHandler.ExecuteFixedBomb(new() { Vector3Int.zero }, pos, _explosionPrefab, 1f, 1f, _damage, WarningType.Type1);
        }

        yield return new WaitForSeconds(boss.Beat);
    }

    private void AddIfValid(Vector3Int pos, List<Vector3Int> list)
    {
        if (pos.x >= 0 && pos.x < 9 && pos.y >= 0 && pos.y < 9)
            list.Add(pos);
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