using UnityEngine;
using System.Collections;

public class DiagonalCrossPattern : IBossAttackPattern
{
    private GameObject _explosionEffectPrefab;

    public string PatternName => "Diagonal Cross Attack";

    public DiagonalCrossPattern(GameObject explosionEffectPrefab)
    {
        _explosionEffectPrefab = explosionEffectPrefab;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteDiagonalCrossAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return AttackPreviewManager.Instance != null && _explosionEffectPrefab != null;
    }

    /// <summary>
    /// 대각선 + 가로 공격 실행
    /// </summary>
    private IEnumerator ExecuteDiagonalCrossAttack(BaseBoss boss)
    {
        // 1단계: 플레이어 위치 기준 대각선 공격 (↘ 방향)
        AttackPreviewManager.Instance.CreatePlayerTargetingCustomPattern(
            pattern: AttackPatterns.DiagonalLineDownRight,
            attackPrefab: _explosionEffectPrefab,
            previewDuration: 0.8f,
            damage: 10,
            onAttackComplete: () => {
                boss.AttackAnimation();
            }
        );

        yield return new WaitForSeconds(1.4f); // 전조 + 공격 완료 대기

        // 2단계: 현재 플레이어 위치 기준 가로 공격
        AttackPreviewManager.Instance.CreatePlayerTargetingCustomPattern(
            pattern: AttackPatterns.HorizontalLine,
            attackPrefab: _explosionEffectPrefab,
            previewDuration: 0.8f,
            damage: 15,
            onAttackComplete: () => {
                boss.AttackAnimation();
            }
        );
    }
}