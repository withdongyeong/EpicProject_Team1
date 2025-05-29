using UnityEngine;

/// <summary>
/// 인벤토리 아이템 배치 로직 전담 클래스
/// </summary>
public class InventoryItemPlacer
{
    private InventoryGrid _inventoryGrid;
    private Camera _camera;
    
    /// <summary>
    /// 배치 담당자 초기화
    /// </summary>
    /// <param name="inventoryGrid">인벤토리 그리드</param>
    /// <param name="camera">카메라</param>
    public void Initialize(InventoryGrid inventoryGrid, Camera camera)
    {
        _inventoryGrid = inventoryGrid;
        _camera = camera;
    }
    
    /// <summary>
    /// 그리드에 아이템 배치 시도
    /// </summary>
    /// <param name="item">배치할 아이템</param>
    /// <param name="targetTransform">배치 대상 Transform</param>
    /// <returns>배치 성공 여부</returns>
    public bool TryPlaceItem(InventoryItem item, Transform targetTransform)
    {
        if (!ValidateItemForPlacement(item))
        {
            return false;
        }
        
        // 마우스 위치를 그리드 좌표로 변환
        if (!GetGridCoordinatesFromMouse(out int gridX, out int gridY))
        {
            Debug.Log("그리드 좌표 변환 실패: 마우스가 그리드 영역 밖에 있습니다");
            return false;
        }
        
        // 배치 가능성 및 골드 확인
        if (!CanPlaceItemAt(item, gridX, gridY))
        {
            return false;
        }
        
        // 실제 배치 실행
        return ExecutePlacement(item, targetTransform, gridX, gridY);
    }
    
    /// <summary>
    /// 아이템 배치 가능성 검증
    /// </summary>
    /// <param name="item">검증할 아이템</param>
    /// <returns>검증 통과 여부</returns>
    private bool ValidateItemForPlacement(InventoryItem item)
    {
        if (_inventoryGrid == null)
        {
            _inventoryGrid = Object.FindAnyObjectByType<InventoryGrid>();
            if (_inventoryGrid == null)
            {
                Debug.LogError("인벤토리 그리드 참조를 찾을 수 없습니다");
                return false;
            }
        }
        
        if (item == null || item.ItemData == null)
        {
            Debug.LogError("아이템 데이터가 없습니다");
            return false;
        }
        
        if (_camera == null)
        {
            _camera = Camera.main;
            if (_camera == null)
            {
                Debug.LogError("카메라 참조를 찾을 수 없습니다");
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
    private bool GetGridCoordinatesFromMouse(out int gridX, out int gridY)
    {
        return _inventoryGrid.TryGetGridCoordinates(Input.mousePosition, _camera, out gridX, out gridY);
    }
    
    /// <summary>
    /// 특정 위치에 아이템 배치 가능한지 확인
    /// </summary>
    /// <param name="item">배치할 아이템</param>
    /// <param name="gridX">그리드 X 좌표</param>
    /// <param name="gridY">그리드 Y 좌표</param>
    /// <returns>배치 가능 여부</returns>
    private bool CanPlaceItemAt(InventoryItem item, int gridX, int gridY)
    {
        // 배치 가능 여부 확인
        if (!_inventoryGrid.CanPlaceItem(item, gridX, gridY))
        {
            return false;
        }
        
        // 골드 확인
        if (!ShopManager.Instance.CanPurchase(item.ItemData))
        {
            Debug.Log("골드가 부족하여 아이템을 배치할 수 없습니다.");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 실제 배치 실행
    /// </summary>
    /// <param name="item">배치할 아이템</param>
    /// <param name="targetTransform">배치 대상 Transform</param>
    /// <param name="gridX">그리드 X 좌표</param>
    /// <param name="gridY">그리드 Y 좌표</param>
    /// <returns>배치 성공 여부</returns>
    private bool ExecutePlacement(InventoryItem item, Transform targetTransform, int gridX, int gridY)
    {
        // 그리드에 배치 시도
        if (!_inventoryGrid.PlaceItem(item, gridX, gridY))
        {
            return false;
        }
        
        // 골드 차감
        if (!ShopManager.Instance.Purchase(item.ItemData))
        {
            // 골드 부족으로 구매 실패 시 배치 취소
            _inventoryGrid.RemoveItem(item);
            return false;
        }
        
        // Transform 위치 설정
        SetItemTransform(targetTransform, gridX, gridY);
        
        return true;
    }
    
    /// <summary>
    /// 아이템 Transform 설정
    /// </summary>
    /// <param name="targetTransform">설정할 Transform</param>
    /// <param name="gridX">그리드 X 좌표</param>
    /// <param name="gridY">그리드 Y 좌표</param>
    private void SetItemTransform(Transform targetTransform, int gridX, int gridY)
    {
        // 부모 설정
        targetTransform.SetParent(_inventoryGrid.GridContainer);
    
        // 위치 설정 + 오프셋 보정
        RectTransform rectTransform = targetTransform.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector2 gridPosition = _inventoryGrid.GetCellPosition(gridX, gridY);
        
            // TODO 문제여지있음
            // 하드코딩된 오프셋 적용
            Vector2 offset = new Vector2(256f, -256f);
            rectTransform.anchoredPosition = gridPosition + offset;
        }
        
        // 정렬 순서 조정 (다른 UI 요소보다 위에 표시)
        Canvas itemCanvas = targetTransform.GetComponent<Canvas>();
        if (itemCanvas == null)
        {
            itemCanvas = targetTransform.gameObject.AddComponent<Canvas>();
        }
        itemCanvas.overrideSorting = true;
        itemCanvas.sortingOrder = 10;
        
        // 캔버스 업데이트
        Canvas.ForceUpdateCanvases();
    }
}