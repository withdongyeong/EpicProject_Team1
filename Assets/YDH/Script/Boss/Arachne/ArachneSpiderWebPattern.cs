using UnityEngine;
using System.Collections;

public class ArachneSpiderWebPattern : IBossAttackPattern
{
    private GameObject _spiderWebPrefeb;
    private int _spiderWebCount;
    private Transform _arachneTransform;

    private float cellSize = 1f;

    public string PatternName => "ArachneSpiderWeb";

    /// <summary>
    /// �Ź��� ��ġ ���� ������
    /// </summary>
    public ArachneSpiderWebPattern(GameObject spiderWebPrefeb, int spiderWebCount, Transform arachneTransform)
    {
        _spiderWebPrefeb = spiderWebPrefeb;
        _spiderWebCount = spiderWebCount;
        _arachneTransform = arachneTransform;
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
    /// �Ź��� ��ġ
    /// </summary>
    private IEnumerator ExecuteAreaAttack(BaseBoss boss)
    {
        for (int i = 0; i < _spiderWebCount; i++)
        {
            int row = Random.Range(-15,-7);
            int column = Random.Range(-4, 3);
            Vector3 tentaclePos = _arachneTransform.position + new Vector3(row * cellSize, column * cellSize, 0);

            GameObject tentacle = ItemObject.Instantiate(_spiderWebPrefeb, tentaclePos, Quaternion.identity);
        }

            yield return 0; 
    }
}
