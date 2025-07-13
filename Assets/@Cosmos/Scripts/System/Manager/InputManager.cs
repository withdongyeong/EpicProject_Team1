using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.SceneManagement;

// Singleton<T>는 이미 구현되어 있다고 가정합니다.
public class InputManager : Singleton<InputManager>
{
    // 자동으로 생성된 C# 클래스 참조 (이름을 PlayerActions로 변경했다고 가정)
    private InputActions _inputActions;

    // 외부에서 구독할 이벤트
    public Action<Vector2> OnMove;
    public Action OnRotate;

    public bool playerInputEnabled;
    public bool invenInputEnabled;
    

    // base.Awake()가 싱글톤 인스턴스를 처리하므로, 여기서는 입력 시스템 초기화만 합니다.
    protected override void Awake()
    {
        base.Awake();
        _inputActions = new InputActions(); // ★ 1. 여기서 인스턴스 생성
    }

    // ★ 3. OnEnable에서 이벤트 구독 및 활성화
    private void OnEnable()
    {
        // 2. 람다 대신 이름이 있는 메서드를 연결
        _inputActions.Player.Move.performed += HandleMovePerformed;
        _inputActions.Player.Move.canceled += HandleMoveCanceled;
        _inputActions.Inven.Rotate.performed += HandleRotatePerformed;
        // "Inven" 맵의 "LeftClick" 액션에 대한 모든 이벤트를 로그로 출력
        _inputActions.Inven.LeftClick.started += ctx => Debug.Log("--- Click Started (Pointer Down) ---");
        _inputActions.Inven.LeftClick.performed += ctx => Debug.Log("--- Click Performed ---");
        _inputActions.Inven.LeftClick.canceled += ctx => Debug.Log("--- Click Canceled (Pointer Up) ---"); // ★★★
        _inputActions.Player.Enable();
        _inputActions.Inven.Enable(); // 인벤토리 입력도 활성화
    }

    private void Start()
    {
        EventBus.SubscribeSceneLoaded(EnableInput);
    }

    // ★ 3. OnDisable에서 이벤트 구독 해제 및 비활성화
    private void OnDisable()
    {
        // 2. 연결했던 메서드를 그대로 해제
        _inputActions.Player.Move.performed -= HandleMovePerformed;
        _inputActions.Player.Move.canceled -= HandleMoveCanceled;
        _inputActions.Inven.Rotate.performed -= HandleRotatePerformed;
        _inputActions.Inven.LeftClick.started -= ctx => Debug.Log("--- [Test] Click Started ---");
        _inputActions.Inven.LeftClick.performed -= ctx => Debug.Log("--- [Test] Click Performed ---");
        _inputActions.Inven.LeftClick.canceled -= ctx => Debug.Log("--- [Test] Click CANCELED ---");
        
        _inputActions.Player.Disable();
    }

    public void Update()
    {
        playerInputEnabled = _inputActions.Player.enabled;
        invenInputEnabled = _inputActions.Inven.enabled;
    }

    // ★ 2. 이벤트 처리를 위한 별도 메서드 구현
    private void HandleMovePerformed(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(context.ReadValue<Vector2>());
    }

    private void HandleMoveCanceled(InputAction.CallbackContext context)
    {
        OnMove?.Invoke(Vector2.zero);
    }
    
    private void HandleRotatePerformed(InputAction.CallbackContext context)
    {
        OnRotate?.Invoke();
    }



    private void EnableInput(Scene scene, LoadSceneMode mode)
    {
        if (SceneLoader.IsInStage())
        {
            _inputActions.Player.Enable();
            _inputActions.Inven.Disable();
        }

        if (SceneLoader.IsInBuilding())
        {
            _inputActions.Player.Disable();
            _inputActions.Inven.Enable();
        }
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(EnableInput);
    }
}