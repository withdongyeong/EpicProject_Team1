using System;
using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// 게임 전체 관리 클래스 - TileBuilder로 타일 생성 로직 분리
/// 싱글톤 제거, 일반 MonoBehaviour로 동작
/// </summary>
public class StageHandler : MonoBehaviour
{
    [Header("프리팹들")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject enemyPrefab;
    
    [Header("보드 프리팹")] public GameObject nightBoard;
    
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private float countdownDuration = 3f;

    [Header("몬스터 소환 위치")]
    [SerializeField] private GameObject enemySpawnPosition;

    private SpriteRenderer backgroundSpriteRenderer;
    
    // 시스템 참조
    private BaseBoss _enemy;
    private PlayerController _player;
    private PlayerHp _playerHp;
    private PlayerMana _playerMana;
    private GameStateManager _gameStateManager;

    // Public 프로퍼티로 접근 가능하게
    public GameObject PlayerPrefab => playerPrefab;
    public GameObject EnemyPrefab => enemyPrefab;
    
    public TextMeshProUGUI CountdownText => countdownText;
    public float CountdownDuration => countdownDuration;
    public GameObject EnemySpawnPosition => enemySpawnPosition;
    public PlayerController Player => _player;
    public BaseBoss Enemy => _enemy;
    public PlayerHp PlayerHp => _playerHp;
    public PlayerMana PlayerMana => _playerMana;

    /// <summary>
    /// 게임 초기화
    /// </summary>
    

    private void Start()
    {
        

        _gameStateManager = GameStateManager.Instance;
        TryFindUIComponents();
        CreateGameContent();
        // 필수 컴포넌트 검증
        if (!ValidateComponents())
        {
            Debug.LogError("[StageHandler] 필수 컴포넌트가 누락되어 게임을 시작할 수 없습니다!");
            return;
        }
        StartCoroutine(StartCountdown());
        EventBus.PublishGameStart();
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
            Debug.LogError("[StageHandler] PlayerPrefab이 할당되지 않았습니다!");
            isValid = false;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("[StageHandler] EnemyPrefab이 할당되지 않았습니다!");
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
                    Debug.Log("[StageHandler] CountdownText를 자동으로 찾았습니다.");
                }
            }
        }

        if (enemySpawnPosition == null)
        {
            GameObject spawnObj = GameObject.Find("EnemySpawnPosition");
            if (spawnObj != null)
            {
                enemySpawnPosition = spawnObj;
                Debug.Log("[StageHandler] EnemySpawnPosition을 자동으로 찾았습니다.");
            }
        }
    }
    
    /// <summary>
    /// 게임 콘텐츠 생성 (타일, 플레이어, 적)
    /// </summary>
    private void CreateGameContent()
    {
        enemyPrefab = StageSelectManager.Instance.currentStageData.enemyPrefab;
        backgroundSpriteRenderer = transform.GetChild(0).transform.GetComponent<SpriteRenderer>();
        backgroundSpriteRenderer.sprite = StageSelectManager.Instance.currentStageData.backgroundSprite;
        SoundManager.Instance.PlayBGMSound(StageSelectManager.Instance.currentStageData.bgmClip, StageSelectManager.Instance.currentStageData.bgmVolume);
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
            Vector3 spawnPosition = new Vector3(0, 0f, 0f);

            GameObject effect = Instantiate(nightBoard, spawnPosition, Quaternion.identity);


        }
        else
        {
            Debug.LogWarning("[StageHandler] GroundEffectPrefab이 할당되지 않았습니다!");
        }
    }
    
    /// <summary>
    /// 플레이어 캐릭터 생성
    /// </summary>
    private void SpawnPlayer()
    {
        Vector3 position = GridManager.Instance.GridToWorldPosition(new Vector3Int(4, 4, 0));
        GameObject playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
        _player = playerObj.GetComponent<PlayerController>();
        _playerHp = playerObj.GetComponent<PlayerHp>();
        _playerMana = playerObj.GetComponent<PlayerMana>();
        
        // Animator를 UnscaledTime 모드로 설정 (timeScale 영향 안받음)
        Animator playerAnimator = _player.GetComponent<Animator>();
        if (playerAnimator != null)
        {
            playerAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }
        
        // 플레이어 사망 이벤트 연결
        if (_playerHp != null)
        {
           EventBus.SubscribePlayerDeath(HandlePlayerDeath);;
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
            Debug.LogWarning("[StageHandler] EnemySpawnPosition이 설정되지 않아 기본 위치를 사용합니다.");
        }
        
        GameObject enemyObj = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        _enemy = enemyObj.GetComponent<BaseBoss>();
        
        // 적 사망 이벤트 연결
        if (_enemy != null)
        {
            EventBus.SubscribeBossDeath(HandleBossDeath);
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
    /// 게임 재시작 , 상점씬으로 돌아가게 해야합니다.
    /// </summary>
    public void RestartGame()
    {
        // 현재 게임 오브젝트들 정리
        if (_player != null) Destroy(_player.gameObject);
        if (_enemy != null) Destroy(_enemy.gameObject);
        
        // 게임 콘텐츠 재생성
        CreateGameContent();
        StartCoroutine(StartCountdown());
    }

    
    
    /// <summary>
    /// 이벤트 정리 해제
    /// </summary>
    private void OnDestroy()
    {
        EventBus.UnsubscribePlayerDeath(HandlePlayerDeath);
        EventBus.UnsubscribeBossDeath(HandleBossDeath);
    }
}