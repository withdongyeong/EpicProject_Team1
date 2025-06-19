using System.Collections;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 수정된 아라크네 독 분출 패턴 - 새로운 BombAvoidanceHandler 사용
/// </summary>
public class ArachnePoisionAriaPattern : IBossAttackPattern
{
    private GameObject _explosionEffectPrefab;
    private List<Vector3Int> _attackShape;
    
    public string PatternName => "ArachnePoisionAria";

    /// <summary>
    /// 아라크네 독 분출 패턴 생성자
    /// </summary>
    /// <param name="explosionEffectPrefab">폭발 이펙트 프리팹</param>
    /// <param name="bombManager">폭탄 피하기 매니저</param>
    public ArachnePoisionAriaPattern(GameObject explosionEffectPrefab)
    {
        _explosionEffectPrefab = explosionEffectPrefab;
        
        // 3x3 공격 모양 설정
        _attackShape = new List<Vector3Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                _attackShape.Add(new Vector3Int(x, y, 0));
            }
        }
    }

    /// <summary>
    /// 패턴 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        // 사운드 재생
        SoundManager.Instance.ArachneSoundClip("PoisonBallActivate");
        
        // 플레이어 추적 폭탄 공격 실행
        boss.BombHandler.ExecuteTargetingBomb(_attackShape, _explosionEffectPrefab, 
                                          warningDuration: 0.6f, explosionDuration: 0.7f, damage: 10);
        
        // 또는 지연 후 재생
        yield return boss.StartCoroutine(PlayExplosionSoundDelayed());
    }

    /// <summary>
    /// 폭발 사운드 지연 재생
    /// </summary>
    private System.Collections.IEnumerator PlayExplosionSoundDelayed()
    {
        yield return new WaitForSeconds(0.6f); // 경고 시간 후
        SoundManager.Instance.ArachneSoundClip("PoisionExplotionActivate");
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler.PlayerController != null && 
               _explosionEffectPrefab != null && 
               boss.BombHandler != null;
    }
}