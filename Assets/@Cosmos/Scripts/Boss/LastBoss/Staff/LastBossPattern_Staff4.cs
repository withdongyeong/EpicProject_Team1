using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastBossPattern_Staff4 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    private int _damage;
    private bool _isSoundCoolTime = false; 
    public string PatternName => "10_8";

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

        // 4구역 중앙 2x2 좌표들 (Staff3 나선형과 겹치는 부분)
        HashSet<Vector3Int> excludePositions = new HashSet<Vector3Int>
        {
            new(1,1,0), new(2,1,0), new(1,2,0), new(2,2,0), // 좌하 중앙
            new(6,1,0), new(7,1,0), new(6,2,0), new(7,2,0), // 우하 중앙
            new(1,6,0), new(2,6,0), new(1,7,0), new(2,7,0), // 좌상 중앙
            new(6,6,0), new(7,6,0), new(6,7,0), new(7,7,0)  // 우상 중앙
        };

        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 9; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                
                if (x == 4 || y == 4) continue; // 십자형 공간 제외
                if ((x + y) % 2 != 0) continue; // 체스판 패턴 (짝수 합만)
                if (excludePositions.Contains(pos)) continue; // 4구역 중앙 제외
                
                positions.Add(pos);
            }

        boss.StartCoroutine(PlayAttackSound(boss, boss.Beat));

        foreach (var pos in positions)
        {
            boss.BombHandler.ExecuteFixedBomb(new() { Vector3Int.zero }, pos, _explosionPrefab, 1f, 1f, _damage, warningType:WarningType.Type1, patternName:PatternName);
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