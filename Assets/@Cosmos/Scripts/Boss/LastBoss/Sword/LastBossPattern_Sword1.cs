using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종보스 - Sword1: 검 모양 → 굵기 3짜리 직선 연속 공격
/// </summary>
public class LastBossPattern_Sword1 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    private int _damage;
    public string PatternName => "Sword1";

    public LastBossPattern_Sword1(GameObject explosionPrefab, int damage)
    {
        _explosionPrefab = explosionPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");

        var (dir, tip) = GetRandomDirectionAndTip();

        // 1. 검 모양 1회 공격
        List<Vector3Int> swordShape = GetSwordShape(tip, dir);
        foreach (var pos in swordShape)
        {
            if (IsValid(pos))
            {
                boss.StartCoroutine(SoundPlay());

                boss.BombHandler.ExecuteFixedBomb(
                    new() { Vector3Int.zero },
                    pos,
                    _explosionPrefab,
                    1f,
                    1f,
                    _damage,
                    WarningType.Type1
                );
            }
        }

        yield return new WaitForSeconds(boss.Beat);

        // 2. 직선 연속 공격 (굵기 3)
        for (int i = 1; i < 9; i++)
        {
            Vector3Int head = tip + dir * i;
            List<Vector3Int> line = GetThickLine(head, dir);

            foreach (var pos in line)
            {
                if (IsValid(pos))
                {
                    boss.StartCoroutine(SoundPlay());

                    boss.BombHandler.ExecuteFixedBomb(
                        new() { Vector3Int.zero },
                        pos,
                        _explosionPrefab,
                        1f,
                        1f,
                        _damage,
                        WarningType.Type1
                    );
                }
            }

            yield return new WaitForSeconds(boss.Beat/8);
        }

        yield return new WaitForSeconds(boss.Beat/2);
    }

    private (Vector3Int dir, Vector3Int tip) GetRandomDirectionAndTip()
    {
        int dirIndex = Random.Range(0, 4);
        int[] tipOffsets = { 1, 2, 3, 4, 5, 6, 7 };
        int index = tipOffsets[Random.Range(0, tipOffsets.Length)];
        Vector3Int dir, tip;

        switch (dirIndex)
        {
            case 0: // 위
                dir = Vector3Int.up;
                tip = new Vector3Int(index, 1, 0);
                break;
            case 1: // 아래
                dir = Vector3Int.down;
                tip = new Vector3Int(index, 7, 0);
                break;
            case 2: // 왼쪽
                dir = Vector3Int.left;
                tip = new Vector3Int(7, index, 0);
                break;
            default: // 오른쪽
                dir = Vector3Int.right;
                tip = new Vector3Int(1, index, 0);
                break;
        }

        return (dir, tip);
    }

    private List<Vector3Int> GetSwordShape(Vector3Int tip, Vector3Int dir)
    {
        List<Vector3Int> shape = new() { tip };

        // guard 좌우
        if (dir.x != 0)
        {
            shape.Add(tip + Vector3Int.up);
            shape.Add(tip + Vector3Int.down);
        }
        else
        {
            shape.Add(tip + Vector3Int.left);
            shape.Add(tip + Vector3Int.right);
        }

        // 칼날
        for (int i = 1; i <= 3; i++)
            shape.Add(tip + dir * i);

        // 손잡이
        shape.Add(tip - dir);

        return shape;
    }

    private List<Vector3Int> GetThickLine(Vector3Int center, Vector3Int dir)
    {
        List<Vector3Int> result = new() { center };

        if (dir.x != 0)
        {
            result.Add(center + Vector3Int.up);
            result.Add(center + Vector3Int.down);
        }
        else
        {
            result.Add(center + Vector3Int.left);
            result.Add(center + Vector3Int.right);
        }

        return result;
    }

    private bool IsValid(Vector3Int pos) =>
        pos.x >= 0 && pos.x < 9 && pos.y >= 0 && pos.y < 9;

    private IEnumerator SoundPlay()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.LastBossSoundClip("LastBossSwordAttackActivate");
    }
}
