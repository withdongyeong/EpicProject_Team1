using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 거미 소환 패턴 - 격자 가장자리에서 거미들을 소환
/// </summary>
public class ArachneSummonSpiderPattern : IBossAttackPattern
{
    private List<GameObject> _summonSpiders;
    private int _spiderCount;

    public string PatternName => "ArachneSummonSpider";

    public ArachneSummonSpiderPattern(List<GameObject> summonSpiders, int spiderCount)
    {
        _summonSpiders = summonSpiders;
        _spiderCount = spiderCount;
    }

    public void Execute(BaseBoss boss)
    {
        // 거미 소환은 직접적인 공격이 아니라 소환이므로 AttackPreviewManager 없이 처리
        boss.StartCoroutine(SummonSpiders(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _summonSpiders != null && _summonSpiders.Count > 0;
    }

    /// <summary>
    /// 격자 오른쪽 가장자리에서 거미들을 소환
    /// </summary>
    private IEnumerator SummonSpiders(BaseBoss boss)
    {
        for (int i = 0; i < _spiderCount; i++)
        {
            // 오른쪽 가장자리 랜덤 Y 위치
            int randomY = Random.Range(0, 8);
            Vector3 pos = GridManager.Instance.GridToWorldPosition(new Vector3Int(8, randomY, 0));

            // 랜덤 거미 선택
            GameObject randomSpider = _summonSpiders[Random.Range(0, _summonSpiders.Count)];
            GameObject spawnedSpider = Object.Instantiate(randomSpider, pos + new Vector3(1f, 0, 0), Quaternion.identity);

            yield return new WaitForSeconds(0.3f);
        }
    }
}