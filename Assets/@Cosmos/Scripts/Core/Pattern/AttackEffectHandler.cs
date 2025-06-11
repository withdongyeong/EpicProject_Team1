using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격 이펙트 및 오브젝트 생성을 담당하는 핸들러 클래스
/// </summary>
public class AttackEffectHandler
{
    private GameObject attackPreviewPrefab;
    private List<GameObject> activePreviewObjects;
    
    public GameObject AttackPreviewPrefab { get => attackPreviewPrefab; set => attackPreviewPrefab = value; }
    
    /// <summary>
    /// 이펙트 핸들러 생성자
    /// </summary>
    /// <param name="previewPrefab">전조 표시에 사용할 프리팹</param>
    public AttackEffectHandler(GameObject previewPrefab)
    {
        attackPreviewPrefab = previewPrefab;
        activePreviewObjects = new List<GameObject>();
    }
    
    /// <summary>
    /// 전조 오브젝트들 생성
    /// </summary>
    /// <param name="gridPositions">전조를 표시할 격자 위치들</param>
    /// <returns>생성된 전조 오브젝트들</returns>
    public List<GameObject> CreatePreviewObjects(List<Vector3Int> gridPositions)
    {
        List<GameObject> previewObjects = new List<GameObject>();
        
        // 게임이 Playing 상태일 때만 생성
        if (GameStateManager.Instance == null || 
            GameStateManager.Instance.CurrentState != GameStateManager.GameState.Playing)
        {
            return previewObjects;
        }
        
        foreach (Vector3Int gridPos in gridPositions)
        {
            if (GridManager.Instance.IsWithinGrid(gridPos))
            {
                Vector3 worldPos = GridManager.Instance.GridToWorldPosition(gridPos);
                GameObject preview = GameObject.Instantiate(attackPreviewPrefab, worldPos, Quaternion.identity);
                previewObjects.Add(preview);
                activePreviewObjects.Add(preview);
            }
        }
        
        return previewObjects;
    }
    
    /// <summary>
    /// 전조 오브젝트들 제거
    /// </summary>
    /// <param name="previewObjects">제거할 전조 오브젝트들</param>
    public void DestroyPreviewObjects(List<GameObject> previewObjects)
    {
        foreach (GameObject obj in previewObjects)
        {
            if (obj != null)
            {
                activePreviewObjects.Remove(obj);
                GameObject.Destroy(obj);
            }
        }
    }
    
    /// <summary>
    /// 공격 이펙트 생성
    /// </summary>
    /// <param name="gridPositions">이펙트를 생성할 격자 위치들</param>
    /// <param name="attackPrefab">사용할 공격 이펙트 프리팹</param>
    public void CreateAttackEffects(List<Vector3Int> gridPositions, GameObject attackPrefab)
    {
        // 게임이 Playing 상태일 때만 생성
        if (GameStateManager.Instance == null || 
            GameStateManager.Instance.CurrentState != GameStateManager.GameState.Playing)
        {
            return;
        }
        
        foreach (Vector3Int gridPos in gridPositions)
        {
            if (GridManager.Instance.IsWithinGrid(gridPos))
            {
                Vector3 worldPos = GridManager.Instance.GridToWorldPosition(gridPos);
                GameObject effect = GameObject.Instantiate(attackPrefab, worldPos, Quaternion.identity);
                GameObject.Destroy(effect, 2f); // 2초 후 자동 제거
            }
        }
    }
    
    /// <summary>
    /// 모든 활성 전조 오브젝트 제거
    /// </summary>
    public void ClearAllPreviews()
    {
        foreach (GameObject obj in activePreviewObjects)
        {
            if (obj != null)
            {
                GameObject.Destroy(obj);
            }
        }
        activePreviewObjects.Clear();
    }
    
    /// <summary>
    /// 현재 활성화된 전조 오브젝트 수 반환
    /// </summary>
    /// <returns>활성 전조 오브젝트 개수</returns>
    public int GetActivePreviewCount()
    {
        return activePreviewObjects.Count;
    }
}