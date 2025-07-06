using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종보스 - Frost1: 랜덤한 지역에 십자 모양으로 얼음 공격을 쏟아붓기
/// </summary>
public class LastBossPattern_Frost1 : IBossAttackPattern
{
    private GameObject _explosionPrefab;
    private int _damage;
    public string PatternName => "Frost1";

    public LastBossPattern_Frost1(GameObject explosionPrefab, int damage)
    {
        _explosionPrefab = explosionPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss) => boss != null && _explosionPrefab != null;

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");

        // 십자 공격 실행
        for (int i = 0; i < 15; i++)
        {
            Vector3Int center = GetRandomPosition();
            List<Vector3Int> crossShape = GetCrossShape(center);

            foreach (var pos in crossShape)
            {
                if (IsValid(pos))
                {
                    boss.StartCoroutine(SoundPlay());

                    boss.BombHandler.ExecuteFixedBomb(
                        new() { Vector3Int.zero },
                        pos,
                        _explosionPrefab,
                        0.8f,
                        0.8f,
                        _damage,
                        WarningType.Type1
                    );
                }
            }

            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(0.5f);
    }

    private Vector3Int GetRandomPosition()
    {
        // 9x9 격자에서 랜덤한 위치 선택
        // 경계에서 너무 가까운 곳은 피하기 위해 1~7 범위 사용
        int x = Random.Range(1, 8);
        int y = Random.Range(1, 8);
        return new Vector3Int(x, y, 0);
    }

    private List<Vector3Int> GetCrossShape(Vector3Int center)
    {
        List<Vector3Int> shape = new() { center };

        // 십자 모양 생성 (상하좌우 각각 2칸씩)
        Vector3Int[] directions = { 
            Vector3Int.up, Vector3Int.down, 
            Vector3Int.left, Vector3Int.right 
        };

        foreach (var dir in directions)
        {
            for (int i = 1; i <= 2; i++)
            {
                Vector3Int pos = center + dir * i;
                shape.Add(pos);
            }
        }

        return shape;
    }

    private bool IsValid(Vector3Int pos) =>
        pos.x >= 0 && pos.x < 9 && pos.y >= 0 && pos.y < 9;
    private IEnumerator SoundPlay()
    {
        yield return new WaitForSeconds(0.8f);
        SoundManager.Instance.LastBossSoundClip("LastBossFrostAttackActivate");
    }
}