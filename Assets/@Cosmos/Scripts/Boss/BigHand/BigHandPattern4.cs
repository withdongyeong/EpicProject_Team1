using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종 보스 패턴4 - 체스판 패턴
/// </summary>
public class BigHandPattern4 : IBossAttackPattern
{
    private GameObject _attackEffectPrefab;
    private int _damage;    
    private bool _isSoundCoolTime = false;
    public string PatternName => "패턴4_체스판";
    
    public BigHandPattern4(GameObject attackEffectPrefab, int damage)
    {
        _attackEffectPrefab = attackEffectPrefab;
        _damage = damage;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && 
               _attackEffectPrefab != null && 
               GridManager.Instance != null &&
               boss.BombHandler != null;
    }

    /// <summary>
    /// 체스판 패턴 - 격자무늬로 안전지대 제공
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");
        
        // 1단계: 검은 칸들 (x+y가 짝수)
        List<Vector3Int> blackSquares = new List<Vector3Int>();
        for (int x = 1; x <= 7; x++)
        {
            for (int y = 1; y <= 7; y++)
            {
                if ((x + y) % 2 == 0)
                {
                    blackSquares.Add(new Vector3Int(x - 4, y - 4, 0));
                }
            }
        }

        boss.StartCoroutine(PlayAttackSound(boss, boss.Beat));

        boss.BombHandler.ExecuteFixedBomb(
            blackSquares, 
            new Vector3Int(4, 4, 0), 
            _attackEffectPrefab,
            warningDuration: 1f, 
            explosionDuration: 0.8f, 
            damage: _damage, 
            warningType: WarningType.Type1
        );
        
        yield return new WaitForSeconds(boss.Beat * 2);
        
        // 2단계: 흰 칸들 (x+y가 홀수)
        List<Vector3Int> whiteSquares = new List<Vector3Int>();
        for (int x = 1; x <= 7; x++)
        {
            for (int y = 1; y <= 7; y++)
            {
                if ((x + y) % 2 == 1)
                {
                    whiteSquares.Add(new Vector3Int(x - 4, y - 4, 0));
                }
            }
        }
        
        boss.StartCoroutine(PlayAttackSound(boss, boss.Beat));

        boss.BombHandler.ExecuteFixedBomb(
            whiteSquares, 
            new Vector3Int(4, 4, 0), 
            _attackEffectPrefab,
            warningDuration: 1f, 
            explosionDuration: 0.8f, 
            damage: _damage, 
            warningType: WarningType.Type1
        );
        
        yield return new WaitForSeconds(boss.Beat * 2);
    }

    public void Cleanup()
    {
        Debug.Log("패턴4 정리 완료");
    }

    public IEnumerator PlayAttackSound(BaseBoss boss, float coolTime)
    {
        if (_isSoundCoolTime)
        {
            yield break; // 쿨타임 중이면 실행하지 않음
        }
        boss.StartCoroutine(SoundPlay());
        boss.StartCoroutine(SetSoundCoolTime(coolTime));
    }

    public IEnumerator SoundPlay()
    {
        yield return new WaitForSeconds(1f); // 예시로 빈 코루틴 반환
        SoundManager.Instance.BigHandSoundClip("BigHandAttackActivate");
    }

    public IEnumerator SetSoundCoolTime(float isCoolTime)
    {
        _isSoundCoolTime = true;
        yield return new WaitForSeconds(isCoolTime);
        _isSoundCoolTime = false;
    }

}