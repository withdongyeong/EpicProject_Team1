using UnityEngine;
using System.Collections;
public class ArachneSpiderSilkPattern : IBossAttackPattern
{
    private GameObject _spiderSilkPrefeb;
    private int _spiderSilkCount;
    private Transform _arachneTransform;

    public int gridSize = 8;
    public float cellSize = 1f;


    public string PatternName => "SpiderSilk";

    /// <summary>
    /// �� ����� ������
    /// </summary>
    public ArachneSpiderSilkPattern(GameObject spiderSilkPrefeb, int spiderSilkCount , Transform ArachneTransform)
    {
        _spiderSilkPrefeb = spiderSilkPrefeb;
        _spiderSilkCount = spiderSilkCount;
        _arachneTransform = ArachneTransform;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(SpiderSilk(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.Player != null && _spiderSilkPrefeb != null;
    }

    /// <summary>
    /// �ǿ� ������ �÷��̾� ������ ������ �̵�
    /// </summary>
    /// <param name="boss"></param>
    /// <returns></returns>
    private IEnumerator SpiderSilk(BaseBoss boss)
    {
        for (int i = 0; i< _spiderSilkCount; i++)
        {
            int column = Random.Range(-4, 3);
            Vector3 spiderSilkPos = _arachneTransform.position + new Vector3(-7, column * cellSize, 0);
            GameObject spiderSilk = ItemObject.Instantiate(_spiderSilkPrefeb, spiderSilkPos, Quaternion.identity);

            // �ʱ� ������
            spiderSilk.transform.localScale = new Vector3(0.1f, 1, 1);

            // ������ ������Ʈ�� �ʱⰪ ����
            SpiderSilk silkScript = spiderSilk.GetComponent<SpiderSilk>();
            if (silkScript != null)
            {
                silkScript.Init(spiderSilkPos, gridSize); // �ʿ� �Ķ���� ����
            }
            yield return 0;
        }
    }
}

