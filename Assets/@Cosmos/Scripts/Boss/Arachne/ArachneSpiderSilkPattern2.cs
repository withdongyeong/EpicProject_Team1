using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 지속적인 거미줄 실 패턴 - 일정 간격으로 반복 공격
/// </summary>
public class ArachneSpiderSilkPattern2 : MonoBehaviour
{
    private GameObject _spiderSilkPrefab;
    private bool _isActive = false;

    public void Initialize(GameObject spiderSilkPrefab)
    {
        _spiderSilkPrefab = spiderSilkPrefab;
    }

    public void StartPattern()
    {
        if (!_isActive)
        {
            _isActive = true;
            InvokeRepeating(nameof(ExecuteSilkAttack), 1.2f, 5f);
        }
    }

    public void StopPattern()
    {
        _isActive = false;
        CancelInvoke(nameof(ExecuteSilkAttack));
    }

    private void ExecuteSilkAttack()
    {
        if (!_isActive || AttackPreviewManager.Instance == null) return;

        int randomY = Random.Range(0, 8);
        List<Vector3Int> horizontalLine = CreateHorizontalLinePattern(randomY);
        
        AttackPreviewManager.Instance.CreateSpecificPositionAttack(
            gridPositions: horizontalLine,
            attackPrefab: _spiderSilkPrefab,
            previewDuration: 0.8f,
            damage: 15,
            onAttackComplete: () => {
                SoundManager.Instance?.ArachneSoundClip("SpiderSilkActivate");
            }
        );
    }

    private List<Vector3Int> CreateHorizontalLinePattern(int y)
    {
        List<Vector3Int> pattern = new List<Vector3Int>();
        
        for (int x = 0; x < 8; x++)
        {
            Vector3Int pos = new Vector3Int(x, y, 0);
            if (GridManager.Instance.IsWithinGrid(pos))
            {
                pattern.Add(pos);
            }
        }
        
        return pattern;
    }

    private void OnDestroy()
    {
        StopPattern();
    }
}