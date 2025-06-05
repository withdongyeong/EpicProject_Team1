using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 게임 전체 관리 클래스 - TileBuilder로 타일 생성 로직 분리
/// 싱글톤 제거, 일반 MonoBehaviour로 동작
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("프리팹들")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject highlightTilePrefab;
    
    [Header("타일프리팹을 넣는 리스트")]
    private List<GameObject> tilePrefabList = new();

    [Header("보드 프리팹")] public GameObject nightBoard;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float countdownDuration = 3f;

    [Header("몬스터 소환 위치")]
    [SerializeField] private GameObject enemySpawnPosition;
    
    // 시스템 참조
    private GridSystem _gridSystem;
    private BaseBoss _enemy;
    private PlayerController _player;
    private PlayerHealth _playerHealth;
    private PlayerMana _playerMana;
    private GameStateManager _gameStateManager;
    private TileBuilder _tileBuilder;

    // Public 프로퍼티로 접근 가능하게
    public GameObject PlayerPrefab => playerPrefab;
    public GameObject EnemyPrefab => enemyPrefab;
    public GameObject HighlightTilePrefab => highlightTilePrefab;
    public TextMeshProUGUI CountdownText => countdownText;
    public float CountdownDuration => countdownDuration;
    public GameObject EnemySpawnPosition => enemySpawnPosition;
    public PlayerController Player => _player;
    public BaseBoss Enemy => _enemy;
    public PlayerHealth PlayerHealth => _playerHealth;
    public PlayerMana PlayerMana => _playerMana;
    public GridSystem GridSystem => _gridSystem;
    
    /// <summary>
    /// 게임 초기화
    /// </summary>
    private void Start()
    {
        // 필수 컴포넌트 검증
        if (!ValidateComponents())
        {
            Debug.LogError("[GameManager] 필수 컴포넌트가 누락되어 게임을 시작할 수 없습니다!");
            return;
        }

        InitializeSystems();
        CreateGameContent();
        StartCoroutine(StartCountdown());
    }

    /// <summary>
    /// 필수 컴포넌트 검증 및 자동 찾기
    /// </summary>
    /// <returns>모든 필수 컴포넌트가 준비되면 true</returns>
    private bool ValidateComponents()
    {
        bool isValid = true;

        // 프리팹 검증
        if (playerPrefab == null)
        {
            Debug.LogError("[GameManager] PlayerPrefab이 할당되지 않았습니다!");
            isValid = false;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("[GameManager] EnemyPrefab이 할당되지 않았습니다!");
            isValid = false;
        }

        if (highlightTilePrefab == null)
        {
            Debug.LogError("[GameManager] HighlightTilePrefab이 할당되지 않았습니다!");
            isValid = false;
        }

        // UI 컴포넌트 자동 찾기
        TryFindUIComponents();

        return isValid;
    }

    /// <summary>
    /// UI 컴포넌트 자동 찾기
    /// </summary>
    private void TryFindUIComponents()
    {
        if (countdownText == null)
        {
            GameObject textObj = GameObject.Find("CountdownText");
            if (textObj != null)
            {
                countdownText = textObj.GetComponent<TextMeshProUGUI>();
                if (countdownText != null)
                {
                    Debug.Log("[GameManager] CountdownText를 자동으로 찾았습니다.");
                }
            }
        }

        if (enemySpawnPosition == null)
        {
            GameObject spawnObj = GameObject.Find("EnemySpawnPosition");
            if (spawnObj != null)
            {
                enemySpawnPosition = spawnObj;
                Debug.Log("[GameManager] EnemySpawnPosition을 자동으로 찾았습니다.");
            }
        }
    }
    
    /// <summary>
    /// 시스템들 초기화
    /// </summary>
    private void InitializeSystems()
    {
        _gridSystem = GetComponent<GridSystem>();
        if (_gridSystem == null)
        {
            _gridSystem = FindObjectOfType<GridSystem>();
            if (_gridSystem == null)
            {
                Debug.LogError("[GameManager] GridSystem을 찾을 수 없습니다!");
                return;
            }
        }
        
        _gameStateManager = GameStateManager.Instance;

        // TileBuilder 초기화
        if (tilePrefabList.Count == 0)
        {
            tilePrefabList.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles"));
        }
        
        _tileBuilder = new TileBuilder();
        _tileBuilder.Initialize(highlightTilePrefab, tilePrefabList);
    }
    
    /// <summary>
    /// 게임 콘텐츠 생성 (타일, 플레이어, 적)
    /// </summary>
    private void CreateGameContent()
    {
        // 빌딩 씬에서 배치한 타일 생성
        if (InventoryManager.Instance != null && InventoryManager.Instance.PlacedTiles.Count > 0)
        {
            _tileBuilder.CreateTilesFromBuildingData(_gridSystem, InventoryManager.Instance.PlacedTiles);
        }
        else
        {
            Debug.LogWarning("[GameManager] 빈 인벤토리로 게임에 왔어요!! 죽으셔야해요!");
        }
        
        SpawnPlayer();
        SpawnEnemy();
    }
    
    /// <summary>
    /// 게임 시작 카운트다운
    /// </summary>
    private IEnumerator StartCountdown()
    {
        // 플레이어 생성 대기 (한 프레임 대기)
        yield return null;
        
        // 게임 시간은 멈추되 UI는 업데이트되도록 설정
        if (TimeScaleManager.Instance != null)
        {
            TimeScaleManager.Instance.StopTimeScale();
        }
        
        // 플레이어의 스타트 애니메이션 재생 (속도 조정 포함)
        if (_player != null && _player.Animator != null)
        {
            _player.Animator.SetTrigger("Start");
        }
        
        

        // 카운트다운 시작
        float timeLeft = countdownDuration;

        while (timeLeft > 0)
        {
            // 카운트다운 텍스트 업데이트
            if (countdownText != null)
            {
                countdownText.text = Mathf.CeilToInt(timeLeft).ToString();
                countdownText.gameObject.SetActive(true);
            }

            // Time.timeScale에 영향받지 않는 WaitForSecondsRealtime 사용
            yield return new WaitForSecondsRealtime(0.1f);
            timeLeft -= 0.1f;
        }

        // 카운트다운 완료
        if (countdownText != null)
        {
            countdownText.text = "Start!";
            yield return new WaitForSecondsRealtime(0.5f);
            countdownText.gameObject.SetActive(false);
        }

        if (TimeScaleManager.Instance != null)
        {
            TimeScaleManager.Instance.ResetTimeScale();
        }

        // 게임 시작 상태로 설정
        if (_gameStateManager != null)
        {
            _gameStateManager.StartGame();
        }
    }
    
    /// <summary>
    /// 땅 이펙트 프리팹 생성
    /// </summary>
    public void SpawnGroundEffect()
    {
        if (nightBoard != null && _player != null)
        {
            //Vector3 spawnPosition = _player.transform.position;
            Vector3 spawnPosition = new Vector3(-3.5f, -0.5f, 0f);
            
            GameObject effect = Instantiate(nightBoard, spawnPosition, Quaternion.identity);
            
            Debug.Log("[GameManager] 지팡이 땅 찍기 이펙트 생성!");
        }
        else
        {
            Debug.LogWarning("[GameManager] GroundEffectPrefab이 할당되지 않았습니다!");
        }
    }
    
    /// <summary>
    /// 플레이어 캐릭터 생성
    /// </summary>
    private void SpawnPlayer()
    {
        Vector3 position = _gridSystem.GetWorldPosition(4, 4);
        GameObject playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
        _player = playerObj.GetComponent<PlayerController>();
        _playerHealth = playerObj.GetComponent<PlayerHealth>();
        _playerMana = playerObj.GetComponent<PlayerMana>();
        
        // Animator를 UnscaledTime 모드로 설정 (timeScale 영향 안받음)
        Animator playerAnimator = _player.GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
        
        // 플레이어 사망 이벤트 연결
        if (_playerHealth != null)
        {
            _playerHealth.OnPlayerDeath += HandlePlayerDeath;
        }
    }
    
    /// <summary>
    /// 적 캐릭터 생성
    /// </summary>
    private void SpawnEnemy()
    {
        Vector3 enemyPosition = Vector3.zero;
        
        if (enemySpawnPosition != null)
        {
            enemyPosition = enemySpawnPosition.transform.position;
        }
        else
        {
            enemyPosition = new Vector3(13f, 3.5f, 0f);
            Debug.LogWarning("[GameManager] EnemySpawnPosition이 설정되지 않아 기본 위치를 사용합니다.");
        }
        
        GameObject enemyObj = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        _enemy = enemyObj.GetComponent<BaseBoss>();
        
        // 적 사망 이벤트 연결
        if (_enemy != null)
        {
            _enemy.OnBossDeath += HandleBossDeath;
        }
    }
    
    /// <summary>
    /// 플레이어 사망 처리
    /// </summary>
    private void HandlePlayerDeath()
    {
        if (_gameStateManager != null)
        {
            _gameStateManager.LoseGame();
        }
    }
    
    /// <summary>
    /// 보스 사망 처리
    /// </summary>
    public void HandleBossDeath()
    {
        if (_gameStateManager != null)
        {
            _gameStateManager.WinGame();
        }
    }
    
    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        // 현재 게임 오브젝트들 정리
        if (_player != null) Destroy(_player.gameObject);
        if (_enemy != null) Destroy(_enemy.gameObject);
        
        // 이벤트 연결 해제
        UnsubscribeEvents();
        
        // 게임 콘텐츠 재생성
        CreateGameContent();
        StartCoroutine(StartCountdown());
    }

    /// <summary>
    /// 이벤트 연결 해제
    /// </summary>
    private void UnsubscribeEvents()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnPlayerDeath -= HandlePlayerDeath;
        }
        
        if (_enemy != null)
        {
            _enemy.OnBossDeath -= HandleBossDeath;
        }
    }
    
    /// <summary>
    /// 컴포넌트 정리
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}