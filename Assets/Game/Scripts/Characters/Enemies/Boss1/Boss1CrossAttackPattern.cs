using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 패턴 4: 십자 공격
/// </summary>
public class Boss1CrossAttackPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;
    
    public string PatternName => "Cross Attack";
    
    public Boss1CrossAttackPattern(GameObject warningTilePrefab, GameObject explosionEffectPrefab)
    {
        _warningTilePrefab = warningTilePrefab;
        _explosionEffectPrefab = explosionEffectPrefab;
    }
    
    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteCrossAttack(boss));
    }
    
    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && boss.Player != null && _warningTilePrefab != null;
    }
    
    /// <summary>
    /// 십자 공격 실행
    /// </summary>
    private IEnumerator ExecuteCrossAttack(BaseBoss boss)
    {
        boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);
        
        List<GameObject> warningTiles = new List<GameObject>();
        List<Vector3> attackPositions = new List<Vector3>();
        
        // 가로 라인
        for (int x = 0; x < boss.GridSystem.Width; x++)
        {
            Vector3 pos = boss.GridSystem.GetWorldPosition(x, playerY);
            attackPositions.Add(pos);
            warningTiles.Add(TileObject.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
        }
        
        // 세로 라인
        for (int y = 0; y < boss.GridSystem.Height; y++)
        {
            if (y != playerY)
            {
                Vector3 pos = boss.GridSystem.GetWorldPosition(playerX, y);
                attackPositions.Add(pos);
                warningTiles.Add(TileObject.Instantiate(_warningTilePrefab, pos, Quaternion.identity));
            }
        }
        
        yield return new WaitForSeconds(0.8f);
        
        boss.GridSystem.GetXY(boss.Player.transform.position, out int currentX, out int currentY);
        if (currentX == playerX || currentY == playerY)
        {
            boss.ApplyDamageToPlayer(15);
        }
        
        foreach (Vector3 pos in attackPositions)
        {
            boss.CreateDamageEffect(pos, _explosionEffectPrefab);
        }
        
        foreach (GameObject tile in warningTiles)
        {
            TileObject.Destroy(tile);
        }
    }
}