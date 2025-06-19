using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 아라크네 거미 소환 패턴 - GameManager를 통해 플레이어 참조
/// </summary>
public class ArachneSummonSpiderPattern : IBossAttackPattern
{
    private List<GameObject> _summonSpiders;
    private int _spiderCount;
    private float cellSize = 1f;
    
    public string PatternName => "ArachneSummonSpider";

    /// <summary>
    /// 거미 소환 공격 패턴 생성자
    /// </summary>
    /// <param name="summonSpiders">소환할 거미 프리팹 리스트</param>
    /// <param name="spiderCount">소환할 거미 수</param>
    public ArachneSummonSpiderPattern(List<GameObject> summonSpiders, int spiderCount)
    {
        _summonSpiders = summonSpiders;
        _spiderCount = spiderCount;
    }

    /// <summary>
    /// 패턴 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(SummonSpider(boss));
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        StageHandler stageHandler = GameObject.FindAnyObjectByType<StageHandler>();
        return boss.GridSystem != null && 
               stageHandler != null && 
               stageHandler.Player != null && 
               _summonSpiders != null && 
               _summonSpiders.Count > 0;
    }

    /// <summary>
    /// 직선으로 움직이는 거미를 거미리스트에서 랜덤으로 꺼내서 소환 
    /// 소환된 거미는 직선으로 빠르게 움직임
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator SummonSpider(BaseBoss boss)
    {   
        StageHandler stageHandler = GameObject.FindAnyObjectByType<StageHandler>();
        if (stageHandler == null || stageHandler.Player == null)
        {
            Debug.LogError("StageHandler or Player not found for ArachneSummonSpiderPattern!");
            yield break;
        }

        for (int i = 0; i < _spiderCount; i++)
        {
            // 랜덤 Y 위치 선택 (0~7)
            int Y = Random.Range(0, 8);
            
            // 그리드 오른쪽 끝에서 소환 (X=8)
            Vector3 pos = GridManager.Instance.GridToWorldPosition(new Vector3Int(8, Y, 0));

            // 랜덤 거미 선택
            GameObject randomSpider = _summonSpiders[Random.Range(0, _summonSpiders.Count)];
            
            // 거미 소환
            GameObject spider = Object.Instantiate(randomSpider, pos + new Vector3(cellSize, 0, 0), Quaternion.identity);

            Debug.Log($"Spider {i + 1}/{_spiderCount} summoned at position Y={Y} ({spider.name})");

            yield return new WaitForSeconds(0.3f);
        }
        
        Debug.Log($"ArachneSummonSpiderPattern completed: {_spiderCount} spiders summoned");
    }
}