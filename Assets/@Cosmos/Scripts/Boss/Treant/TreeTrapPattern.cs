using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreeTrapPattern : IBossAttackPattern
{
    private GameObject _treeTrapPrefab;

    public string PatternName => "Tree Trap";

    public TreeTrapPattern(GameObject treeTrapPrefab)
    {
        _treeTrapPrefab = treeTrapPrefab;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(TreeTrap(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return AttackPreviewManager.Instance != null && _treeTrapPrefab != null;
    }

    /// <summary>
    /// 나무 함정 생성 - 플레이어 위치 기준 특수 패턴
    /// </summary>
    private IEnumerator TreeTrap(BaseBoss boss)
    {
        // 플레이어 위치 스냅샷 (공격 시점의 위치 고정)
        List<Vector3Int> trapPattern = CreateTreeTrapPattern();
        
        AttackPreviewManager.Instance.CreateSpecificPositionAttack(
            gridPositions: trapPattern,
            attackPrefab: _treeTrapPrefab,
            previewDuration: 0.8f,
            damage: 20,
            onAttackComplete: () => {
                boss.AttackAnimation();
            }
        );

        yield return null;
    }

    /// <summary>
    /// 현재 플레이어 위치 기준 나무 함정 패턴 생성
    /// </summary>
    private List<Vector3Int> CreateTreeTrapPattern()
    {
        List<Vector3Int> pattern = new List<Vector3Int>();
        Vector3Int playerPos = AttackPreviewManager.Instance.GetCurrentPlayerPosition();

        // 가로 라인 (플레이어 Y 좌표 전체)
        for (int x = 0; x < 8; x++)
        {
            pattern.Add(new Vector3Int(x, playerPos.y, 0));
        }

        // 세로 라인 (X=0 위치만, 플레이어 Y 제외)
        for (int y = 0; y < 8; y++)
        {
            if (y != playerPos.y)
            {
                pattern.Add(new Vector3Int(0, y, 0));
            }
        }

        return pattern;
    }
}