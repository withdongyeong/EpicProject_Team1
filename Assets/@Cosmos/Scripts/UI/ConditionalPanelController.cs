using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 타이틀로 돌아가는 버튼에서 조건에 따라 다른 동작을 수행하는 컨트롤러
/// 조건 0: StageNum == 0이면 바로 타이틀로 이동
/// 조건 1: 빌딩씬이면 패널1 활성화
/// 조건 2: 스테이지씬이면 패널2 활성화
/// </summary>
public class ConditionalPanelController : MonoBehaviour
{
    [Header("패널 설정")]
    [SerializeField] private GameObject buildingScenePanel; // 빌딩씬일 때 활성화할 패널
    [SerializeField] private GameObject stageScenePanel;    // 스테이지씬일 때 활성화할 패널
    
    private Button targetButton;
    
    private void Awake()
    {
        // 버튼 컴포넌트 가져오기
        targetButton = GetComponent<Button>();
        
        if (targetButton == null)
        {
            Debug.LogError("[ConditionalPanelController] Button 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        
        // 버튼 클릭 이벤트 등록
        targetButton.onClick.AddListener(OnButtonClick);
        
        // 초기 상태에서 모든 패널 비활성화
        InitializePanels();
    }
    
    /// <summary>
    /// 패널 초기화 - 모든 패널을 비활성화
    /// </summary>
    private void InitializePanels()
    {
        if (buildingScenePanel != null)
        {
            buildingScenePanel.SetActive(false);
        }
        
        if (stageScenePanel != null)
        {
            stageScenePanel.SetActive(false);
        }
    }
    
    /// <summary>
    /// 버튼 클릭 시 호출되는 메서드
    /// 조건에 따라 바로 타이틀 이동 또는 적절한 패널 활성화
    /// </summary>
    private void OnButtonClick()
    {
        // 조건 0: StageNum이 0이면 바로 타이틀로 이동
        if (StageSelectManager.Instance != null && StageSelectManager.Instance.StageNum == 0)
        {
            Debug.Log("[ConditionalPanelController] StageNum이 0이므로 바로 타이틀로 이동");
            ExecuteDirectTitleReturn();
            return;
        }
        
        // 조건 1, 2: 빌딩씬 또는 스테이지씬에 따라 패널 활성화
        if (SceneLoader.IsInBuilding() || SceneLoader.IsInStage())
        {
            // 모든 패널 비활성화 후 조건에 맞는 패널만 활성화
            InitializePanels();
            
            if (SceneLoader.IsInBuilding())
            {
                // 빌딩씬인 경우
                ActivatePanel(buildingScenePanel, "빌딩씬 패널");
            }
            else if (SceneLoader.IsInStage())
            {
                // 스테이지씬인 경우
                ActivatePanel(stageScenePanel, "스테이지씬 패널");
            }
        }
        else
        {
            Debug.LogWarning("[ConditionalPanelController] 빌딩씬도 스테이지씬도 아닙니다.");
        }
    }
    
    /// <summary>
    /// 바로 타이틀로 돌아가는 기능 실행
    /// </summary>
    private void ExecuteDirectTitleReturn()
    {
        ButtonClickSound();
        SceneLoader.ToggleSetting();
        GameManager.Instance.LoadTitle();
    }
    
    /// <summary>
    /// 버튼 클릭 사운드 재생
    /// </summary>
    private void ButtonClickSound()
    {
        // SoundManager를 통해 버튼 클릭 사운드 재생
        if (SoundManager.Instance != null)
        {
            // 실제 사운드 메서드명에 맞게 수정 필요
            // 예: SoundManager.Instance.PlayButtonClickSound();
            Debug.Log("[ConditionalPanelController] 버튼 클릭 사운드 재생");
        }
    }
    
    /// <summary>
    /// 지정된 패널을 활성화
    /// </summary>
    /// <param name="panel">활성화할 패널</param>
    /// <param name="panelName">패널 이름 (로그용)</param>
    private void ActivatePanel(GameObject panel, string panelName)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            Debug.Log($"[ConditionalPanelController] {panelName} 활성화됨");
        }
        else
        {
            Debug.LogWarning($"[ConditionalPanelController] {panelName}이 설정되지 않았습니다.");
        }
    }
    
    /// <summary>
    /// 컴포넌트 정리
    /// </summary>
    private void OnDestroy()
    {
        if (targetButton != null)
        {
            targetButton.onClick.RemoveListener(OnButtonClick);
        }
    }
}