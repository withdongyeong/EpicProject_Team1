using UnityEngine;
using System.Collections;
public class TurtreeSummonFrog : IBossAttackPattern
{
    private GameObject _frogPrefab;
    private float _summonInterval;

    public string PatternName => "TurtreeSummonFrog";

    /// <summary>
    /// 개구리 패턴
    /// </summary>
    /// <param name="frogPrefab">소환할 개구리 프리팹</param>
    /// <param name="summonInterval">각 소환 사이 간격 (기본값: 0.3초)</param>
    public TurtreeSummonFrog(GameObject frogPrefab, float summonInterval = 0.3f)
    {
        _frogPrefab = frogPrefab;
        _summonInterval = summonInterval;
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && _frogPrefab != null && boss.BombHandler != null;
    }

    /// <summary>
    /// 패턴 실행 - 1행→9행, 9행→1행 순차 소환
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        int R = Random.Range(0, 2);

        if(R == 0) boss.StartCoroutine(SummonFromTopToBottom(boss));
        else boss.StartCoroutine(SummonFromBottomToTop(boss));

        yield return null;
    }

    /// <summary>
    /// 1행 → 9행 순차 소환
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator SummonFromTopToBottom(BaseBoss boss)
    {
        for (int row = 0; row < 9; row++)
        {
            // 보스 위치 기준으로 상대적 위치 계산 (보스 앞부터 뒤까지)
            Vector3 summonPos = boss.transform.position + new Vector3(0, row - 4, 0); // -4~+4 범위

            // 개구리 소환
            GameObject frog = Object.Instantiate(_frogPrefab, summonPos, Quaternion.identity);

            // 다음 소환까지 대기
            if (row < 8) // 마지막이 아니면 대기
            {
                yield return new WaitForSeconds(_summonInterval);
            }
        }
    }

    /// <summary>
    /// 9행 → 1행 순차 소환
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator SummonFromBottomToTop(BaseBoss boss)
    {
        for (int row = 8; row >= 0; row--)
        {
            // 보스 위치 기준으로 상대적 위치 계산 (보스 뒤부터 앞까지)
            Vector3 summonPos = boss.transform.position + new Vector3(0, row - 4, 0); // +4~-4 범위

            // 개구리 소환
            GameObject frog = Object.Instantiate(_frogPrefab, summonPos, Quaternion.identity);
            Debug.Log($"OrcMagePattern1: Frog summoned at relative row {row - 4} (world: {summonPos})");

            // 다음 소환까지 대기
            if (row > 0) // 마지막이 아니면 대기
            {
                yield return new WaitForSeconds(_summonInterval);
            }
        }
    }
}
