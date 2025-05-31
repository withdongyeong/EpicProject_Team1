using System.Collections;
using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

/// <summary>
/// 게임 전체 관리 클래스 - TileBuilder로 타일 생성 로직 분리
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("프리팹들")]
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject highlightTilePrefab;

    [Header("타일프리팹을 넣는 리스트")]
    public List<GameObject> tilePrefabList = new();
    
    [Header("UI")]
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 3f;
    
    // 시스템 참조
    private GridSystem _gridSystem;
    private BaseBoss _enemy;
    private PlayerController _player;
    private PlayerHealth _playerHealth;
    private PlayerMana _playerMana;
    private GameStateManager _gameStateManager;
    private TileBuilder _tileBuilder;
    
    /// <summary>
    /// 게임 초기화
    /// </summary>
    private void Start()
    {
        InitializeSystems();
        CreateGameContent();
        StartCoroutine(StartCountdown());
    }
    /// <summary>
    /// 시스템들 초기화
    /// </summary>
    private void InitializeSystems()
    {
        _gridSystem = GetComponent<GridSystem>();
        _gameStateManager = GameStateManager.Instance;
        
        // TileBuilder 초기화
        _tileBuilder = new TileBuilder();
        _tileBuilder.Initialize(highlightTilePrefab, tilePrefabList);
    }
    
    /// <summary>
    /// 게임 콘텐츠 생성 (타일, 플레이어, 적)
    /// </summary>
    private void CreateGameContent()
    {
        // 빌딩 씬에서 배치한 타일 생성
        if (InventoryManager.Instance.PlacedTiles.Count > 0)
        {
            _tileBuilder.CreateTilesFromBuildingData(_gridSystem, InventoryManager.Instance.PlacedTiles);
        }
        else
        {
            Debug.LogWarning("빈 인벤토리로 게임에 왔어요!! 죽으셔야해요!");
        }
        
        SpawnPlayer();
        SpawnEnemy();
    }
    
    /// <summary>
    /// 게임 시작 카운트다운
    /// </summary>
    private IEnumerator StartCountdown()
    {
        // 게임 시간은 멈추되 UI는 업데이트되도록 설정
        TimeScaleManager.Instance.StopTimeScale();

        // 카운트다운 텍스트 설정
        SetupCountdownText();

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

        TimeScaleManager.Instance.ResetTimeScale();

        // 게임 시작 상태로 설정
        _gameStateManager.StartGame();
    }
    
    /// <summary>
    /// 카운트다운 텍스트 설정
    /// </summary>
    private void SetupCountdownText()
    {
        if (countdownText == null)
        {
            GameObject textObj = GameObject.Find("CountdownText");
            if (textObj != null)
            {
                countdownText = textObj.GetComponent<TextMeshProUGUI>();
            }
        }
    }
    
    /// <summary>
    /// 플레이어 캐릭터 생성
    /// </summary>
    private void SpawnPlayer()
    {
        Vector3 position = _gridSystem.GetWorldPosition(0, 0);
        GameObject playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
        _player = playerObj.GetComponent<PlayerController>();
        _playerHealth = playerObj.GetComponent<PlayerHealth>();
        _playerMana = playerObj.GetComponent<PlayerMana>();
        
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
        Vector3 enemyPosition = new Vector3(15f, 4f, 0f); // 오른쪽 위치
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
        _gameStateManager.LoseGame();
    }
    
    /// <summary>
    /// 보스 사망 처리
    /// </summary>
    public void HandleBossDeath()
    {
        _gameStateManager.WinGame();
    }
    
    /// <summary>
    /// 이벤트 연결 해제
    /// </summary>
    private void OnDestroy()
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
}