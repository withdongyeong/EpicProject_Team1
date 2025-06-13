using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 다양한 공격 모양 패턴 예시들
/// </summary>
public static class PatternShapeExamples
{
    /// <summary>
    /// 3x3 사각형 모양
    /// </summary>
    public static List<Vector3Int> Create3x3Square()
    {
        var pattern = new List<Vector3Int>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                pattern.Add(new Vector3Int(x, y, 0));
            }
        }
        return pattern;
    }
    
    /// <summary>
    /// 십자 모양
    /// </summary>
    public static List<Vector3Int> CreateCrossShape()
    {
        return new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0),   // 중앙
            new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),  // 좌우
            new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),  // 상하
            new Vector3Int(2, 0, 0), new Vector3Int(-2, 0, 0),  // 좌우 확장
            new Vector3Int(0, 2, 0), new Vector3Int(0, -2, 0)   // 상하 확장
        };
    }
    
    /// <summary>
    /// 하트 모양
    /// </summary>
    public static List<Vector3Int> CreateHeartShape()
    {
        return new List<Vector3Int>
        {
            // 상단 두 원
            new Vector3Int(-1, 2, 0), new Vector3Int(0, 2, 0), new Vector3Int(1, 2, 0),
            new Vector3Int(-2, 1, 0), new Vector3Int(-1, 1, 0), new Vector3Int(1, 1, 0), new Vector3Int(2, 1, 0),
            // 중간
            new Vector3Int(-1, 0, 0), new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0),
            // 하단 삼각형
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, -2, 0)
        };
    }
    
    /// <summary>
    /// L자 모양
    /// </summary>
    public static List<Vector3Int> CreateLShape()
    {
        return new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0),   // 코너
            new Vector3Int(1, 0, 0), new Vector3Int(2, 0, 0), new Vector3Int(3, 0, 0), // 수평선
            new Vector3Int(0, 1, 0), new Vector3Int(0, 2, 0), new Vector3Int(0, 3, 0)  // 수직선
        };
    }
    
    /// <summary>
    /// 다이아몬드 모양
    /// </summary>
    public static List<Vector3Int> CreateDiamondShape()
    {
        return new List<Vector3Int>
        {
            new Vector3Int(0, 2, 0),   // 상단
            new Vector3Int(-1, 1, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0),
            new Vector3Int(-2, 0, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 0, 0), new Vector3Int(1, 0, 0), new Vector3Int(2, 0, 0),
            new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0),
            new Vector3Int(0, -2, 0)   // 하단
        };
    }
    
    /// <summary>
    /// 원형 모양 (반지름 2)
    /// </summary>
    public static List<Vector3Int> CreateCircleShape()
    {
        var pattern = new List<Vector3Int>();
        
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                // 원의 방정식: x² + y² <= r²
                if (x * x + y * y <= 4) // 반지름 2
                {
                    pattern.Add(new Vector3Int(x, y, 0));
                }
            }
        }
        
        return pattern;
    }
    
    /// <summary>
    /// 단일 점
    /// </summary>
    public static List<Vector3Int> CreateSinglePoint()
    {
        return new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0)
        };
    }
    
    /// <summary>
    /// 수직선
    /// </summary>
    public static List<Vector3Int> CreateVerticalLine(int length)
    {
        var pattern = new List<Vector3Int>();
        int half = length / 2;
        
        for (int y = -half; y <= half; y++)
        {
            pattern.Add(new Vector3Int(0, y, 0));
        }
        
        return pattern;
    }
    
    /// <summary>
    /// 수평선
    /// </summary>
    public static List<Vector3Int> CreateHorizontalLine(int length)
    {
        var pattern = new List<Vector3Int>();
        int half = length / 2;
        
        for (int x = -half; x <= half; x++)
        {
            pattern.Add(new Vector3Int(x, 0, 0));
        }
        
        return pattern;
    }
}