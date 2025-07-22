using UnityEngine;
using UnityEngine.UI;

public class StageSceneButtonUIHandler : MonoBehaviour
{
    private Button settingButton;
    
    void Start()
    {
        // 설정 버튼 참조 획득
        settingButton = transform.GetChild(0)?.GetComponent<Button>();
        
        if (settingButton != null)
        {
            settingButton.onClick.AddListener(() => SceneLoader.ToggleSetting());
        }
        
        // 게임 상태 변경 이벤트 구독
        EventBus.SubscribeGameStateChanged(OnGameStateChanged);
        
        // 초기 버튼 상태 설정
        UpdateButtonVisibility();
    }
    
    /// <summary>
    /// 게임 상태에 따른 버튼 표시/숨김
    /// </summary>
    private void OnGameStateChanged(GameState newState)
    {
        UpdateButtonVisibility();
    }
    
    /// <summary>
    /// 현재 게임 상태에 따라 버튼 활성화/비활성화
    /// </summary>
    private void UpdateButtonVisibility()
    {
        if (settingButton == null) return;
        
        GameState currentState = GameStateManager.Instance.CurrentState;
        
        // Count 상태일 때는 버튼 숨김
        bool shouldShow = (currentState != GameState.Count);
        settingButton.gameObject.SetActive(shouldShow);
    }
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStateChanged(OnGameStateChanged);
    }
}