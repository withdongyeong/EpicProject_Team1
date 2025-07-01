using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 최종 보스 패턴2 - 회전하는 Y자 스포크 패턴
/// </summary>
public class BigHandPattern2 : IBossAttackPattern
{
    private GameObject _attackEffectPrefab;

    public string PatternName => "패턴2_회전Y스포크";
    
    public BigHandPattern2(GameObject attackEffectPrefab)
    {
        _attackEffectPrefab = attackEffectPrefab;
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss != null && 
               _attackEffectPrefab != null && 
               GridManager.Instance != null &&
               boss.BombHandler != null;
    }

    /// <summary>
    /// 회전하는 Y자 패턴 (3개 스포크가 천천히 회전하며 7x7 격자 끝까지 도달)
    /// </summary>
    public IEnumerator Execute(BaseBoss boss)
    {
        boss.SetAnimationTrigger("Attack");
        
        Debug.Log("패턴2 시작 - 회전 Y스포크");
        
        Vector3Int centerPos = new Vector3Int(4, 4, 0);
        
        // 10번 회전 (36도씩, 더 천천히)
        for (int rotation = 0; rotation < 10; rotation++)
        {
            List<Vector3Int> spokesPattern = new List<Vector3Int>();
            
            // 중심점
            spokesPattern.Add(new Vector3Int(0, 0, 0));
            
            // 3개의 스포크 (120도씩 떨어짐)
            for (int spoke = 0; spoke < 3; spoke++)
            {
                float angle = (rotation * 36f + spoke * 120f) * Mathf.Deg2Rad;
                
                // 각 스포크마다 4칸 길이 (7x7 격자 끝까지 도달)
                for (int length = 1; length <= 4; length++)
                {
                    int x = Mathf.RoundToInt(Mathf.Cos(angle) * length);
                    int y = Mathf.RoundToInt(Mathf.Sin(angle) * length);
                    
                    Vector3Int spokePos = new Vector3Int(x, y, 0);
                    Vector3Int absolutePos = centerPos + spokePos;
                    
                    if (IsWithin7x7Grid(absolutePos))
                        spokesPattern.Add(spokePos);
                }
            }
            
            boss.BombHandler.ExecuteFixedBomb(
                spokesPattern, 
                centerPos, 
                _attackEffectPrefab,
                warningDuration: 0.8f, 
                explosionDuration: 0.6f, 
                damage: 20, 
                warningType: WarningType.Type1
            );
            
            // 회전 속도 감소: 0.25초 → 0.45초
            yield return new WaitForSeconds(0.45f);
        }
        
        yield return new WaitForSeconds(0.4f + 0.6f);
        
        Debug.Log("패턴2 완료");
    }

    /// <summary>
    /// 7x7 격자 범위 내에 있는지 확인
    /// </summary>
    private bool IsWithin7x7Grid(Vector3Int position)
    {
        return position.x >= 1 && position.x <= 7 && 
               position.y >= 1 && position.y <= 7;
    }

    public void Cleanup()
    {
        Debug.Log("패턴2 정리 완료");
    }
}