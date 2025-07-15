using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종 보스 패턴3 - 나선형 확산
/// </summary>
public class BigHandPattern3 : IBossAttackPattern
{
    private GameObject _attackEffectPrefab;
    private int _damage;
    private bool _isSoundCoolTime = false;
    public string PatternName => "패턴3_나선확산";
    
    public BigHandPattern3(GameObject attackEffectPrefab, int damage)
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
    /// 나선형 확산 - 시계방향으로 순서대로 터짐
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");
        
        Vector3Int centerPos = new Vector3Int(4, 4, 0);
        
        // 시계방향 순서
        Vector3Int[] spiralOrder = new Vector3Int[]
        {
            new Vector3Int(0, 1, 0),   // 위
            new Vector3Int(1, 1, 0),   // 우상
            new Vector3Int(1, 0, 0),   // 우
            new Vector3Int(1, -1, 0),  // 우하
            new Vector3Int(0, -1, 0),  // 하
            new Vector3Int(-1, -1, 0), // 좌하
            new Vector3Int(-1, 0, 0),  // 좌
            new Vector3Int(-1, 1, 0)   // 좌상
        };

        for (int distance = 1; distance <= 3; distance++)
        {
            for (int i = 0; i < spiralOrder.Length; i++)
            {
                Vector3Int direction = spiralOrder[i];
                Vector3Int targetPos = direction * distance;
                Vector3Int absolutePos = centerPos + targetPos;
                
                if (IsWithin7x7Grid(absolutePos))
                {
                    boss.StartCoroutine(PlayAttackSound(boss, boss.Beat / 8));

                    boss.BombHandler.ExecuteFixedBomb(
                        new List<Vector3Int> { targetPos }, 
                        centerPos, 
                        _attackEffectPrefab,
                        warningDuration: 1f, 
                        explosionDuration: 0.6f, 
                        damage: _damage, 
                        warningType: WarningType.Type1
                    );
                }
                
                yield return new WaitForSeconds(boss.Beat / 8 );
            }
            yield return new WaitForSeconds(boss.Beat / 4);
        }
    }

    private bool IsWithin7x7Grid(Vector3Int position)
    {
        return position.x >= 1 && position.x <= 7 && 
               position.y >= 1 && position.y <= 7;
    }

    public void Cleanup()
    {
        Debug.Log("패턴3 정리 완료");
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