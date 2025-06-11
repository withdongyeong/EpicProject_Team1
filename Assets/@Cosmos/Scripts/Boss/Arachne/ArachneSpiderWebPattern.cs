using UnityEngine;

/// <summary>
/// 거미줄 설치 패턴 - 랜덤 위치에 거미줄 설치
/// </summary>
public class ArachneSpiderWebPattern : IBossAttackPattern
{
    private GameObject _spiderWebPrefab;
    private int _spiderWebCount;

    public string PatternName => "ArachneSpiderWeb";

    public ArachneSpiderWebPattern(GameObject spiderWebPrefab, int spiderWebCount)
    {
        _spiderWebPrefab = spiderWebPrefab;
        _spiderWebCount = spiderWebCount;
    }

    public void Execute(BaseBoss boss)
    {
        // 간단하게 랜덤 위치 공격으로 처리
        AttackPreviewManager.Instance.CreateRandomPositionAttack(
            attackCount: _spiderWebCount,
            attackPrefab: _spiderWebPrefab,
            previewDuration: 0.5f,
            damage: 5,
            onAttackComplete: () => {
                SoundManager.Instance?.ArachneSoundClip("SpiderWebPlace");
            }
        );
    }

    public bool CanExecute(BaseBoss boss)
    {
        return AttackPreviewManager.Instance != null && _spiderWebPrefab != null;
    }
}