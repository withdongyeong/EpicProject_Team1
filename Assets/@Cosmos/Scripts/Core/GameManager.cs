using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 게임 전체 관리 클래스 - TileBuilder로 타일 생성 로직 분리
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("프리팹들")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    
    [Header("타일프리팹을 넣는 리스트")]
    private List<GameObject> tilePrefabList = new();

    [Header("보드 프리팹")] 
    [SerializeField] private GameObject nightBoard;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float countdownDuration = 3f;

    [Header("몬스터 소환 위치")]
    [SerializeField] private GameObject enemySpawnPosition;
    
    // 시스템 참조
    private BaseBoss enemy;
    private PlayerController player;
    private PlayerHealth playerHealth;
    private PlayerMana playerMana;
    private GameStateManager gameStateManager;

    public GameObject PlayerPrefab { get => playerPrefab; set => playerPrefab = value; }
    public GameObject EnemyPrefab { get => enemyPrefab; set => enemyPrefab = value; }
    public GameObject NightBoard { get => nightBoard; set => nightBoard = value; }
    public TextMeshProUGUI CountdownText { get => countdownText; set => countdownText = value; }
    public float CountdownDuration { get => countdownDuration; set => countdownDuration = value; }
    public GameObject EnemySpawnPosition { get => enemySpawnPosition; set => enemySpawnPosition = value; }
    public PlayerController Player { get => player; }
    public BaseBoss Enemy { get => enemy; }
    public PlayerHealth PlayerHealth { get => playerHealth; }
    public PlayerMana PlayerMana { get => playerMana; }
    
    /// <summary>
    /// 게임 초기화
    /// </summary>
    private void Start()
    {
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
        gameStateManager = GameStateManager.Instance;
        
        // GameStateManager에 자신을 등록
        if (gameStateManager != null)
        {
            gameStateManager.RegisterGameManager(this);
        }
    }
    
    /// <summary>
    /// 게임 콘텐츠 생성 (타일, 플레이어, 적)
    /// </summary>
    private void CreateGameContent()
    {
        SpawnPlayer();
        SpawnEnemy();
    }
    
    /// <summary>
    /// 게임 시작 카운트다운
    /// </summary>
    private IEnumerator StartCountdown()
    {
        yield return null;
        
        if (TimeScaleManager.Instance != null)
        {
            TimeScaleManager.Instance.StopTimeScale();
        }
        
        if (player != null && player.Animator != null)
        {
            player.Animator.SetTrigger("Start");
        }

        float timeLeft = countdownDuration;

        while (timeLeft > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = Mathf.CeilToInt(timeLeft).ToString();
                countdownText.gameObject.SetActive(true);
            }

            yield return new WaitForSecondsRealtime(0.1f);
            timeLeft -= 0.1f;
        }

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

        if (gameStateManager != null)
        {
            gameStateManager.StartGame();
        }
    }
    
    /// <summary>
    /// 땅 이펙트 프리팹 생성
    /// </summary>
    public void SpawnGroundEffect()
    {
        if (nightBoard != null && player != null)
        {
            Vector3 spawnPosition = new Vector3(-3.5f, 0f, 0f);
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
        Vector3 position = GridManager.Instance.GridToWorldPosition(new Vector3Int(4, 4, 0));
        GameObject playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
        player = playerObj.GetComponent<PlayerController>();
        playerHealth = playerObj.GetComponent<PlayerHealth>();
        playerMana = playerObj.GetComponent<PlayerMana>();
    
        if (AttackPreviewManager.Instance != null)
        {
            AttackPreviewManager.Instance.SetPlayerController(player);
        }
    
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
    
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath += HandlePlayerDeath;
        }
        
        Debug.Log("[GameManager] 플레이어 스폰 완료!");
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
        enemy = enemyObj.GetComponent<BaseBoss>();
        
        if (enemy != null)
        {
            enemy.OnBossDeath += HandleBossDeath;
        }
        
        Debug.Log("[GameManager] 적 스폰 완료!");
    }
    
    /// <summary>
    /// 플레이어 사망 처리
    /// </summary>
    private void HandlePlayerDeath()
    {
        Debug.Log("[GameManager] 플레이어 사망 처리");
        if (gameStateManager != null)
        {
            gameStateManager.LoseGame();
        }
    }
    
    /// <summary>
    /// 보스 사망 처리
    /// </summary>
    public void HandleBossDeath()
    {
        Debug.Log("[GameManager] 보스 사망 처리");
        if (gameStateManager != null)
        {
            gameStateManager.WinGame();
        }
    }
    
    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("[GameManager] 게임 재시작 시작...");
        
        // 기존 오브젝트들 정리
        CleanupGameObjects();
        
        // 이벤트 연결 해제
        UnsubscribeEvents();
        
        // AttackPreviewManager 정리
        if (AttackPreviewManager.Instance != null)
        {
            AttackPreviewManager.Instance.ClearAllPreviews();
            AttackPreviewManager.Instance.ClearAllDamageZones();
        }
        
        // 게임 콘텐츠 재생성
        CreateGameContent();
        StartCoroutine(StartCountdown());
        
        Debug.Log("[GameManager] 게임 재시작 완료!");
    }

    /// <summary>
    /// 게임 오브젝트들 정리
    /// </summary>
    private void CleanupGameObjects()
    {
        if (player != null) 
        {
            Destroy(player.gameObject);
            player = null;
        }
        
        if (enemy != null) 
        {
            Destroy(enemy.gameObject);
            enemy = null;
        }
        
        playerHealth = null;
        playerMana = null;
    }

    /// <summary>
    /// 이벤트 연결 해제
    /// </summary>
    private void UnsubscribeEvents()
    {
        if (playerHealth != null)
        {
            playerHealth.OnPlayerDeath -= HandlePlayerDeath;
        }
        
        if (enemy != null)
        {
            enemy.OnBossDeath -= HandleBossDeath;
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