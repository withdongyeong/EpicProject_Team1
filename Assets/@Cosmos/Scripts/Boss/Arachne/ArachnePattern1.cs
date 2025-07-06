using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 아라크네 패턴1 - BombHandler 사용 버전
/// </summary>
public class ArachnePattern1 : IBossAttackPattern
{
    private GameObject _lToRspiderLegPrefab;
    private GameObject _rToLspiderLegPrefab;
    private int _damage;

    public string PatternName => "ArachnePattern1";

    /// <summary>
    /// 아라크네 패턴1 생성자
    /// </summary>
    /// <param name="spiderLegPrefab">거미 다리 이펙트 프리팹</param>
    public ArachnePattern1(GameObject LToRspiderLegPrefab, GameObject RToLspiderLegPrefab, int Damage)
    {
        _lToRspiderLegPrefab = LToRspiderLegPrefab;
        _rToLspiderLegPrefab = RToLspiderLegPrefab;
        _damage = Damage;
    }

    /// <summary>
    /// 패턴 실행 - 두 개의 대각선 슬래쉬 동시 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        Coroutine slash1 = boss.StartCoroutine(SpiderSlash1(boss, true));

        yield return new WaitForSeconds(2f);

        Coroutine slash2 = boss.StartCoroutine(SpiderSlash2(boss, false));

        yield return new WaitForSeconds(2f);

        // 두 개의 코루틴을 동시에 시작
        Coroutine slash3 = boss.StartCoroutine(SpiderSlash1(boss, true));
        Coroutine slash4 = boss.StartCoroutine(SpiderSlash2(boss, true));
        
        // 두 코루틴이 모두 완료될 때까지 대기
        yield return slash3;
        yield return slash4;
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        
        var test1 = boss.BombHandler;
        var test2 = boss.BombHandler.PlayerController;
        var test3 = _lToRspiderLegPrefab;
        var test4 = boss.BombHandler != null && 
                    boss.BombHandler.PlayerController != null &&
                    _lToRspiderLegPrefab != null;
        return boss.BombHandler != null && 
               boss.BombHandler.PlayerController != null &&
               _lToRspiderLegPrefab != null;
    }

    /// <summary>
    /// 첫 번째 대각선 슬래쉬 (우하향 ↘)
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator SpiderSlash1(BaseBoss boss, bool isTorsion)
    {
        for (int i = 0; i < 9; i++)
        {
            int centerX = i;
            int centerY = 8 - i;

            if(isTorsion) centerY = 8 - i;
            else centerY = 8 - i + 1;

            Vector3Int centerPos = new Vector3Int(centerX, centerY, 0);

            // 대각선 5칸 모양 생성 (↘ 방향)
            List<Vector3Int> EffectslashShape = new List<Vector3Int>();
            List<Vector3Int> slashShape = new List<Vector3Int>();
            for (int j = -2; j <= 2; j++)
            {
                if (j == 0) EffectslashShape.Add(new Vector3Int(j, j, 0));
                else slashShape.Add(new Vector3Int(j, j, 0)); // 상대 좌표
            }

            //이펙트 데미지
            boss.BombHandler.ExecuteFixedBomb(EffectslashShape, centerPos, _rToLspiderLegPrefab,
                                  warningDuration: 0.8f, explosionDuration: 0.3f, damage: _damage);

            yield return new WaitForSeconds(0.05f);

            // 데미지만
            boss.BombHandler.ExecuteWarningThenDamage(slashShape, centerPos,
                                              warningDuration: 0.8f, damage: _damage);

            // 사운드 재생
            boss.StartCoroutine(PlayDelayedSound("SpiderLegActivate", 0.8f));

            // 공격 애니메이션
            boss.AttackAnimation();

            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// 두 번째 대각선 슬래쉬 (좌하향 ↙)
    /// </summary>
    /// <param name="boss">보스 객체</param>
    private IEnumerator SpiderSlash2(BaseBoss boss, bool isTorsion)
    {
        for (int i = 0; i < 9; i++)
        {
            int centerX = 8 - i;
            int centerY = 8 - i;
         
           if(isTorsion == true) centerY = 8 - i;
           else centerY = 8 - i +1;

            Vector3Int centerPos = new Vector3Int(centerX, centerY, 0);

            // 대각선 5칸 모양 생성 (↙ 방향)
            List<Vector3Int> EffectslashShape = new List<Vector3Int>();
            List<Vector3Int> slashShape = new List<Vector3Int>();
            for (int j = -2; j <= 3; j++)
            {
                if (j == 0) EffectslashShape.Add(new Vector3Int(j, -j, 0));
                else slashShape.Add(new Vector3Int(j, -j, 0)); // 상대 좌표
            }

            //이펙트
            boss.BombHandler.ExecuteFixedBomb(EffectslashShape, centerPos, _lToRspiderLegPrefab,
                                  warningDuration: 0.8f, explosionDuration: 0.3f, damage: _damage);

            yield return new WaitForSeconds(0.05f);

            // 데미지만
            boss.BombHandler.ExecuteWarningThenDamage(slashShape, centerPos,
                                              warningDuration: 0.8f, damage: _damage);

            // 사운드 재생 - 하나만 재생
            if(!isTorsion) boss.StartCoroutine(PlayDelayedSound("SpiderLegActivate", 0.8f));

            // 공격 애니메이션
            boss.AttackAnimation();

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator PlayDelayedSound(string soundName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SoundManager.Instance.ArachneSoundClip(soundName);
    }
}