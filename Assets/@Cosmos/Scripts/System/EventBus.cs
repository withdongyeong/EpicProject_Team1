using System;
using UnityEngine.SceneManagement;


//GetInvocationList().Contains(handler) 를 사용하여 중복 구독을 방지할 수 있지만, 안쓰고 있어요.
public static class EventBus
{
    private static bool _isInitialized = false;
    
    public static void Init()
    {
        if (_isInitialized) return;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        _isInitialized = true;
    }
    //GameStart 관련 이벤트
    private static Action _onGameStart; //게임 자체의 시작이 아닌 Stage 시작임... 오해금물 !
    public static void SubscribeGameStart(Action handler) => _onGameStart += handler; 
    public static void UnsubscribeGameStart(Action handler) => _onGameStart -= handler;
    public static void PublishGameStart() => _onGameStart?.Invoke();

    //SceneLoaded 관련 이벤트
    private static Action<Scene, LoadSceneMode> _onSceneLoaded;
    public static void SubscribeSceneLoaded(Action<Scene,LoadSceneMode> handler) => _onSceneLoaded += handler;
    public static void UnsubscribeSceneLoaded(Action<Scene, LoadSceneMode> handler) => _onSceneLoaded -= handler;
    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode) => _onSceneLoaded?.Invoke(scene, mode);
    
    
    //GameState 변경 관련 이벤트
    private static Action<GameState> _onGameStateChanged;
    public static void SubscribeGameStateChanged(Action<GameState> handler) => _onGameStateChanged += handler;
    public static void UnsubscribeGameStateChanged(Action<GameState> handler) => _onGameStateChanged -= handler;
    public static void PublishGameStateChanged(GameState newState) => _onGameStateChanged?.Invoke(newState);
    
    //Player Hp 관련 이벤트
    private static Action<int> _onPlayerHpChanged;
    public static void SubscribePlayerHpChanged(Action<int> handler) => _onPlayerHpChanged += handler;
    public static void UnsubscribePlayerHpChanged(Action<int> handler) => _onPlayerHpChanged -= handler;
    public static void PublishPlayerHpChanged(int hp) => _onPlayerHpChanged?.Invoke(hp);
    
    //Player Death 관련 이벤트
    private static Action _onPlayerDeath;
    public static void SubscribePlayerDeath(Action handler) => _onPlayerDeath += handler;
    public static void UnsubscribePlayerDeath(Action handler) => _onPlayerDeath -= handler;
    public static void PublishPlayerDeath() => _onPlayerDeath?.Invoke();
    
    //Boss Death 관련 이벤트
    private static Action _onBossDeath;
    public static void SubscribeBossDeath(Action handler) => _onBossDeath += handler;
    public static void UnsubscribeBossDeath(Action handler) => _onBossDeath -= handler;
    public static void PublishBossDeath() => _onBossDeath?.Invoke();

    //스테이지 번호 증가 관련 이벤트
    private static Action _onStageChange;
    public static void SubscribeStageChange(Action handler) => _onStageChange += handler;
    public static void UnsubscribeStageChange(Action handler) => _onStageChange -= handler;
    public static void PublishStageChange() => _onStageChange?.Invoke();
    
    //gold 관련 이벤트
    private static Action<int> _onGoldChanged;
    public static void SubscribeGoldChanged(Action<int> handler) => _onGoldChanged += handler;
    public static void UnsubscribeGoldChanged(Action<int> handler) => _onGoldChanged -= handler;
    public static void PublishGoldChanged(int gold) => _onGoldChanged?.Invoke(gold);
    
    
    //보호상태 변경 이벤트
    private static Action<bool> _onPlayerProtectionChanged;
    public static void SubscribePlayerProtectionChanged(Action<bool> handler) => _onPlayerProtectionChanged += handler;
    public static void UnsubscribePlayerProtectionChanged(Action<bool> handler) => _onPlayerProtectionChanged -= handler;
    public static void PublishPlayerProtectionChanged(bool isProtected) => _onPlayerProtectionChanged?.Invoke(isProtected);

    //그리드에 타일이 배치되었을 때 호출되는 이벤트
    private static Action<TileObject> _onTilePlaced;
    public static void SubscribeTilePlaced(Action<TileObject> handler) => _onTilePlaced += handler;
    public static void UnSubscribeTilePlaced(Action<TileObject> handler) => _onTilePlaced -= handler;
    public static void PublishTilePlaced(TileObject tileObject) => _onTilePlaced?.Invoke(tileObject);

    //타일이 판매되었을 때 호출되는 이벤트
    private static Action<TileObject> _onTileSell;
    public static void SubscribeTileSell(Action<TileObject> handler) => _onTileSell += handler;
    public static void UnSubscribeTileSell(Action<TileObject> handler) => _onTileSell -= handler;
    public static void PublishTileSell(TileObject tileObject) => _onTileSell?.Invoke(tileObject);

    //보호막이 적의 공격 이외로 소모되었을때 호출되는 이벤트입니다
    private static Action<int> _onProtectionConsume;
    public static void SubscribeProtectionConsume(Action<int> handler) => _onProtectionConsume += handler;
    public static void UnSubscribeProtectionConsume(Action<int> handler) => _onProtectionConsume -= handler;
    public static void PublishProtectionConsume(int num) => _onProtectionConsume?.Invoke(num);


    public static void Reset() => _isInitialized = false;

    
     

    
    
   
    
    
}