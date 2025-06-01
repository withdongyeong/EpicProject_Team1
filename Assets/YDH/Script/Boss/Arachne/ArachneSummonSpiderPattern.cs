using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArachneSummonSpiderPattern : IBossAttackPattern
{
    private List<GameObject> _summonSpiders;
    private int _spiderCount;
    private Transform _arachneTransform;

    private float cellSize = 1f;
    public string PatternName => "ArachneSummonSpider";


    /// <summary>
    /// 거미 소환 공격 생성자
    /// </summary>
    public ArachneSummonSpiderPattern(List<GameObject> summonSpiders, int spiderCount, Transform arachneTransform)
    {
        _summonSpiders = summonSpiders;
        _spiderCount = spiderCount;
        _arachneTransform = arachneTransform;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(SummonSpider(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.Player != null && _summonSpiders != null;
    }

    /// <summary>
    /// 직선으로 움직이는 거미를 거미리스트에서 랜덤으로 꺼내서 소환 
    /// 소환된 거미는 직선으로 빠르게 움직임
    /// </summary>
    /// <param name="boss"></param>
    /// <returns></returns>
    private IEnumerator SummonSpider(BaseBoss boss)
    {   
        for(int i = 0; i < _spiderCount; i++)
        {
            int column = Random.Range(-4, 3);
            Vector3 tentaclePos = _arachneTransform.position + new Vector3(-4, column * cellSize, 0);

            GameObject randomSpider = _summonSpiders[Random.Range(0, _summonSpiders.Count)];
            GameObject tentacle = Object.Instantiate(randomSpider, tentaclePos, Quaternion.identity);

            yield return new WaitForSeconds(0.3f);
        }
    }
}
