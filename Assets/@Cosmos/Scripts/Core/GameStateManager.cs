using System;
using UnityEngine;

/// <summary>
/// 게임 상태를 관리하는 매니저 클래스
/// </summary>
public class GameStateManager : Singleton<GameStateManager>
{
    private static GameStateManager _instance;
    private GameState _currentState;
    private TimeScaleManager _timeScaleManager;
   
    public GameState CurrentState { get => _currentState; }

    private void Start()
    {
        // 타임스케일 매니저 참조 확보
        _timeScaleManager = TimeScaleManager.Instance;
        // 기본 상태 설정
        SetGameState(GameState.Building);
    }

    /// <summary>
    /// 게임 상태 변경
    /// </summary>
    public void SetGameState(GameState newState)
    {
        _currentState = newState;
        
        // 상태에 따른 타임스케일 조절
        switch (newState)
        {
            case GameState.Playing:
                _timeScaleManager.ResetTimeScale();
                break;
            case GameState.Victory:
            case GameState.Defeat:
                _timeScaleManager.StopTimeScale();
                break;
        }
        
        // 상태 변경 이벤트 발생
        EventBus.PublishGameStateChanged(newState);
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
    }

    /// <summary>
    /// 게임 패배
    /// </summary>
    public void LoseGame()
    {
        SetGameState(GameState.Defeat);
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        _timeScaleManager.ResetTimeScale();
        SetGameState(GameState.Playing);
    }
}