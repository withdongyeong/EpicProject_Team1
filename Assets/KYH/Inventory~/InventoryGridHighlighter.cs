using UnityEngine;

/// <summary>
/// 인벤토리 그리드 하이라이트 관리 전담 클래스
/// </summary>
public class InventoryGridHighlighter
{
    private InventoryGrid _inventoryGrid;
    private Camera _camera;
    
    /// <summary>
    /// 하이라이터 초기화
    /// </summary>
    /// <param name="inventoryGrid">인벤토리 그리드</param>
    /// <param name="camera">카메라</param>
    public void Initialize(InventoryGrid inventoryGrid, Camera camera)
    {
        _inventoryGrid = inventoryGrid;
        _camera = camera;
    }
    
    /// <summary>
    /// 그리드 하이라이트 업데이트
    /// </summary>
    /// <param name="item">하이라이트할 아이템</param>
    public void UpdateHighlight(InventoryItem item)
    {
        if (!ValidateForHighlight(item))
        {
            ResetHighlight();
            return;
        }
        
        // 마우스 위치를 그리드 좌표로 변환
        if (TryGetMouseGridCoordinates(out int gridX, out int gridY))
        {
            ShowHighlightAt(item, gridX, gridY);
        }
        else
        {
            // 그리드 밖에 있을 경우 하이라이트 제거
            ResetHighlight();
        }
    }
    
    /// <summary>
    /// 하이라이트 제거
    /// </summary>
    public void ResetHighlight()
    {
        if (_inventoryGrid != null)
        {
            _inventoryGrid.ResetHighlights();
        }
    }
    
    /// <summary>
    /// 하이라이트 검증
    /// </summary>
    /// <param name="item">검증할 아이템</param>
    /// <returns>검증 통과 여부</returns>
    private bool ValidateForHighlight(InventoryItem item)
    {
        if (item == null || item.ItemData == null)
        {
            Debug.LogWarning("아이템 참조가 없어 하이라이트를 업데이트할 수 없습니다");
            return false;
        }
        
        if (_inventoryGrid == null)
        {
            Debug.LogWarning("그리드 참조가 없어 하이라이트를 업데이트할 수 없습니다");
            return false;
        }
        
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Debug.LogWarning("카메라 참조가 없어 하이라이트를 업데이트할 수 없습니다");
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 마우스 위치에서 그리드 좌표 가져오기
    /// </summary>
    /// <param name="gridX">그리드 X 좌표</param>
    /// <param name="gridY">그리드 Y 좌표</param>
    /// <returns>변환 성공 여부</returns>
    private bool TryGetMouseGridCoordinates(out int gridX, out int gridY)
    {
        return _inventoryGrid.TryGetGridCoordinates(Input.mousePosition, _camera, out gridX, out gridY);
    }
    
    /// <summary>
    /// 특정 위치에 하이라이트 표시
    /// </summary>
    /// <param name="item">하이라이트할 아이템</param>
    /// <param name="gridX">그리드 X 좌표</param>
    /// <param name="gridY">그리드 Y 좌표</param>
    private void ShowHighlightAt(InventoryItem item, int gridX, int gridY)
    {
        // 배치 가능 여부 확인
        bool canPlace = _inventoryGrid.CanPlaceItem(item, gridX, gridY);
        
        // 하이라이트 표시
        _inventoryGrid.HighlightCells(gridX, gridY, item.ShapeData, canPlace);
    }
}