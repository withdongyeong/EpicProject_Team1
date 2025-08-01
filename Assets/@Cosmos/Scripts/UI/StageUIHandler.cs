using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 게임 UI 관리 클래스
/// </summary>
public class StageUIHandler : MonoBehaviour
{
    [Header("결과 패널")]
    public GameObject victoryPanel;    // 승리 시 표시되는 패널
    public GameObject defeatPanel;     // 패배 시 표시되는 패널
    public GameObject titleScenePanel; // 타이틀 씬 패널

    [Header("버튼")]
    public Button victoryReturnButton; // 승리 패널의 돌아가기 버튼
    public Button endingReturnButton; // 엔딩 패널의 돌아가기 버튼
    public Button defeatReturnButton;  // 패배 패널의 돌아가기 버튼
    public Button menuButton;
    public Button endingMenuButton;// 메인 메뉴 버튼
    public Button retryButton;         // 재시도 버튼
    public Button continueButton;      // 계속하기 버튼
    public Button abandonButton;       // 포기 버튼


    private GameStateManager _gameStateManager;


    /// <summary>
    /// 초기화 및 이벤트 연결
    /// </summary>
    private void Start()
    {
        // 패널 초기 상태 설정
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
        
        // 게임 상태 매니저 참조
        _gameStateManager = GameStateManager.Instance;
        EventBus.SubscribeGameStateChanged(HandleGameStateChanged);
        
        // 버튼 이벤트 연결
        if (victoryReturnButton != null)
            victoryReturnButton.onClick.AddListener(ReturnToBuilding);
        
        if (endingReturnButton != null)
            endingReturnButton.onClick.AddListener(ReturnToBuilding);
        

        if (defeatReturnButton != null)
        {
            if(LifeManager.Instance.Life > 0)
            {
                defeatReturnButton.onClick.AddListener(ReturnToBuilding);
            }
            else
            {
                defeatReturnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(0x44 / 255f, 0x44 / 255f, 0x44 / 255f);
                defeatReturnButton.interactable = false;
            }
                
        }

        if (menuButton != null)
            menuButton.onClick.AddListener(ShowTitleScenePanel);

        if (endingMenuButton != null)
            endingMenuButton.onClick.AddListener(ReturnToMainMenu);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueButton);

        if (abandonButton != null) {
            abandonButton.onClick.AddListener(ReturnToMainMenuWithSave);
        }

    }

    /// <summary>
    /// 게임 상태 변경 처리
    /// </summary>
    private void HandleGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.Victory:
                ShowVictoryPanel();
                break;
            case GameState.Defeat:
                ShowDefeatPanel();
                break;
            case GameState.Playing:
                HideResultPanels();
                break;
        }
    }

    /// <summary>
    /// 승리 패널 표시
    /// </summary>
    private void ShowVictoryPanel()
    {
        victoryPanel.SetActive(true);
        defeatPanel.SetActive(false);
    }

    /// <summary>
    /// 패배 패널 표시
    /// </summary>
    private void ShowDefeatPanel()
    {
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(true);
    }

    /// <summary>
    /// 타이틀 씬 패널 표시
    /// </summary>
    private void ShowTitleScenePanel()
    {
        ButtonClickSound();
        if (LifeManager.Instance.Life <= 0)
        {
            // 생명이 없을 때는 타이틀 씬 패널을 표시하지 않음
            ReturnToMainMenu();
            return;
        }
        titleScenePanel.SetActive(true);
    }

    /// <summary>
    /// 결과 패널 숨기기
    /// </summary>
    private void HideResultPanels()
    {
        victoryPanel.SetActive(false);
        defeatPanel.SetActive(false);
    }

    /// <summary>
    /// 빌딩 씬으로 돌아가기
    /// </summary>
    private void ReturnToBuilding()
    {
        ButtonClickSound();
        TimeScaleManager.Instance.ResetTimeScale();
        SceneLoader.LoadBuilding();
        
        // 게임 격자 다시 상점 자리로 원위치
        GridManager.Instance.transform.position = new Vector3(0, 0, 0);

        if(GameStateManager.Instance.CurrentState == GameState.Defeat)
        {
            LifeManager.Instance.RemoveLife(1);
        }

    }

    private void OnContinueButton()
    {
        ButtonClickSound();
        titleScenePanel.SetActive(false);
    }

    private void ReturnToMainMenu()
    {
        
        //DragManager.Instance.PlacedHandler.SavePlacedTiles();
        ButtonClickSound();
        TimeScaleManager.Instance.ResetTimeScale();
        GameManager.Instance.LoadTitle();

        // 게임 격자 다시 상점 자리로 원위치
        GridManager.Instance.transform.position = new Vector3(0, 0, 0);
    }
    
    
    private void ReturnToMainMenuWithSave()
    {
        
        DragManager.Instance.PlacedHandler.SavePlacedTiles();
        ButtonClickSound();
        TimeScaleManager.Instance.ResetTimeScale();
        GameManager.Instance.LoadTitle();

        // 게임 격자 다시 상점 자리로 원위치
        GridManager.Instance.transform.position = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// 게임 재시도
    /// </summary>
    private void RetryGame()
    {
        HideResultPanels();
        _gameStateManager.RestartGame();
    }

    private void ButtonClickSound()
    {
        // 버튼 클릭 사운드 재생
        SoundManager.Instance.UISoundClip("ButtonActivate");
    }

    /// <summary>
    /// 이벤트 연결 해제
    /// </summary>
    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStateChanged(HandleGameStateChanged);
    }
}