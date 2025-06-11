
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 거미줄 실 패턴 - 특정 위치에서 가로로 확장되는 공격
/// </summary>
public class ArachneSpiderSilkPattern : IBossAttackPattern
{
    private GameObject _spiderSilkPrefab;
    private int _spiderSilkCount;

    public string PatternName => "SpiderSilk";

    public ArachneSpiderSilkPattern(GameObject spiderSilkPrefab, int spiderSilkCount)
    {
        _spiderSilkPrefab = spiderSilkPrefab;
        _spiderSilkCount = spiderSilkCount;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(SpiderSilkAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return AttackPreviewManager.Instance != null && _spiderSilkPrefab != null;
    }

    /// <summary>
    /// 실 공격 - 오른쪽 끝에서 가로줄로 확장
    /// </summary>
    private IEnumerator SpiderSilkAttack(BaseBoss boss)
    {
        for (int i = 0; i < _spiderSilkCount; i++)
        {
            // 랜덤 Y 위치, X는 격자 끝(오른쪽)에서 시작
            int randomY = Random.Range(0, 8);
            
            // 가로줄 전체를 공격 패턴으로 생성
            List<Vector3Int> horizontalLine = CreateHorizontalLinePattern(randomY);
            
            AttackPreviewManager.Instance.CreateSpecificPositionAttack(
                gridPositions: horizontalLine,
                attackPrefab: _spiderSilkPrefab,
                previewDuration: 0.5f,
                damage: 10,
                onAttackComplete: () => {
                    SoundManager.Instance?.ArachneSoundClip("SpiderSilkActivate");
                }
            );

            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// 특정 Y 좌표의 가로줄 패턴 생성
    /// </summary>
    private List<Vector3Int> CreateHorizontalLinePattern(int y)
    {
        List<Vector3Int> pattern = new List<Vector3Int>();
        
        // X축 전체를 가로지르는 선
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
}