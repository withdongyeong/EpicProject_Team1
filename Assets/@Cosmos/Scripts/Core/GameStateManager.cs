using System;
using UnityEngine;

/// <summary>
/// 게임 상태를 관리하는 매니저 클래스
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Victory,
        Defeat
    }

    private static GameStateManager instance;
    private GameState currentState;
    private TimeScaleManager timeScaleManager;
    private GameManager gameManager;

    public event Action<GameState> OnGameStateChanged;

    public static GameStateManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameStateManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject("GameStateManager");
                    instance = obj.AddComponent<GameStateManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }

    public GameState CurrentState { get => currentState; }
    public GameManager GameManager { get => gameManager; set => gameManager = value; }

    /// <summary>
    /// 싱글톤 초기화 및 기본 설정
    /// </summary>
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        timeScaleManager = TimeScaleManager.Instance;
        SetGameState(GameState.Playing);
    }

    /// <summary>
    /// GameManager 참조 설정 및 검증
    /// </summary>
    private void Start()
    {
        FindGameManager();
    }

    /// <summary>
    /// GameManager 참조 찾기
    /// </summary>
    private void FindGameManager()
    {
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
            
            if (gameManager == null)
            {
                Debug.LogWarning("[GameStateManager] GameManager를 찾을 수 없습니다!");
            }
            else
            {
                Debug.Log("[GameStateManager] GameManager 참조를 성공적으로 설정했습니다.");
            }
        }
    }

    /// <summary>
    /// 게임 상태 변경
    /// </summary>
    /// <param name="newState">새로운 게임 상태</param>
    public void SetGameState(GameState newState)
    {
        currentState = newState;
        
        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1.0f;
                break;
            case GameState.Victory:
            case GameState.Defeat:
                Time.timeScale = 0.0f;
                break;
        }
        
        OnGameStateChanged?.Invoke(newState);
        Debug.Log($"[GameStateManager] 게임 상태가 {newState}로 변경되었습니다.");
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
        SetGameState(GameState.Playing);
    }

    /// <summary>
    /// 게임 승리
    /// </summary>
    public void WinGame()
    {
        SetGameState(GameState.Victory);
        Debug.Log("[GameStateManager] 게임 승리!");
    }

    /// <summary>
    /// 게임 패배
    /// </summary>
    public void LoseGame()
    {
        SetGameState(GameState.Defeat);
        Debug.Log("[GameStateManager] 게임 패배!");
    }

    /// <summary>
    /// 게임 재시작 - GameManager의 실제 재시작 로직 호출
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("[GameStateManager] 게임 재시작 시도...");
        
        // GameManager 참조 재확인
        if (gameManager == null)
        {
            FindGameManager();
        }
        
        if (gameManager != null)
        {
            Debug.Log("[GameStateManager] GameManager.RestartGame() 호출");
            gameManager.RestartGame();
        }
        else
        {
            Debug.LogError("[GameStateManager] GameManager를 찾을 수 없어 재시작할 수 없습니다!");
            
            // GameManager가 없으면 씬 재로드로 대체
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        
        // 타임스케일 리셋
        if (timeScaleManager != null)
        {
            timeScaleManager.ResetTimeScale();
        }
        
        SetGameState(GameState.Playing);
    }

    /// <summary>
    /// 외부에서 GameManager 참조 설정 (GameManager Start에서 호출)
    /// </summary>
    /// <param name="manager">GameManager 인스턴스</param>
    public void RegisterGameManager(GameManager manager)
    {
        gameManager = manager;
        Debug.Log("[GameStateManager] GameManager가 등록되었습니다.");
    }
}