
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 패턴 2: 그리드 절반 공격 (메테오 애니메이션)
/// </summary>
public class Boss1HalfGridAttackPattern : IBossAttackPattern
{
    private GameObject _warningTilePrefab;
    private GameObject _explosionEffectPrefab;
    private GameObject _meteorPrefab;
    
    public string PatternName => "Half Grid Attack";
    
    public Boss1HalfGridAttackPattern(GameObject warningTilePrefab, GameObject explosionEffectPrefab, GameObject meteorPrefab)
    {
        _warningTilePrefab = warningTilePrefab;
        _explosionEffectPrefab = explosionEffectPrefab;
        _meteorPrefab = meteorPrefab;
    }
    
    public void Execute(BaseBoss boss)
    {
        boss.StartCoroutine(ExecuteHalfGridAttack(boss));
    }
    
    public bool CanExecute(BaseBoss boss)
    {
        return boss.GridSystem != null && _warningTilePrefab != null;
    }
    
    /// <summary>
    /// 절반 그리드 공격 실행
    /// </summary>
    private IEnumerator ExecuteHalfGridAttack(BaseBoss boss)
    {
        bool isHorizontal = Random.value > 0.5f;
        int splitValue = boss.GridSystem.Height / 2;
        
        List<Vector3> attackPositions = new List<Vector3>();
        GameObject[] warningTiles = new GameObject[boss.GridSystem.Width * boss.GridSystem.Height / 2];
        int index = 0;
        
        Vector3 targetCenter = Vector3.zero;
        int positionCount = 0;
        
        for (int x = 0; x < boss.GridSystem.Width; x++)
        {
            for (int y = 0; y < boss.GridSystem.Height; y++)
            {
                bool shouldAttack = isHorizontal ? y < splitValue : x < splitValue;
                
                if (shouldAttack)
                {
                    Vector3 tilePos = boss.GridSystem.GetWorldPosition(x, y);
                    attackPositions.Add(tilePos);
                    warningTiles[index] = TileObject.Instantiate(_warningTilePrefab, tilePos, Quaternion.identity);
                    index++;
                    
                    targetCenter += tilePos;
                    positionCount++;
                }
            }
        }
        
        targetCenter /= positionCount;
        
        if (_meteorPrefab != null)
        {
            boss.StartCoroutine(MeteorAnimation(boss, targetCenter, 1.5f));
        }
        
        yield return new WaitForSeconds(1.5f);
        
        boss.GridSystem.GetXY(boss.Player.transform.position, out int playerX, out int playerY);
        bool isPlayerInArea = isHorizontal ? playerY < splitValue : playerX < splitValue;
        
        if (isPlayerInArea)
        {
            boss.ApplyDamageToPlayer(20);
        }
        
        foreach (Vector3 pos in attackPositions)
        {
            boss.CreateDamageEffect(pos, _explosionEffectPrefab);
        }
        
        foreach (GameObject tile in warningTiles)
        {
            if (tile != null)
            {
                TileObject.Destroy(tile);
            }
        }
    }
    
    /// <summary>
    /// 메테오 애니메이션
    /// </summary>
    private IEnumerator MeteorAnimation(BaseBoss boss, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = boss.transform.position + Vector3.up * 2f;
        GameObject meteor = TileObject.Instantiate(_meteorPrefab, startPosition, Quaternion.identity);

        float riseHeight = 5f;
        Vector3 risePosition = startPosition + Vector3.up * riseHeight;
        float riseTime = duration * 0.2f;

        float elapsed = 0f;
        while (elapsed < riseTime)
        {
            meteor.transform.position = Vector3.Lerp(startPosition, risePosition, elapsed / riseTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        float hoverTime = duration * 0.3f;
        yield return new WaitForSeconds(hoverTime);

        float fallTime = duration * 0.5f;
        elapsed = 0f;
        Vector3 initialScale = meteor.transform.localScale;
        Vector3 finalScale = initialScale * 1.5f;

        while (elapsed < fallTime)
        {
            float t = elapsed / fallTime;
            meteor.transform.position = Vector3.Lerp(risePosition, targetPosition, t);
            meteor.transform.localScale = Vector3.Lerp(initialScale, finalScale, t);
            meteor.transform.Rotate(Vector3.forward, 360f * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        TileObject.Destroy(meteor);
    }
}