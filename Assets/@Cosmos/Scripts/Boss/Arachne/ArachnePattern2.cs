using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 아라크네 패턴2 - BombHandler 사용 버전
/// </summary>
public class ArachnePattern2 : IBossAttackPattern
{
    private GameObject _poisionAriaPrefab;
    private List<Vector3Int> _singlePointShape;
    private List<Vector3Int> _bigAttackShape;

    public string PatternName => "ArachnePattern2";

    /// <summary>
    /// 아라크네 패턴2 생성자
    /// </summary>
    /// <param name="poisionAriaPrefab">독 이펙트 프리팹</param>
    public ArachnePattern2(GameObject poisionAriaPrefab)
    {
        _poisionAriaPrefab = poisionAriaPrefab;
        
        // 단일 점 공격 모양
        _singlePointShape = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0)
        };

        // 대형 공격 모양 생성 (체스판 패턴)
        _bigAttackShape = new List<Vector3Int>();
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                // 짝수 좌표는 제외 (체스판 패턴)
                if (x % 2 == 0 && y % 2 == 0) continue;
                
                _bigAttackShape.Add(new Vector3Int(x - 4, y - 4, 0)); // 중심 기준 상대 좌표
            }
        }
    }

    /// <summary>
    /// 패턴 실행 - 6번 추적 공격 + 2번 대형 공격
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(SpiderAttack(boss));
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler != null && 
               boss.BombHandler.PlayerController != null && 
               _poisionAriaPrefab != null;
    }

    /// <summary>
    /// 단발공격 + 범위공격 메인 루틴
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator SpiderAttack(BaseBoss boss)
    {
        boss.BombHandler.ExecuteTargetingBomb(_singlePointShape, _poisionAriaPrefab,
                                                 warningDuration: 0.8f, explosionDuration: 0.7f, damage: 10);

        // 사운드 재생
        boss.StartCoroutine(PlayDelayedSound("PoisionExplotionActivate", 0.8f));
        boss.AttackAnimation();

        yield return new WaitForSeconds(0.3f);

        // 1단계: 6번의 플레이어 추적 공격
        for (int i = 0; i < 5; i++)
        {
            // 플레이어 추적 단일 점 공격
            boss.BombHandler.ExecuteTargetingBomb(_singlePointShape, _poisionAriaPrefab,
                                                  warningDuration: 0.8f, explosionDuration: 0.7f, damage: 10);

            // 사운드 재생
            boss.StartCoroutine(PlayDelayedSound("PoisionExplotionActivate", 0.85f));

            boss.AttackAnimation();

            yield return new WaitForSeconds(0.3f);
        }

        // 2단계: 첫 번째 대형 공격 (가로 절반)
        Coroutine bigAttack1 = boss.StartCoroutine(PoisonBigAttack_LShape(boss));

        // 3단계: 두 번째 대형 공격 (세로 절반)
        //Coroutine bigAttack2 = boss.StartCoroutine(PoisonBigAttack(boss, false));
        
        // 두 번째 대형 공격 완료까지 대기
        yield return bigAttack1;
    }

    /// <summary>
    /// 대형 범위 공격 (절반 영역)
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <param name="isHorizontal">가로 절반(true) vs 세로 절반(false)</param>
    private IEnumerator PoisonBigAttack_LShape(BaseBoss boss)
    {
        int splitValue = 5;
        HashSet<Vector3Int> attackShape = new HashSet<Vector3Int>();

        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                // L자 범위 조건: 좌측 또는 상단
                bool inHorizontal = y < splitValue;
                bool inVertical = x < splitValue;

                if (inHorizontal || inVertical)
                {
                    // 체스판 패턴: 짝수 좌표 제외
                    if (x % 2 == 0 && y % 2 == 0) continue;

                    attackShape.Add(new Vector3Int(x - 4, y - 4, 0)); // 중심 기준 상대 좌표
                }
            }
        }

        Vector3Int centerPos = new Vector3Int(4, 4, 0);

        boss.BombHandler.ExecuteFixedBomb(attackShape.ToList(), centerPos, _poisionAriaPrefab,
                                          warningDuration: 0.8f, explosionDuration: 0.7f, damage: 20);

        boss.StartCoroutine(PlayDelayedSound("PoisionExplotionActivate", 0.8f));

        yield return null;
    }

    private IEnumerator PlayDelayedSound(string soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.ArachneSoundClip(soundName);
    }
}