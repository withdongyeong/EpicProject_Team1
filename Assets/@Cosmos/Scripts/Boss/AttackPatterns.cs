using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 다양한 공격 패턴들을 미리 정의해둔 유틸리티 클래스
/// </summary>
public static class AttackPatterns
{
    /// <summary>
    /// 십자 모양 패턴
    /// </summary>
    public static List<Vector3Int> Cross => new List<Vector3Int>
    {
        new Vector3Int(0, 0, 0),   // 중심
        new Vector3Int(-1, 0, 0),  // 왼쪽
        new Vector3Int(1, 0, 0),   // 오른쪽
        new Vector3Int(0, -1, 0),  // 아래
        new Vector3Int(0, 1, 0)    // 위
    };
    
    /// <summary>
    /// 대각선 십자 패턴 (X자)
    /// </summary>
    public static List<Vector3Int> DiagonalCross => new List<Vector3Int>
    {
        new Vector3Int(0, 0, 0),   // 중심
        new Vector3Int(-1, -1, 0), // 좌하
        new Vector3Int(1, 1, 0),   // 우상
        new Vector3Int(-1, 1, 0),  // 좌상
        new Vector3Int(1, -1, 0)   // 우하
    };
    
    /// <summary>
    /// 별 모양 패턴 (5방향)
    /// </summary>
    public static List<Vector3Int> Star => new List<Vector3Int>
    {
        new Vector3Int(0, 0, 0),   // 중심
        new Vector3Int(0, 2, 0),   // 위
        new Vector3Int(-2, 1, 0),  // 좌상
        new Vector3Int(-1, -1, 0), // 좌하
        new Vector3Int(1, -1, 0),  // 우하
        new Vector3Int(2, 1, 0)    // 우상
    };
    
    /// <summary>
    /// 대각선 라인 패턴 (↘ 방향)
    /// </summary>
    public static List<Vector3Int> DiagonalLineDownRight => new List<Vector3Int>
    {
        new Vector3Int(-2, -2, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(2, 2, 0)
    };
    
    /// <summary>
    /// 대각선 라인 패턴 (↙ 방향)
    /// </summary>
    public static List<Vector3Int> DiagonalLineDownLeft => new List<Vector3Int>
    {
        new Vector3Int(-2, 2, 0),
        new Vector3Int(-1, 1, 0),
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, -1, 0),
        new Vector3Int(2, -2, 0)
    };
    
    /// <summary>
    /// 수직선 패턴
    /// </summary>
    public static List<Vector3Int> VerticalLine => new List<Vector3Int>
    {
        new Vector3Int(0, -2, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 2, 0)
    };
    
    /// <summary>
    /// 수평선 패턴
    /// </summary>
    public static List<Vector3Int> HorizontalLine => new List<Vector3Int>
    {
        new Vector3Int(-2, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(2, 0, 0)
    };
    
    /// <summary>
    /// L자 패턴
    /// </summary>
    public static List<Vector3Int> LShape => new List<Vector3Int>
    {
        new Vector3Int(0, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 2, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(2, 0, 0)
    };
    
    /// <summary>
    /// T자 패턴
    /// </summary>
    public static List<Vector3Int> TShape => new List<Vector3Int>
    {
        new Vector3Int(0, 0, 0),   // 중심
        new Vector3Int(-1, 0, 0),  // 왼쪽
        new Vector3Int(1, 0, 0),   // 오른쪽
        new Vector3Int(0, 1, 0),   // 위
        new Vector3Int(0, 2, 0)    // 더 위
    };
    
    /// <summary>
    /// 왼쪽 T자 패턴 - 가로 전체 + 세로 왼쪽 가장자리
    /// </summary>
    public static List<Vector3Int> LeftTShape => CreateLeftTShape();
    
    /// <summary>
    /// 왼쪽 T자 패턴 생성 (8x8 격자 기준)
    /// </summary>
    private static List<Vector3Int> CreateLeftTShape()
    {
        List<Vector3Int> pattern = new List<Vector3Int>();
        
        // 가로 라인 (중심 Y=0 기준, 실제로는 플레이어 위치에서 계산됨)
        for (int x = 0; x < 8; x++)
        {
            pattern.Add(new Vector3Int(x, 0, 0));
        }
        
        // 세로 라인 (왼쪽 가장자리 X=0, 중심 제외)
        for (int y = 1; y < 8; y++)
        {
            pattern.Add(new Vector3Int(0, y, 0));
        }
        for (int y = -1; y >= -7; y--)
        {
            pattern.Add(new Vector3Int(0, y, 0));
        }
        
        return pattern;
    }
    
    /// <summary>
    /// 원형 패턴 (반지름 1)
    /// </summary>
    public static List<Vector3Int> Circle => new List<Vector3Int>
    {
        new Vector3Int(0, 0, 0),   // 중심
        new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0),
        new Vector3Int(-1, 0, 0),                             new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 1, 0),  new Vector3Int(0, 1, 0),  new Vector3Int(1, 1, 0)
    };
    
    /// <summary>
    /// 큰 원형 패턴 (반지름 2)
    /// </summary>
    public static List<Vector3Int> LargeCircle => new List<Vector3Int>
    {
        // 중심
        new Vector3Int(0, 0, 0),
        
        // 반지름 1
        new Vector3Int(-1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(1, -1, 0),
        new Vector3Int(-1, 0, 0),                             new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 1, 0),  new Vector3Int(0, 1, 0),  new Vector3Int(1, 1, 0),
        
        // 반지름 2
        new Vector3Int(-2, -2, 0), new Vector3Int(-1, -2, 0), new Vector3Int(0, -2, 0), 
        new Vector3Int(1, -2, 0),  new Vector3Int(2, -2, 0),
        new Vector3Int(-2, -1, 0),                                                      new Vector3Int(2, -1, 0),
        new Vector3Int(-2, 0, 0),                                                       new Vector3Int(2, 0, 0),
        new Vector3Int(-2, 1, 0),                                                       new Vector3Int(2, 1, 0),
        new Vector3Int(-2, 2, 0),  new Vector3Int(-1, 2, 0),  new Vector3Int(0, 2, 0),  
        new Vector3Int(1, 2, 0),   new Vector3Int(2, 2, 0)
    };
    
    /// <summary>
    /// 커스텀 직사각형 패턴 생성
    /// </summary>
    /// <param name="width">가로 크기</param>
    /// <param name="height">세로 크기</param>
    /// <param name="filled">내부를 채울지 여부</param>
    public static List<Vector3Int> CreateRectangle(int width, int height, bool filled = true)
    {
        List<Vector3Int> pattern = new List<Vector3Int>();
        int halfWidth = width / 2;
        int halfHeight = height / 2;
        
        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int y = -halfHeight; y <= halfHeight; y++)
            {
                if (filled || x == -halfWidth || x == halfWidth || y == -halfHeight || y == halfHeight)
                {
                    pattern.Add(new Vector3Int(x, y, 0));
                }
            }
        }
        
        return pattern;
    }
    
    /// <summary>
    /// 커스텀 원형 패턴 생성
    /// </summary>
    /// <param name="radius">반지름</param>
    /// <param name="filled">내부를 채울지 여부</param>
    public static List<Vector3Int> CreateCircle(int radius, bool filled = true)
    {
        List<Vector3Int> pattern = new List<Vector3Int>();
        
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                float distance = Mathf.Sqrt(x * x + y * y);
                
                if (filled ? distance <= radius : Mathf.Abs(distance - radius) < 0.5f)
                {
                    pattern.Add(new Vector3Int(x, y, 0));
                }
            }
        }
        
        return pattern;
    }
    
    /// <summary>
    /// 커스텀 라인 패턴 생성
    /// </summary>
    /// <param name="direction">방향 벡터</param>
    /// <param name="length">길이</param>
    public static List<Vector3Int> CreateLine(Vector2Int direction, int length)
    {
        List<Vector3Int> pattern = new List<Vector3Int>();
        int halfLength = length / 2;
        
        for (int i = -halfLength; i <= halfLength; i++)
        {
            pattern.Add(new Vector3Int(direction.x * i, direction.y * i, 0));
        }
        
        return pattern;
    }
}