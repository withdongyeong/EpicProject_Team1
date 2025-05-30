using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 타일 생성 전담 클래스 - 인벤토리 데이터를 기반으로 게임 내 타일 생성
/// </summary>
public class TileBuilder
{
    private GameObject _attackTilePrefab;
    private GameObject _defenseTilePrefab;
    private GameObject _healTilePrefab;
    private GameObject _manaHealTilePrefab;
    private GameObject _highlightTilePrefab;

    /// <summary>
    /// TileBuilder 초기화
    /// </summary>
    /// <param name="attackTilePrefab">공격 타일 프리팹</param>
    /// <param name="defenseTilePrefab">방어 타일 프리팹</param>
    /// <param name="healTilePrefab">힐 타일 프리팹</param>
    /// <param name="manaHealTilePrefab">마나 힐 타일 프리팹</param>
    /// <param name="highlightTilePrefab">하이라이트 타일 프리팹</param>
    public void Initialize(GameObject attackTilePrefab, GameObject defenseTilePrefab, GameObject healTilePrefab, GameObject manaHealTilePrefab, GameObject highlightTilePrefab)
    {
        _attackTilePrefab = attackTilePrefab;
        _defenseTilePrefab = defenseTilePrefab;
        _healTilePrefab = healTilePrefab;
        _manaHealTilePrefab = manaHealTilePrefab;
        _highlightTilePrefab = highlightTilePrefab;
    }
    
    /// <summary>
    /// 빌딩 씬 데이터를 기반으로 게임 내 타일 생성
    /// </summary>
    /// <param name="gridSystem">그리드 시스템</param>
    /// <param name="placedTiles">배치된 타일 데이터 목록</param>
    public void CreateTilesFromBuildingData(GridSystem gridSystem, System.Collections.Generic.List<TilePlacementData> placedTiles)
    {
        // 그리드 총 행 수 정의
        int totalRows = 8;
        
        // 선명한 색상 팔레트 정의 (20가지)
        Color[] colorPalette = GetColorPalette();
        
        // 색상 인덱스 (순차적으로 색상 할당)
        int colorIndex = 0;
        
        foreach (TilePlacementData placementData in placedTiles)
        {
            // InventoryItemData 사용
            InventoryItemData itemData = placementData.itemData;
            
            if (itemData != null)
            {
                GameObject tilePrefab = GetTilePrefabByType(itemData.TileType);
                
                if (tilePrefab != null)
                {
                    // 타일 원점(좌상단) 위치
                    int startX = placementData.x;
                    int startY = totalRows - 1 - placementData.y;
                    
                    // 타일 중심 위치 계산
                    Vector3 centerPos = gridSystem.GetWorldPosition(startX, startY);
                    
                    // 타일 생성 (회전값은 0으로 설정 - ShapeData에 이미 회전이 반영됨)
                    GameObject tileObj = Object.Instantiate(tilePrefab, centerPos, Quaternion.identity);
                    BaseTile tile = tileObj.GetComponent<BaseTile>();
                    
                    // 타일 속성 설정
                    ConfigureTileProperties(tile, itemData);
                    
                    // 팔레트에서 순차적으로 색상 선택
                    Color tileColor = colorPalette[colorIndex % colorPalette.Length];
                    colorIndex++;
                    
                    // 그리드에 타일 등록 및 하이라이트 생성
                    RegisterTileToGrid(gridSystem, tile, itemData, startX, startY, totalRows, tileColor);
                }
            }
        }
        
        Debug.Log($"빌딩 데이터에서 {placedTiles.Count}개 타일 생성 완료");
    }
    
    /// <summary>
    /// 색상 팔레트 반환
    /// </summary>
    /// <returns>색상 배열</returns>
    private Color[] GetColorPalette()
    {
        return new Color[]
        {
            new Color(1.0f, 0.0f, 0.0f, 0.7f),       // 빨강
            new Color(0.0f, 1.0f, 0.0f, 0.7f),       // 녹색
            new Color(0.0f, 0.0f, 1.0f, 0.7f),       // 파랑
            new Color(1.0f, 1.0f, 0.0f, 0.7f),       // 노랑
            new Color(1.0f, 0.0f, 1.0f, 0.7f),       // 마젠타
            new Color(0.0f, 1.0f, 1.0f, 0.7f),       // 시안
            new Color(1.0f, 0.5f, 0.0f, 0.7f),       // 주황
            new Color(0.5f, 0.0f, 1.0f, 0.7f),       // 보라
            new Color(0.0f, 0.5f, 1.0f, 0.7f),       // 하늘색
            new Color(0.5f, 1.0f, 0.0f, 0.7f),       // 라임
            new Color(1.0f, 0.0f, 0.5f, 0.7f),       // 핑크
            new Color(0.0f, 1.0f, 0.5f, 0.7f),       // 민트
            new Color(0.5f, 0.5f, 1.0f, 0.7f),       // 라벤더
            new Color(1.0f, 0.5f, 0.5f, 0.7f),       // 살구색
            new Color(0.5f, 1.0f, 0.5f, 0.7f),       // 연두
            new Color(0.7f, 0.3f, 0.0f, 0.7f),       // 갈색
            new Color(0.0f, 0.7f, 0.3f, 0.7f),       // 청록
            new Color(0.3f, 0.0f, 0.7f, 0.7f),       // 남색
            new Color(0.7f, 0.0f, 0.3f, 0.7f),       // 자주
            new Color(0.3f, 0.7f, 0.0f, 0.7f)        // 올리브
        };
    }
    
    /// <summary>
    /// 타일 타입에 따른 프리팹 반환
    /// </summary>
    /// <param name="type">타일 타입</param>
    /// <returns>해당하는 프리팹</returns>
    private GameObject GetTilePrefabByType(TileType type)
    {
        switch (type)
        {
            case TileType.Attack:
                return _attackTilePrefab;
            case TileType.Defense:
                return _defenseTilePrefab;
            case TileType.Heal:
                return _healTilePrefab;
            case TileType.ManaHeal:
                return _manaHealTilePrefab;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// 타일 속성 설정
    /// </summary>
    /// <param name="tile">설정할 타일</param>
    /// <param name="itemData">아이템 데이터</param>
    private void ConfigureTileProperties(BaseTile tile, InventoryItemData itemData)
    {
        if (tile == null) return;
        
        // 공통 타이밍 속성 설정
        tile.ChargeTime = itemData.ChargeTime;
        
        // 타일 타입별 속성 설정
        if (tile is ProjectileTile attackTile)
        {
            attackTile.Damage = itemData.Damage;
        }
        else if (tile is DefenseTile defenseTile)
        {
            defenseTile.InvincibilityDuration = itemData.InvincibilityDuration;
        }
        else if (tile is HealTile healTile)
        {
            healTile.HealAmount = itemData.HealAmount;
        }
        else if (tile is ObstacleTile obstacleTile)
        {
            obstacleTile.Duration = itemData.ObstacleDuration;
        }
    }
    
    /// <summary>
    /// 그리드에 타일 등록 및 하이라이트 생성
    /// </summary>
    /// <param name="gridSystem">그리드 시스템</param>
    /// <param name="tile">등록할 타일</param>
    /// <param name="itemData">아이템 데이터</param>
    /// <param name="startX">시작 X 좌표</param>
    /// <param name="startY">시작 Y 좌표</param>
    /// <param name="totalRows">총 행 수</param>
    /// <param name="tileColor">타일 색상</param>
    private void RegisterTileToGrid(GridSystem gridSystem, BaseTile tile, InventoryItemData itemData, 
                                   int startX, int startY, int totalRows, Color tileColor)
    {
        // ShapeData에서 실제로 차지하는 셀 정보 사용 (Y축 반전)
        bool[,] shapeData = itemData.ShapeData;
        
        for (int y = 0; y < itemData.Height; y++)
        {
            for (int x = 0; x < itemData.Width; x++)
            {
                // Y축 반전하여 배치 (아래에서 위로)
                int invertedY = itemData.Height - 1 - y;
                
                // 해당 위치에 타일이 존재하는 경우에만 처리
                if (shapeData[invertedY, x])
                {
                    int gridX = startX + x;
                    int gridY = startY + y - itemData.Height + 1; // Height만큼 아래로 이동
                    
                    // 그리드에 타일 등록
                    gridSystem.RegisterTile(tile, gridX, gridY);
                    
                    // 하이라이트 타일 추가 (실제 차지하는 셀만)
                    CreateHighlightTile(gridSystem, gridX, gridY, tileColor);
                }
            }
        }
    }
    
    /// <summary>
    /// 하이라이트 타일 생성
    /// </summary>
    /// <param name="gridSystem">그리드 시스템</param>
    /// <param name="gridX">그리드 X 좌표</param>
    /// <param name="gridY">그리드 Y 좌표</param>
    /// <param name="tileColor">타일 색상</param>
    private void CreateHighlightTile(GridSystem gridSystem, int gridX, int gridY, Color tileColor)
    {
        if (_highlightTilePrefab != null)
        {
            Vector3 worldPos = gridSystem.GetWorldPosition(gridX, gridY);
            GameObject highlight = Object.Instantiate(_highlightTilePrefab, worldPos, Quaternion.identity);
            
            // 하이라이트에 색상 적용
            SpriteRenderer renderer = highlight.GetComponentInChildren<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = tileColor;
            }
        }
    }
}