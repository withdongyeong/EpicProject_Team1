using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 아라크네 패턴3 - BombManager 사용 버전
/// </summary>
public class ArachnePattern3 : IBossAttackPattern
{
    private GameObject _poisionAriaPrefab;
    private GameObject _lToRspiderLeg;
    private GameObject _rToLspiderLeg;

    public string PatternName => "ArachnePattern3";

    /// <summary>
    /// 아라크네 패턴3 생성자
    /// </summary>
    /// <param name="poisionAriaPrefab">독 이펙트 프리팹</param>
    /// <param name="spiderLegPrefab">거미 다리 이펙트 프리팹</param>
    public ArachnePattern3(GameObject poisionAriaPrefab, GameObject LToRspiderLegPrefab, GameObject RToLspiderLegPrefab)
    {
        _poisionAriaPrefab = poisionAriaPrefab;
        _lToRspiderLeg = LToRspiderLegPrefab;
        _rToLspiderLeg = RToLspiderLegPrefab;
    }

    /// <summary>
    /// 패턴 실행 - 4번 3x3 공격 + 2번 대각선 슬래쉬
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(SpiderLeg(boss));
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombManager != null && 
               boss.BombManager.PlayerController != null && 
               _poisionAriaPrefab != null &&
               _lToRspiderLeg != null;
    }

    /// <summary>
    /// 메인 패턴 루틴
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator SpiderLeg(BaseBoss boss)
    {
        // 1단계: 4번의 3x3 범위 공격 (한 칸 제외)
        for (int i = 0; i < 4; i++)
        {
            boss.StartCoroutine(ExecuteAreaAttack(boss));
            yield return new WaitForSeconds(0.7f);
        }

        yield return new WaitForSeconds(0.9f);

        // 2단계: 첫 번째 대각선 슬래쉬 (↘)
        boss.StartCoroutine(SpiderLeg_DiagonalSlash1(boss));
        yield return new WaitForSeconds(0.2f);

        // 3단계: 두 번째 대각선 슬래쉬 (↙)
        boss.StartCoroutine(SpiderLeg_DiagonalSlash2(boss));
    }

    /// <summary>
    /// 3x3 범위 공격 (한 방향 제외)
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator ExecuteAreaAttack(BaseBoss boss)
    {
        SoundManager.Instance.ArachneSoundClip("PoisonBallActivate");

        // 제외할 방향 랜덤 선택
        Vector2Int[] excludeDirections = {
            new Vector2Int(-1, 0), // 왼쪽
            new Vector2Int(1, 0),  // 오른쪽
            new Vector2Int(0, 1),  // 위
            new Vector2Int(0, -1)  // 아래
        };

        Vector2Int excludeDirection = excludeDirections[Random.Range(0, excludeDirections.Length)];

        // 3x3 영역에서 한 칸 제외한 모양 생성
        List<Vector3Int> attackShape = new List<Vector3Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // 제외할 방향은 스킵
                if ((x == excludeDirection.x && y == excludeDirection.y) || (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1))
                    continue;

                attackShape.Add(new Vector3Int(x, y, 0));
            }
        }

        // 플레이어 추적 공격 (제외된 칸은 안전지대)
        boss.BombManager.ExecuteTargetingBomb(attackShape, _poisionAriaPrefab,
                                              warningDuration: 0.6f, explosionDuration: 0.7f, damage: 10);

        boss.AttackAnimation();
        
        // 폭발 사운드는 0.8초 후 재생
        boss.StartCoroutine(PlayDelayedSound("PoisionExplotionActivate", 0.8f));

        yield return null;
    }

    /// <summary>
    /// 첫 번째 대각선 슬래쉬 (↘ 방향)
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator SpiderLeg_DiagonalSlash1(BaseBoss boss)
    {
        // 대각선 5칸 모양 생성 (↘ 방향)
        List<Vector3Int> EffectslashShape = new List<Vector3Int>();
        List<Vector3Int> slashShape = new List<Vector3Int>();
        for (int i = -2; i <= 2; i++)
        {
            if (i == 0) EffectslashShape.Add(new Vector3Int(i, i, 0));
            else slashShape.Add(new Vector3Int(i, i, 0)); // 상대 좌표
        }

        //플레이어 위치
        Vector3Int PlayerPoint = new Vector3Int(boss.BombManager.PlayerController.CurrentX, boss.BombManager.PlayerController.CurrentY, 0);
        // 플레이어 추적 대각선 공격
        boss.BombManager.ExecuteFixedBomb(EffectslashShape, PlayerPoint, _rToLspiderLeg,
                                              warningDuration: 0.8f, explosionDuration: 0.3f, damage: 20);
        // 데미지만
        boss.BombManager.ExecuteWarningThenDamage(slashShape, PlayerPoint,
                                          warningDuration: 0.8f, damage: 20);

        boss.AttackAnimation();
        
        // 0.35초 후 사운드 재생
        boss.StartCoroutine(PlayDelayedSound("SpiderLegActivate", 0.8f));

        yield return null;
    }

    /// <summary>
    /// 두 번째 대각선 슬래쉬 (↙ 방향)
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator SpiderLeg_DiagonalSlash2(BaseBoss boss)
    {
        // 대각선 5칸 모양 생성 (↙ 방향)
        List<Vector3Int> EffectslashShape = new List<Vector3Int>();
        List<Vector3Int> slashShape = new List<Vector3Int>();
        for (int i = -2; i <= 2; i++)
        {
            if (i == 0) EffectslashShape.Add(new Vector3Int(i, -i, 0));
            else slashShape.Add(new Vector3Int(i, -i, 0)); // 상대 좌표
        }

        //플레이어 위치
        Vector3Int PlayerPoint = new Vector3Int(boss.BombManager.PlayerController.CurrentX, boss.BombManager.PlayerController.CurrentY,0);

        // 플레이어 추적 대각선 공격
        boss.BombManager.ExecuteFixedBomb(EffectslashShape, PlayerPoint, _lToRspiderLeg,
                                                     warningDuration: 0.8f, explosionDuration: 0.3f, damage: 20);
        // 데미지만
        boss.BombManager.ExecuteWarningThenDamage(slashShape, PlayerPoint,
                                          warningDuration: 0.8f, damage: 20);

        boss.AttackAnimation();
        
        boss.StartCoroutine(PlayDelayedSound("SpiderLegActivate", 0.8f));

        yield return null;
    }

    /// <summary>
    /// 지연된 사운드 재생
    /// </summary>
    /// <param name="soundName">사운드 이름</param>
    /// <param name="delay">지연 시간</param>
    private IEnumerator PlayDelayedSound(string soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.ArachneSoundClip(soundName);
    }
}