using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class SummonGoblinPattern : IBossAttackPattern
{
    private GameObject _goblin;
    private int _goblinCount;
    private Transform _transform;
    public string PatternName => "SummonGoblinPattern";

    public SummonGoblinPattern(GameObject goblin, int goblinCount, Transform transform)
    {
        _goblin = goblin;
        _goblinCount = goblinCount;
        _transform = transform;
    }
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(SummonGoblin(boss));
    }
    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.BombHandler.PlayerController != null && _goblin != null;
    }


   /// <summary>
   /// ������ ��ȯ ����
   /// </summary>
   /// <param name="boss"></param>
   /// <returns></returns>
    private IEnumerator SummonGoblin(BaseBoss boss)
    {
        for (int i = 0; i < _goblinCount; i++)
        {
            // 1. ����
            int randomNumber = Random.Range(0, 3);
            Vector3 SummonPoint =new Vector3();
            if (randomNumber == 0)
            {
                SummonPoint = new Vector3(3,0,0);
            }
            else if (randomNumber == 1)
            {
                SummonPoint = new Vector3(0, -3, 0);
            }
            else if (randomNumber == 2)
            {
                SummonPoint = new Vector3(0, 3, 0);
            }

            GameObject Goblin = TileObject.Instantiate(_goblin, _transform.position + SummonPoint, Quaternion.identity);

            yield return new WaitForSeconds(0.3f);
        }
        yield return 0;
    }
}
