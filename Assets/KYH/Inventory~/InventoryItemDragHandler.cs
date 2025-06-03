using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 인벤토리 아이템 드래그 이벤트 처리 전담 클래스 - 리팩토링됨
/// </summary>
public class InventoryItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 컴포넌트 참조
    private InventoryItem _itemReference;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Canvas _canvas;
    private Camera _camera;
    
    // 헬퍼 클래스들
    private InventoryItemPlacer _itemPlacer;
    private InventoryGridHighlighter _gridHighlighter;
    
    // 드래그 상태
    private Vector2 _originalPosition;
    private Transform _originalParent;
    private bool _isDragging = false;
    private bool _isInitialized = false;
    
    /// <summary>
    /// 드래그 중인지 여부를 나타내는 프로퍼티
    /// </summary>
    public bool IsDragging 
    { 
        get => _isDragging; 
        private set => _isDragging = value; 
    }

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        InitializeComponents();
        InitializeHelpers();
    }
    
    /// <summary>
    /// 컴포넌트 참조 초기화
    /// </summary>
    private void InitializeComponents()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        _canvas = GetComponentInParent<Canvas>();
        if (_canvas == null)
        {
            _canvas = FindFirstObjectByType<Canvas>();
        }

        _camera = _canvas?.worldCamera;
        if (_camera == null)
        {
            _camera = Camera.main;
        }
    }
    
    /// <summary>
    /// 헬퍼 클래스들 초기화
    /// </summary>
    private void InitializeHelpers()
    {
        InventoryGrid inventoryGrid = FindAnyObjectByType<InventoryGrid>();
        
        _itemPlacer = new InventoryItemPlacer();
        _itemPlacer.Initialize(inventoryGrid, _camera);
        
        _gridHighlighter = new InventoryGridHighlighter();
        _gridHighlighter.Initialize(inventoryGrid, _camera);
    }
    
    /// <summary>
    /// Start에서 아이템 참조 설정
    /// </summary>
    private void Start()
    {
        if (_itemReference == null)
        {
            _itemReference = GetComponent<InventoryItem>();
            if (_itemReference != null && _itemReference.ItemData != null)
            {
                _isInitialized = true;
            }
        }
    }
    
    /// <summary>
    /// Update에서 회전 키 처리
    /// </summary>
    private void Update()
    {
        // 회전 키 처리 (R 키) - 드래그 중일 때만 작동
        if (Input.GetKeyDown(KeyCode.R) && _isDragging && _itemReference != null)
        {
            _itemReference.RotateItem();
            _gridHighlighter.UpdateHighlight(_itemReference);
        }
    }

    /// <summary>
    /// 핸들러 초기화
    /// </summary>
    /// <param name="item">연결할 InventoryItem 참조</param>
    public void Initialize(InventoryItem item)
    {
        _itemReference = item;
        _isInitialized = (_itemReference != null && _itemReference.ItemData != null);
        
        // 헬퍼 클래스들 재초기화
        if (_itemPlacer == null || _gridHighlighter == null)
        {
            InitializeHelpers();
        }
        
        // 카메라 참조 확인 및 업데이트
        UpdateCameraReference();
    }
    
    /// <summary>
    /// 카메라 참조 업데이트
    /// </summary>
    private void UpdateCameraReference()
    {
        if (_camera == null)
        {
            _camera = _canvas?.worldCamera;
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }
    }
    
    /// <summary>
    /// 아이템 회전 후 호출되는 메서드
    /// </summary>
    public void OnItemRotated()
    {
        if (_isDragging)
        {
            _gridHighlighter.UpdateHighlight(_itemReference);
        }
    }

    /// <summary>
    /// 드래그 시작 처리
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!PrepareForDrag()) return;
        
        StartDrag();
        NotifyVisualizerDragStart();
        _gridHighlighter.UpdateHighlight(_itemReference);
    }
    
    /// <summary>
    /// 드래그 준비
    /// </summary>
    /// <returns>드래그 준비 성공 여부</returns>
    private bool PrepareForDrag()
    {
        // 초기화 상태 확인
        if (!_isInitialized && _itemReference == null)
        {
            _itemReference = GetComponent<InventoryItem>();
            _isInitialized = (_itemReference != null && _itemReference.ItemData != null);
        }
        
        if (_itemReference == null || _itemReference.ItemData == null) 
        {
            Debug.LogWarning("아이템 참조가 없어 드래그를 시작할 수 없습니다");
            return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 드래그 시작 설정
    /// </summary>
    private void StartDrag()
    {
        _originalPosition = _rectTransform.anchoredPosition;
        _originalParent = transform.parent;
        _isDragging = true;

        // 아직 그리드에 배치된 아이템이라면 그리드에서 제거
        InventoryGrid inventoryGrid = FindAnyObjectByType<InventoryGrid>();
        if (inventoryGrid != null && _itemReference.GridX != -1 && _itemReference.GridY != -1)
        {
            inventoryGrid.RemoveItem(_itemReference);
        }

        // 드래그 중 UI 설정
        _canvasGroup.alpha = 0.8f;
        _canvasGroup.blocksRaycasts = false;

        // 최상위 캔버스로 이동
        if (_canvas != null)
        {
            transform.SetParent(_canvas.transform);
        }
    }
    
    /// <summary>
    /// 시각화 컴포넌트에 드래그 시작 알림
    /// </summary>
    private void NotifyVisualizerDragStart()
    {
        GetComponent<InventoryItemVisualizer>()?.OnBeginDrag();
    }

    /// <summary>
    /// 드래그 중 처리
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // 드래그 중 상태 보장
        if (!_isDragging)
        {
            _isDragging = true;
            Debug.Log("OnDrag: 드래그 상태 복원");
        }

        // 마우스 위치로 이동
        _rectTransform.position = eventData.position;

        // 하이라이트 업데이트
        _gridHighlighter.UpdateHighlight(_itemReference);
    }

    /// <summary>
    /// 드래그 종료
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_itemReference == null || !_isInitialized) return;
        
        _isDragging = false;
        RestoreDragUI();
        NotifyVisualizerDragEnd();
        
        // 배치 시도
        bool placed = _itemPlacer.TryPlaceItem(_itemReference, transform);
        
        // 배치 실패 시 원래 위치로 복귀
        if (!placed)
        {
            RestoreOriginalPosition();
        }
        
        // 하이라이트 초기화
        _gridHighlighter.ResetHighlight();
    }
    
    /// <summary>
    /// 드래그 UI 복원
    /// </summary>
    private void RestoreDragUI()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }
    
    /// <summary>
    /// 시각화 컴포넌트에 드래그 종료 알림
    /// </summary>
    private void NotifyVisualizerDragEnd()
    {
        GetComponent<InventoryItemVisualizer>()?.OnEndDrag();
    }
    
    /// <summary>
    /// 원래 위치로 복귀
    /// </summary>
    private void RestoreOriginalPosition()
    {
        transform.SetParent(_originalParent);
        _rectTransform.anchoredPosition = _originalPosition;

        // 아이템이 원래 그리드에 있었다면 다시 배치
        InventoryGrid inventoryGrid = FindAnyObjectByType<InventoryGrid>();
        if (inventoryGrid != null && _itemReference.GridX != -1 && _itemReference.GridY != -1)
        {
            inventoryGrid.PlaceItem(_itemReference, _itemReference.GridX, _itemReference.GridY);
        }
    }
}