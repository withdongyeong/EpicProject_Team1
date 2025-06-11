using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격 위치 계산 및 패턴 처리를 담당하는 유틸리티 클래스
/// </summary>
public static class AttackPositionCalculator
{
    /// <summary>
    /// 중심점 기준 범위 내 모든 위치 반환
    /// </summary>
    /// <param name="centerPos">중심 위치</param>
    /// <param name="range">범위 (0이면 중심만, 1이면 3x3, 2면 5x5)</param>
    /// <returns>범위 내 모든 유효한 위치</returns>
    public static List<Vector3Int> GetAreaPositions(Vector3Int centerPos, int range)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                Vector3Int pos = new Vector3Int(centerPos.x + x, centerPos.y + y, 0);
                if (GridManager.Instance.IsWithinGrid(pos))
                {
                    positions.Add(pos);
                }
            }
        }
        
        return positions;
    }
    
    /// <summary>
    /// 패턴을 중심 위치에 적용하여 실제 좌표 반환
    /// </summary>
    /// <param name="pattern">적용할 패턴 (상대 좌표)</param>
    /// <param name="centerPosition">중심 위치</param>
    /// <returns>패턴이 적용된 실제 위치들</returns>
    public static List<Vector3Int> ApplyPatternToPosition(List<Vector3Int> pattern, Vector3Int centerPosition)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        
        foreach (Vector3Int offset in pattern)
        {
            Vector3Int targetPos = new Vector3Int(
                centerPosition.x + offset.x,
                centerPosition.y + offset.y,
                centerPosition.z + offset.z
            );
            
            if (GridManager.Instance.IsWithinGrid(targetPos))
            {
                positions.Add(targetPos);
            }
        }
        
        return positions;
    }
    
    /// <summary>
    /// 랜덤 격자 위치들 반환
    /// </summary>
    /// <param name="count">생성할 위치 개수</param>
    /// <returns>중복되지 않는 랜덤 위치들</returns>
    public static List<Vector3Int> GetRandomGridPositions(int count)
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        List<Vector3Int> availablePositions = GetAllValidGridPositions();
        
        for (int i = 0; i < count && availablePositions.Count > 0; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);
            positions.Add(availablePositions[randomIndex]);
            availablePositions.RemoveAt(randomIndex);
        }
        
        return positions;
    }
    
    /// <summary>
    /// 모든 유효한 격자 위치 반환
    /// </summary>
    /// <returns>그리드 내 모든 유효한 위치</returns>
    public static List<Vector3Int> GetAllValidGridPositions()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        Vector3Int gridSize = GridManager.Instance.GridSize;
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (GridManager.Instance.IsWithinGrid(pos))
                {
                    positions.Add(pos);
                }
            }
        }
        
        return positions;
    }
}