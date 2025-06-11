using System.Collections;
using UnityEngine;

public class TreantVineWhipPattern : IBossAttackPattern
{
    private GameObject _explosionEffectPrefab;
    private int _whipCount;

    public string PatternName => "TreantVineWhip";

    public TreantVineWhipPattern(GameObject explosionEffectPrefab, int whipCount)
    {
        _explosionEffectPrefab = explosionEffectPrefab;
        _whipCount = whipCount;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteVineWhipAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return AttackPreviewManager.Instance != null && _explosionEffectPrefab != null;
    }

    /// <summary>
    /// 덩굴 채찍 공격 - 교대로 양쪽 대각선 공격
    /// </summary>
    private IEnumerator ExecuteVineWhipAttack(BaseBoss boss)
    {
        for (int i = 0; i < _whipCount; i++)
        {
            if (i % 2 == 0)
            {
                // 좌상단-우하단 대각선 (↙ 방향)
                AttackPreviewManager.Instance.CreatePlayerTargetingCustomPattern(
                    pattern: AttackPatterns.DiagonalLineDownLeft,
                    attackPrefab: _explosionEffectPrefab,
                    previewDuration: 0.3f,
                    damage: 15,
                    onAttackComplete: () => {
                        boss.AttackAnimation();
                    }
                );
            }
            else
            {
                // 우상단-좌하단 대각선 (↘ 방향)
                AttackPreviewManager.Instance.CreatePlayerTargetingCustomPattern(
                    pattern: AttackPatterns.DiagonalLineDownRight,
                    attackPrefab: _explosionEffectPrefab,
                    previewDuration: 0.5f,
                    damage: 15,
                    onAttackComplete: () => {
                        boss.AttackAnimation();
                    }
                );
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}