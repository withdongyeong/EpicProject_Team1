using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossPattern_Staff4 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    private int _damage;
    private bool _isSoundCoolTime = false; 
    public string PatternName => "StaffPattern4";

    public LastBossPattern_Staff4(GameObject explosionPrefab, int damage)
    {
        _explosionPrefab = explosionPrefab;
        _damage = damage;
    }

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

        boss.StartCoroutine(PlayAttackSound(boss, boss.Beat));

        foreach (var pos in positions)
        {
            boss.BombHandler.ExecuteFixedBomb(new() { Vector3Int.zero }, pos, _explosionPrefab, 1f, 1f, _damage, WarningType.Type1);
        }

        yield return new WaitForSeconds(boss.Beat);
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