using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ArachneSpiderWebPattern : IBossAttackPattern
{
    private GameObject _spiderWebPrefeb;
    private int _spiderWebCount;

    public string PatternName => "ArachneSpiderWeb";

    /// <summary>
    /// 거미줄 설치 패턴 생성자
    /// </summary>
    public ArachneSpiderWebPattern(GameObject spiderWebPrefeb, int spiderWebCount)
    {
        _spiderWebPrefeb = spiderWebPrefeb;
        _spiderWebCount = spiderWebCount;
    }

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteAreaAttack(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.Player != null && _spiderWebPrefeb != null;
    }

    /// <summary>
    /// 거미줄 설치
    /// </summary>
    private IEnumerator ExecuteAreaAttack(BaseBoss boss)
    {
        List<GameObject> warningTiles = new List<GameObject>();

        for (int i = 0; i < _spiderWebCount; i++)
        {
            int X = Random.Range(0, 8);
            int Y = Random.Range(0, 8);

            if (GridManager.Instance.IsWithinGrid(new Vector3Int(X,Y,0)))
            {
                Vector3 pos = GridManager.Instance.GridToWorldPosition(new Vector3Int(X, Y, 0));
                warningTiles.Add(Object.Instantiate(_spiderWebPrefeb, pos, Quaternion.identity));
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
}
