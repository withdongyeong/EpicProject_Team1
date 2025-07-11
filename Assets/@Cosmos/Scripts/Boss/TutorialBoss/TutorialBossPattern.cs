using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialBossPattern : IBossAttackPattern
{
    private GameObject _tutorialBossAttack;
    private bool _isOddNumber;
    private int _damage;

    public string PatternName => "TutorialBossPattern";
    public TutorialBossPattern(GameObject TutorialBossAttack, bool IsOddNumber, int damage)
    {
        _tutorialBossAttack = TutorialBossAttack;
        _isOddNumber = IsOddNumber;
        _damage = damage;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        boss.AttackAnimation();
        yield return TutorialBossAttack(boss);
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _tutorialBossAttack != null;
    }

    public IEnumerator TutorialBossAttack(BaseBoss boss)
    {
        List<Vector3Int> AttackPoint = new List<Vector3Int>();
        int size = 9;
        int[,] grid = new int[size, size];

        // 마트료시카 패턴 생성
        for (int layer = 0; layer <= size / 2; layer++)
        {
            int value = (layer % 2 == 0) ? 1 : 0;

            for (int i = layer; i < size - layer; i++)
            {
                grid[layer, i] = value;                      // 상단
                grid[size - 1 - layer, i] = value;           // 하단
                grid[i, layer] = value;                      // 좌측
                grid[i, size - 1 - layer] = value;           // 우측
            }
        }

        // 패턴을 기준으로 공격 좌표 결정
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if ((_isOddNumber && grid[x, y] == 1) || (!_isOddNumber && grid[x, y] == 0))
                {
                    // 중심을 (4,4)로 맞추기 위해 -4
                    AttackPoint.Add(new Vector3Int(x - 4, y - 4, 0));
                }
            }
        }

        boss.BombHandler.ExecuteFixedBomb(
            AttackPoint,
            new Vector3Int(4, 4, 0),
            _tutorialBossAttack,
            warningDuration: 1f,
            explosionDuration: 0.4f,
            damage: _damage
        );

        yield return new WaitForSeconds(boss.Beat);
    }
}
