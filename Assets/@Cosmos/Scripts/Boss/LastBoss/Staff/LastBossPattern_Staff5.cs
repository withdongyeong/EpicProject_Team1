using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종보스 - StaffPattern5 : 중심 X자 제외, 회전형 원형 문양
/// </summary>
public class LastBossPattern_Staff5 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    public string PatternName => "StaffPattern5";
    private int _damage;
    private bool _isSoundCoolTime = false;

    public LastBossPattern_Staff5(GameObject explosionPrefab, int damage)
    {
        _explosionPrefab = explosionPrefab;
        _damage = damage;
    }
    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");

        Vector3Int center = new(4, 4, 0);
        int[] radii = { 1, 2, 3, 4 };

        for (int ring = 0; ring < radii.Length; ring++)
        {
            List<Vector3Int> ringPositions = new();
            for (int angle = 0; angle < 360; angle += 30)
            {
                float rad = angle * Mathf.Deg2Rad;
                int x = Mathf.RoundToInt(center.x + radii[ring] * Mathf.Cos(rad));
                int y = Mathf.RoundToInt(center.y + radii[ring] * Mathf.Sin(rad));
                Vector3Int pos = new Vector3Int(x, y, 0);

                if (!IsCenterX(pos) && IsValid(pos))
                    ringPositions.Add(pos);
            }

            boss.StartCoroutine(PlayAttackSound(boss, boss.Beat / 4));
  

            foreach (var pos in ringPositions)
            {
                boss.BombHandler.ExecuteFixedBomb(new() { Vector3Int.zero }, pos, _explosionPrefab, 1f, 1f, _damage, WarningType.Type1);
            }

            yield return new WaitForSeconds(boss.Beat/4);
        }

        yield return new WaitForSeconds(boss.Beat);
    }

    private bool IsCenterX(Vector3Int pos)
    {
        return pos.x == pos.y || pos.x + pos.y == 8;
    }

    private bool IsValid(Vector3Int pos)
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