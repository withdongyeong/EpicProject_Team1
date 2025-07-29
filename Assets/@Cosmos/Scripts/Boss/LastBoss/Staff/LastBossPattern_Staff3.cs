using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종보스 - StaffPattern3 : 십자 중심 제외, 나머지 4구역 4x4 완전 나선
/// </summary>
public class LastBossPattern_Staff3 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    private int _damage;
    private bool _isSoundCoolTime = false; 
    public string PatternName => "10_7";

    public LastBossPattern_Staff3(GameObject explosionPrefab, int damage)
    {
        _explosionPrefab = explosionPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        // 중심 (4, 4) 먼저 공격
        boss.BombHandler.ExecuteFixedBomb(new() { Vector3Int.zero }, new Vector3Int(4, 4, 0), _explosionPrefab, 1f, 1f, _damage, warningType:WarningType.Type1, patternName:PatternName);

        // 병렬 실행
        boss.StartCoroutine(SpiralInArea(boss, 0, 5)); // 좌상
        boss.StartCoroutine(SpiralInArea(boss, 5, 5)); // 우상
        boss.StartCoroutine(SpiralInArea(boss, 0, 0)); // 좌하
        boss.StartCoroutine(SpiralInArea(boss, 5, 0)); // 우하

        // 중심 십자 고정 공격 (중심 x=4 또는 y=4)
        for (int i = 0; i < 9; i++)
        {
            boss.StartCoroutine(PlayAttackSound(boss, boss.Beat/4));

            if (i == 4) continue;
            boss.BombHandler.ExecuteFixedBomb(new() { Vector3Int.zero }, new Vector3Int(4, i, 0), _explosionPrefab, 1f, 1f, _damage, warningType:WarningType.Type1, patternName:PatternName);
            boss.BombHandler.ExecuteFixedBomb(new() { Vector3Int.zero }, new Vector3Int(i, 4, 0), _explosionPrefab, 1f, 1f, _damage, warningType:WarningType.Type1, patternName:PatternName);
        }

        yield return new WaitForSeconds(boss.Beat * 2);
    }

    private IEnumerator SpiralInArea(BaseBoss boss, int startX, int startY)
    {
        Vector3Int[] dirs = { Vector3Int.right, Vector3Int.down, Vector3Int.left, Vector3Int.up };
        bool[,] visited = new bool[4, 4];
        int[,] gridX = new int[4, 4];
        int[,] gridY = new int[4, 4];

        for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
        {
            gridX[i, j] = startX + i;
            gridY[i, j] = startY + j;
        }

        int cx = 0, cy = 0, dir = 0;

        for (int count = 0; count < 16;)
        {
            if (!visited[cx, cy])
            {
                int gx = gridX[cx, cy];
                int gy = gridY[cx, cy];
                boss.BombHandler.ExecuteFixedBomb(new() { Vector3Int.zero }, new Vector3Int(gx, gy, 0), _explosionPrefab, 0.8f, 1f, _damage, warningType:WarningType.Type1, patternName:PatternName);
                visited[cx, cy] = true;
                count++;
                yield return new WaitForSeconds(boss.Beat/4);
            }

            int nx = cx + dirs[dir].x;
            int ny = cy + dirs[dir].y;

            if (nx >= 0 && nx < 4 && ny >= 0 && ny < 4 && !visited[nx, ny])
            {
                boss.StartCoroutine(PlayAttackSound(boss ,boss.Beat / 4));

                cx = nx;
                cy = ny;
            }
            else
            {
                dir = (dir + 1) % 4;
            }
        }
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