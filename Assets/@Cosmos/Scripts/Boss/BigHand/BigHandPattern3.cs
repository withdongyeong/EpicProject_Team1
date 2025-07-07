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
                    boss.StartCoroutine(PlayAttackSound());

                    boss.BombHandler.ExecuteFixedBomb(
                        new List<Vector3Int> { targetPos }, 
                        centerPos, 
                        _attackEffectPrefab,
                        warningDuration: 0.8f, 
                        explosionDuration: 0.6f, 
                        damage: _damage, 
                        warningType: WarningType.Type1
                    );
                }
                
                yield return new WaitForSeconds(0.04f);
            }
            yield return new WaitForSeconds(0.2f);
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

    public IEnumerator PlayAttackSound()
    {
        yield return new WaitForSeconds(0.8f); // 예시로 빈 코루틴 반환
        SoundManager.Instance.BigHandSoundClip("BigHandAttackActivate");
    }

}