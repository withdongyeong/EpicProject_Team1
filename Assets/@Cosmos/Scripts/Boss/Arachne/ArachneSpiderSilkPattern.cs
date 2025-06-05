using UnityEngine;
using System.Collections;
public class ArachneSpiderSilkPattern : IBossAttackPattern
{
    private GameObject _spiderSilkPrefeb;
    private int _spiderSilkCount;

    public int gridSize = 8;
    public float cellSize = 1f;


    public string PatternName => "SpiderSilk";

    /// <summary>
    /// 실 붙잡기 생성자
    /// </summary>
    public ArachneSpiderSilkPattern(GameObject spiderSilkPrefeb, int spiderSilkCount)
    {
        _spiderSilkPrefeb = spiderSilkPrefeb;
        _spiderSilkCount = spiderSilkCount;
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
    /// 실에 잡히면 플레이어 강제로 앞으로 이동
    /// </summary>
    /// <param name="boss"></param>
    /// <returns></returns>
    private IEnumerator SpiderSilk(BaseBoss boss)
    {
        for (int i = 0; i< _spiderSilkCount; i++)
        {
            int Y = Random.Range(0, 8);

            Vector3 pos = GridManager.Instance.GridToWorldPosition(new Vector3Int(8, Y, 0));
            GameObject spiderSilk = GameObject.Instantiate(_spiderSilkPrefeb, pos + new Vector3(cellSize, 0,0), Quaternion.identity);

            // 초기 스케일
            spiderSilk.transform.localScale = new Vector3(0.1f, 1, 1);

            // 생성된 오브젝트에 초기값 전달
            SpiderSilk silkScript = spiderSilk.GetComponent<SpiderSilk>();

            if (silkScript != null)
            {
                silkScript.Init(spiderSilk.transform.position, gridSize); // 필요 파라미터 전달
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}

