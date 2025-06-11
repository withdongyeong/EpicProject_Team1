using UnityEngine;

public class ArachnePoisionAriaPattern : IBossAttackPattern
{
    private GameObject _explosionEffectPrefab;
    
    public string PatternName => "ArachnePoisionAria";

    /// <summary>
    /// 독 영역 공격 패턴 생성자
    /// </summary>
    public ArachnePoisionAriaPattern(GameObject explosionEffectPrefab)
    {
        _explosionEffectPrefab = explosionEffectPrefab;
    }

    public void Execute(BaseBoss boss)
    {
        SoundManager.Instance.ArachneSoundClip("PoisonBallActivate");

        // 플레이어 중심 3x3 독 영역 공격
        AttackPreviewManager.Instance.CreatePlayerTargetingAttack(
            range: 1,  // 3x3 영역
            attackPrefab: _explosionEffectPrefab,
            previewDuration: 0.6f,
            damage: 10,
            onAttackComplete: () => {
                boss.AttackAnimation();
                SoundManager.Instance.ArachneSoundClip("PoisionExplotionActivate");
            }
        );
    }

    public bool CanExecute(BaseBoss boss)
    {
        return AttackPreviewManager.Instance != null && _explosionEffectPrefab != null;
    }
}