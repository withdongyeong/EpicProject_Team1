using UnityEngine;
using System.Collections;

public class ArachneSpiderLegPattern : IBossAttackPattern
{
    private GameObject _spiderLegPrefab;

    public string PatternName => "ArachneSpiderLeg";

    /// <summary>
    /// 거미 다리 패턴 생성자
    /// </summary>
    public ArachneSpiderLegPattern(GameObject spiderLegPrefab)
    {
        _spiderLegPrefab = spiderLegPrefab;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(SpiderLegAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return AttackPreviewManager.Instance != null && _spiderLegPrefab != null;
    }

    /// <summary>
    /// 다리 패턴 - 대각선 공격 2번
    /// </summary>
    private IEnumerator SpiderLegAttack(BaseBoss boss)
    {
        // 첫 번째 대각선 공격 (↘ 방향)
        AttackPreviewManager.Instance.CreatePlayerTargetingCustomPattern(
            pattern: AttackPatterns.DiagonalLineDownRight,
            attackPrefab: _spiderLegPrefab,
            previewDuration: 1f,
            damage: 10,
            onAttackComplete: () => {
                boss.AttackAnimation();
                SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");
            }
        );

        yield return new WaitForSeconds(0.3f);

        // 두 번째 대각선 공격 (↙ 방향)
        AttackPreviewManager.Instance.CreatePlayerTargetingCustomPattern(
            pattern: AttackPatterns.DiagonalLineDownLeft,
            attackPrefab: _spiderLegPrefab,
            previewDuration: 0.5f,
            damage: 10,
            onAttackComplete: () => {
                boss.AttackAnimation();
                SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");
            }
        );
    }
}