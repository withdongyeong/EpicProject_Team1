using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 아라크네 패턴1 - BombManager 사용 버전
/// </summary>
public class ArachnePattern1 : IBossAttackPattern
{
    private GameObject _spiderLegPrefab;

    public string PatternName => "ArachnePattern1";

    /// <summary>
    /// 아라크네 패턴1 생성자
    /// </summary>
    /// <param name="spiderLegPrefab">거미 다리 이펙트 프리팹</param>
    public ArachnePattern1(GameObject spiderLegPrefab)
    {
        _spiderLegPrefab = spiderLegPrefab;
    }

    /// <summary>
    /// 패턴 실행 - 두 개의 대각선 슬래쉬 동시 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        // 두 개의 코루틴을 동시에 시작
        Coroutine slash1 = boss.StartCoroutine(SpiderSlash1(boss));
        Coroutine slash2 = boss.StartCoroutine(SpiderSlash2(boss));
        
        // 두 코루틴이 모두 완료될 때까지 대기
        yield return slash1;
        yield return slash2;
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        
        var test1 = boss.BombManager;
        var test2 = boss.BombManager.PlayerController;
        var test3 = _spiderLegPrefab;
        var test4 = boss.BombManager != null && 
                    boss.BombManager.PlayerController != null && 
                    _spiderLegPrefab != null;
        return boss.BombManager != null && 
               boss.BombManager.PlayerController != null && 
               _spiderLegPrefab != null;
    }

    /// <summary>
    /// 첫 번째 대각선 슬래쉬 (우하향 ↘)
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator SpiderSlash1(BaseBoss boss)
    {
        for (int i = 0; i < 9; i++)
        {
            int centerX = i;
            int centerY = 8 - i;
            Vector3Int centerPos = new Vector3Int(centerX, centerY, 0);

            // 대각선 5칸 모양 생성 (↘ 방향)
            List<Vector3Int> slashShape = new List<Vector3Int>();
            for (int j = -2; j <= 2; j++)
            {
                slashShape.Add(new Vector3Int(j, j, 0)); // 상대 좌표
            }

            // BombManager로 고정 위치 공격 실행
            boss.BombManager.ExecuteFixedBomb(slashShape, centerPos, _spiderLegPrefab,
                                              warningDuration: 0.8f, explosionDuration: 0.3f, damage: 20);

            // 사운드 재생
            SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");
            
            // 공격 애니메이션
            boss.AttackAnimation();

            yield return new WaitForSeconds(0.3f); // 0.2 + 0.1 (기존 타이밍)
        }
    }

    /// <summary>
    /// 두 번째 대각선 슬래쉬 (좌하향 ↙)
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator SpiderSlash2(BaseBoss boss)
    {
        for (int i = 0; i < 9; i++)
        {
            int centerX = 8 - i;
            int centerY = 8 - i;
            Vector3Int centerPos = new Vector3Int(centerX, centerY, 0);

            // 대각선 5칸 모양 생성 (↙ 방향)
            List<Vector3Int> slashShape = new List<Vector3Int>();
            for (int j = -2; j <= 2; j++)
            {
                slashShape.Add(new Vector3Int(j, -j, 0)); // 상대 좌표
            }

            // BombManager로 고정 위치 공격 실행
            boss.BombManager.ExecuteFixedBomb(slashShape, centerPos, _spiderLegPrefab,
                                              warningDuration: 0.8f, explosionDuration: 0.3f, damage: 20);

            // 사운드 재생
            SoundManager.Instance.ArachneSoundClip("SpiderLegActivate");
            
            // 공격 애니메이션
            boss.AttackAnimation();

            yield return new WaitForSeconds(0.3f); // 0.2 + 0.1 (기존 타이밍)
        }
    }
}