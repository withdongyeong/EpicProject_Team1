using System.Collections;
using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

/// <summary>
/// ���� ��ü ���� Ŭ���� - TileBuilder�� Ÿ�� ���� ���� �и�
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("�����յ�")]
    public GameObject playerPrefab;
    public GameObject attackTilePrefab;
    public GameObject defenseTilePrefab;
    public GameObject healTilePrefab;
    public GameObject manaHealTilePrefab;
    public GameObject enemyPrefab;
    public GameObject highlightTilePrefab;

    [Header("Ÿ���������� �ִ� ����Ʈ")]
    public List<GameObject> tilePrefabList = new();
    
    [Header("UI")]
    public TextMeshProUGUI countdownText;
    public float countdownDuration = 3f;
    
    // �ý��� ����
    private GridSystem _gridSystem;
    private BaseBoss _enemy;
    private PlayerController _player;
    private PlayerHealth _playerHealth;
    private PlayerMana _playerMana;
    private GameStateManager _gameStateManager;
    private TileBuilder _tileBuilder;
    
    /// <summary>
    /// ���� �ʱ�ȭ
    /// </summary>
    private void Start()
    {
        InitializeSystems();
        CreateGameContent();
        StartCoroutine(StartCountdown());
    }
    /// <summary>
    /// �ý��۵� �ʱ�ȭ
    /// </summary>
    private void InitializeSystems()
    {
        _gridSystem = GetComponent<GridSystem>();
        _gameStateManager = GameStateManager.Instance;
        
        // TileBuilder �ʱ�ȭ
        _tileBuilder = new TileBuilder();
        _tileBuilder.Initialize(highlightTilePrefab, tilePrefabList);
    }
    
    /// <summary>
    /// ���� ������ ���� (Ÿ��, �÷��̾�, ��)
    /// </summary>
    private void CreateGameContent()
    {
        // ���� ������ ��ġ�� Ÿ�� ����
        if (InventoryManager.Instance.PlacedTiles.Count > 0)
        {
            _tileBuilder.CreateTilesFromBuildingData(_gridSystem, InventoryManager.Instance.PlacedTiles);
        }
        else
        {
            Debug.LogWarning("�� �κ��丮�� ���ӿ� �Ծ��!! �����ž��ؿ�!");
        }
        
        SpawnPlayer();
        SpawnEnemy();
    }
    
    /// <summary>
    /// ���� ���� ī��Ʈ�ٿ�
    /// </summary>
    private IEnumerator StartCountdown()
    {
        // ���� �ð��� ���ߵ� UI�� ������Ʈ�ǵ��� ����
        TimeScaleManager.Instance.StopTimeScale();

        // ī��Ʈ�ٿ� �ؽ�Ʈ ����
        SetupCountdownText();

        // ī��Ʈ�ٿ� ����
        float timeLeft = countdownDuration;

        while (timeLeft > 0)
        {
            // ī��Ʈ�ٿ� �ؽ�Ʈ ������Ʈ
            if (countdownText != null)
            {
                countdownText.text = Mathf.CeilToInt(timeLeft).ToString();
                countdownText.gameObject.SetActive(true);
            }

            // Time.timeScale�� ������� �ʴ� WaitForSecondsRealtime ���
            yield return new WaitForSecondsRealtime(0.1f);
            timeLeft -= 0.1f;
        }

        // ī��Ʈ�ٿ� �Ϸ�
        if (countdownText != null)
        {
            countdownText.text = "Start!";
            yield return new WaitForSecondsRealtime(0.5f);
            countdownText.gameObject.SetActive(false);
        }

        TimeScaleManager.Instance.ResetTimeScale();

        // ���� ���� ���·� ����
        _gameStateManager.StartGame();
    }
    
    /// <summary>
    /// ī��Ʈ�ٿ� �ؽ�Ʈ ����
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
    /// �÷��̾� ĳ���� ����
    /// </summary>
    private void SpawnPlayer()
    {
        Vector3 position = _gridSystem.GetWorldPosition(0, 0);
        GameObject playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
        _player = playerObj.GetComponent<PlayerController>();
        _playerHealth = playerObj.GetComponent<PlayerHealth>();
        _playerMana = playerObj.GetComponent<PlayerMana>();
        
        // �÷��̾� ��� �̺�Ʈ ����
        if (_playerHealth != null)
        {
            _playerHealth.OnPlayerDeath += HandlePlayerDeath;
        }
    }
    
    /// <summary>
    /// �� ĳ���� ����
    /// </summary>
    private void SpawnEnemy()
    {
        Vector3 enemyPosition = new Vector3(15f, 4f, 0f); // ������ ��ġ
        GameObject enemyObj = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        _enemy = enemyObj.GetComponent<BaseBoss>();
        
        // �� ��� �̺�Ʈ ����
        if (_enemy != null)
        {
            _enemy.OnBossDeath += HandleBossDeath;
        }
    }
    
    /// <summary>
    /// �÷��̾� ��� ó��
    /// </summary>
    private void HandlePlayerDeath()
    {
        _gameStateManager.LoseGame();
    }
    
    /// <summary>
    /// ���� ��� ó��
    /// </summary>
    public void HandleBossDeath()
    {
        _gameStateManager.WinGame();
    }
    
    /// <summary>
    /// �̺�Ʈ ���� ����
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