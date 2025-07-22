using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialBossPattern : IBossAttackPattern
{
    private GameObject _tutorialBossAttack;
    private bool _isOddNumber;
    private int _damage;

    public string PatternName => "0_1";
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
        int size = 9;
        int[,] grid = new int[size, size];

        // 마트료시카 패턴 생성
        for (int layer = 0; layer <= size / 2; layer++)
        {
            int value = (layer % 2 == 0) ? 1 : 0;
            for (int i = layer; i < size - layer; i++)
            {
                grid[layer, i] = value;                    // 상단
                grid[size - 1 - layer, i] = value;         // 하단
                grid[i, layer] = value;                    // 좌측
                grid[i, size - 1 - layer] = value;         // 우측
            }
        }

        // ▶ 레이어별 공격 좌표 그룹화
        List<List<Vector3Int>> groupedPoints = new List<List<Vector3Int>>();

        for (int layer = 0; layer <= size / 2; layer++)
        {
            List<Vector3Int> layerPoints = new List<Vector3Int>();

            for (int i = layer; i < size - layer; i++)
            {
                if (Match(grid[layer, i])) layerPoints.Add(new Vector3Int(layer - 4, i - 4, 0));          // 상단
                if (Match(grid[size - 1 - layer, i])) layerPoints.Add(new Vector3Int(size - 1 - layer - 4, i - 4, 0)); // 하단
                if (Match(grid[i, layer])) layerPoints.Add(new Vector3Int(i - 4, layer - 4, 0));          // 좌측
                if (Match(grid[i, size - 1 - layer])) layerPoints.Add(new Vector3Int(i - 4, size - 1 - layer - 4, 0)); // 우측
            } 

            if (layerPoints.Count > 0)
                groupedPoints.Add(layerPoints);
        }

        // ▶ 각 레이어를 순차적으로 공격
        foreach (var group in groupedPoints)
        {
            boss.BombHandler.ExecuteFixedBomb(
                group,
                new Vector3Int(4, 4, 0),
                _tutorialBossAttack,
                warningDuration: 1f,
                explosionDuration: 0.3f,
                damage: _damage,
                patternName:PatternName
            );
            boss.StartCoroutine(SlimeSoundEffect());
            yield return new WaitForSeconds(boss.Beat);
        }
    }

    private bool Match(int gridValue)
    {
        return (_isOddNumber && gridValue == 1) || (!_isOddNumber && gridValue == 0);
    }
    
    public IEnumerator SlimeSoundEffect()
    {
        yield return new WaitForSeconds(1f);
        SoundManager.Instance.SlimeSoundClip("PoisonBallActivate");
        yield return new WaitForSeconds(0.1f);
        SoundManager.Instance.SlimeSoundClip("PoisionExplotionActivate");
    }
}
