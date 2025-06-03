using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// 상점 관리 시스템 - UI 버튼 참조 관리 포함
/// </summary>
public class ShopManager : MonoBehaviour
{
    private static ShopManager _instance;

    [SerializeField] private int _gold = 10;
    [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private Button _startButton;
    private int _tryCount = 1;

    // 싱글톤 인스턴스
    public static ShopManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject shopManagerObj = new GameObject("ShopManager");
                _instance = shopManagerObj.AddComponent<ShopManager>();
                DontDestroyOnLoad(shopManagerObj);
            }

            return _instance;
        }
    }

    // 보유 골드 프로퍼티
    public int Gold
    {
        get => _gold;
        private set
        {
            _gold = value;
            UpdateGoldUI();
        }
    }

    // 시도 횟수 프로퍼티
    public int TryCount
    {
        get => _tryCount;
        set
        {
            _tryCount = value;
            // 시도 횟수 증가 시 골드 업데이트
            UpdateGoldBasedOnTryCount();
        }
    }

    // Start 버튼 프로퍼티
    public Button StartButton => _startButton;

    private void Awake()
    {
        // 싱글톤 처리
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Scene 로드 이벤트 등록
        SceneManager.sceneLoaded += OnSceneLoaded;

        // 초기 골드 설정
        UpdateGoldBasedOnTryCount();
    }

    private void Start()
    {
        // 초기 UI 컴포넌트 찾기
        FindUIComponents();
    }

    /// <summary>
    /// Scene 로드 시 UI 컴포넌트 다시 찾기
    /// </summary>
    /// <param name="scene">로드된 씬</param>
    /// <param name="mode">로드 모드</param>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // BuildingScene에서만 UI 컴포넌트 찾기
        if (scene.name.Contains("Building") || scene.name.Contains("building"))
        {
            FindUIComponents();
        }
    }

    /// <summary>
    /// UI 컴포넌트들 찾기 및 설정
    /// </summary>
    private void FindUIComponents()
    {
        FindMoneyText();
        FindStartButton();
    }

    /// <summary>
    /// Money 텍스트 찾기
    /// </summary>
    private void FindMoneyText()
    {
        if (_goldText == null)
        {
            GameObject moneyTextObj = GameObject.Find("Money");
            if (moneyTextObj != null)
            {
                _goldText = moneyTextObj.GetComponent<TextMeshProUGUI>();
                if (_goldText == null)
                {
                    _goldText = moneyTextObj.GetComponent<Text>()?.GetComponent<TextMeshProUGUI>();
                }
            }

            if (_goldText == null)
            {
                Debug.LogWarning("[ShopManager] Money 텍스트를 찾을 수 없습니다.");
            }
            else
            {
                Debug.Log("[ShopManager] Money 텍스트를 찾았습니다.");
                UpdateGoldUI();
            }
        }
    }

    /// <summary>
    /// Start 버튼 찾기 및 이벤트 연결
    /// </summary>
    private void FindStartButton()
    {
        if (_startButton == null)
        {
            // 여러 가능한 이름으로 Start 버튼 찾기
            string[] buttonNames = { "Start", "StartButton", "start", "GameStart" };
            
            foreach (string buttonName in buttonNames)
            {
                GameObject buttonObj = GameObject.Find(buttonName);
                if (buttonObj != null)
                {
                    _startButton = buttonObj.GetComponent<Button>();
                    if (_startButton != null)
                    {
                        Debug.Log($"[ShopManager] Start 버튼을 찾았습니다: {buttonName}");
                        break;
                    }
                }
            }
        }

        // 버튼 이벤트 연결
        if (_startButton != null)
        {
            // 기존 리스너 제거 후 새로 추가 (중복 방지)
            _startButton.onClick.RemoveAllListeners();
            _startButton.onClick.AddListener(StartGame);
            Debug.Log("[ShopManager] Start 버튼 이벤트가 연결되었습니다.");
        }
        else
        {
            Debug.LogWarning("[ShopManager] Start 버튼을 찾을 수 없습니다.");
        }
    }

    /// <summary>
    /// 게임 시작 처리
    /// </summary>
    public void StartGame()
    {
        Debug.Log("[ShopManager] 게임 시작 버튼이 클릭되었습니다.");
        
        // 인벤토리에서 타일 수집
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.CollectTilesFromGrid();
        }
        
        // 게임플레이 씬으로 이동
        SceneManager.LoadScene("GameplayTestScene");
        
        // 다른 게임플레이 씬들
        // SceneManager.LoadScene("YDH_GameplayScene_Slime");
        // SceneManager.LoadScene("YDH_GameplayScene_GoblinKing");
        // SceneManager.LoadScene("YDH_GameplayScene_Treant");
    }

    /// <summary>
    /// 외부에서 Start 버튼 참조 요청
    /// </summary>
    /// <returns>Start 버튼 컴포넌트</returns>
    public Button GetStartButton()
    {
        if (_startButton == null)
        {
            FindStartButton();
        }
        return _startButton;
    }

    /// <summary>
    /// Start 버튼 수동 설정 (외부에서 직접 할당 시)
    /// </summary>
    /// <param name="button">할당할 버튼</param>
    public void SetStartButton(Button button)
    {
        if (button != null)
        {
            _startButton = button;
            _startButton.onClick.RemoveAllListeners();
            _startButton.onClick.AddListener(StartGame);
            Debug.Log("[ShopManager] Start 버튼이 수동으로 설정되었습니다.");
        }
    }

    /// <summary>
    /// 시도 횟수에 따라 골드 업데이트
    /// </summary>
    private void UpdateGoldBasedOnTryCount()
    {
        // 10 + (시도횟수 × 1)
        SetMoney(10 + (_tryCount - 1));
    }

    /// <summary>
    /// 골드 설정
    /// </summary>
    /// <param name="amount">설정할 골드 양</param>
    public void SetMoney(int amount)
    {
        if (amount >= 0)
        {
            Gold = amount;
            Debug.Log($"[ShopManager] 골드 설정: {_gold} (시도 횟수: {_tryCount})");
        }
    }

    /// <summary>
    /// 골드 UI 업데이트
    /// </summary>
    private void UpdateGoldUI()
    {
        if (_goldText != null)
        {
            _goldText.text = $"Gold: {_gold}";
        }
    }

    /// <summary>
    /// 아이템 구매 가능 여부 확인
    /// </summary>
    /// <param name="itemData">구매할 아이템 데이터</param>
    /// <returns>구매 가능 여부</returns>
    public bool CanPurchase(InventoryItemData itemData)
    {
        return itemData != null && _gold >= itemData.Cost;
    }

    /// <summary>
    /// 아이템 구매 처리
    /// </summary>
    /// <param name="itemData">구매할 아이템 데이터</param>
    /// <returns>구매 성공 여부</returns>
    public bool Purchase(InventoryItemData itemData)
    {
        if (!CanPurchase(itemData))
        {
            Debug.Log($"[ShopManager] 구매 실패: 골드 부족 (보유: {_gold}, 필요: {itemData.Cost})");
            return false;
        }

        // 골드 차감
        Gold -= itemData.Cost;
        Debug.Log($"[ShopManager] 아이템 구매 완료: {itemData.ItemName}, 비용: {itemData.Cost}, 남은 골드: {_gold}");

        return true;
    }

    /// <summary>
    /// 골드 추가
    /// </summary>
    /// <param name="amount">추가할 골드 양</param>
    public void AddGold(int amount)
    {
        if (amount > 0)
        {
            Gold += amount;
            Debug.Log($"[ShopManager] 골드 획득: +{amount}, 총 골드: {_gold}");
        }
    }

    /// <summary>
    /// 시도 횟수 증가
    /// </summary>
    public void IncreaseTryCount()
    {
        TryCount++;
        Debug.Log($"[ShopManager] 시도 횟수 증가: {_tryCount}");
    }

    /// <summary>
    /// 정리 작업
    /// </summary>
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        // 싱글톤 인스턴스 정리
        if (_instance == this)
        {
            _instance = null;
        }
    }
}