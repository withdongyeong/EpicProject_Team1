using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningKnightSpeardAttack : IBossAttackPattern
{
    private GameObject _lightningActtck;
    private GameObject _lightningball;
    private int _damage;
    private Vector3Int _centerPos;
    public string PatternName => "8_6";

    /// <summary>
    /// 보스 생성자
    /// </summary>
    public LightningKnightSpeardAttack(GameObject lightningActtck,GameObject lightningball, Vector3Int centerPos, int damage)
    {
        _lightningActtck = lightningActtck;
        _lightningball = lightningball;
        _centerPos = centerPos;
        _damage = damage;
    }

    /// <summary>
    /// 패턴 실행
    /// </summary>
    /// <param name="boss">보스 객체</param>
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(lightningActtck(boss, _centerPos));
    }

    /// <summary>
    /// 패턴 실행 가능 여부 확인
    /// </summary>
    /// <param name="boss">보스 객체</param>
    /// <returns>실행 가능 여부</returns>
    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler.PlayerController != null && _lightningActtck != null && boss.BombHandler != null;
    }

    public IEnumerator lightningActtck(BaseBoss boss, Vector3Int centerPos)
    {
        Vector3Int[] directions = new Vector3Int[]
        {
        new Vector3Int(0, 1, 0),   
        new Vector3Int(1, 0, 0),   
        new Vector3Int(0, -1, 0),  
        new Vector3Int(-1, 0, 0),  
        new Vector3Int(-1, 1, 0),  
        new Vector3Int(1, 1, 0),  
        new Vector3Int(-1, -1, 0),  
        new Vector3Int(1, -1, 0)
        };

        boss.BombHandler.ExecuteFixedBomb(new List<Vector3Int>() {new Vector3Int(0,0,0)}, centerPos, _lightningball,
                                             warningDuration: 0.5f, explosionDuration: 5.0f, damage: 0, warningType: WarningType.Type3, patternName:PatternName);

        // 9칸까지 확장 (거리 = 0~8)
        for (int dist = 0; dist < 9; dist++)
        {
            List<Vector3Int> result = new List<Vector3Int>();

            foreach (var dir in directions)
            {
                Vector3Int next = dir * dist;
                if (next == new Vector3Int(0, 0, 0)) continue;

                result.Add(next);
            }

            // 공격 실행
            boss.BombHandler.ExecuteFixedBomb(result, centerPos, _lightningActtck,
                                              warningDuration: 0.8f, explosionDuration: 1f, damage: _damage, warningType:WarningType.Type1, patternName:PatternName);

            yield return new WaitForSeconds(0.1f);
        }

        yield return null;
    }
}
