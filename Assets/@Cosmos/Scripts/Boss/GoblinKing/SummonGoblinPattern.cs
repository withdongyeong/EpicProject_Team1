using UnityEngine;
using System.Collections;

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

    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(SummonGoblin(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _goblin != null && _transform != null;
    }

    /// <summary>
    /// 고블린 소환 패턴
    /// </summary>
    private IEnumerator SummonGoblin(BaseBoss boss)
    {
        for (int i = 0; i < _goblinCount; i++)
        {
            // 랜덤 소환 위치 선택
            int randomNumber = Random.Range(0, 3);
            Vector3 summonPoint = Vector3.zero;
            
            if (randomNumber == 0)
            {
                summonPoint = new Vector3(3, 0, 0);   // 오른쪽
            }
            else if (randomNumber == 1)
            {
                summonPoint = new Vector3(0, -3, 0);  // 아래
            }
            else if (randomNumber == 2)
            {
                summonPoint = new Vector3(0, 3, 0);   // 위
            }

            GameObject goblin = Object.Instantiate(_goblin, _transform.position + summonPoint, Quaternion.identity);

            yield return new WaitForSeconds(0.3f);
        }
    }
}