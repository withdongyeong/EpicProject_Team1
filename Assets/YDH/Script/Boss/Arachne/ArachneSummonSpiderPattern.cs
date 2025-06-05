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
    /// �Ź� ��ȯ ���� ������
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
    /// �������� �����̴� �Ź̸� �Ź̸���Ʈ���� �������� ������ ��ȯ 
    /// ��ȯ�� �Ź̴� �������� ������ ������
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
            GameObject tentacle = ItemObject.Instantiate(randomSpider, tentaclePos, Quaternion.identity);

            yield return new WaitForSeconds(0.3f);
        }
    }
}
