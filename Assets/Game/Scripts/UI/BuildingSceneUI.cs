using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 빌딩 씬 UI 관리 클래스 - ShopManager에 Start 버튼 관리 위임
/// </summary>
public class BuildingSceneUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    
    /// <summary>
    /// 초기화
    /// </summary>
    private void Start()
    {
        InitializeUI();
    }
    
    /// <summary>
    /// UI 초기화 및 ShopManager에 버튼 등록
    /// </summary>
    private void InitializeUI()
    {
        // Start 버튼 찾기
        if (startButton == null)
        {
            FindStartButton();
        }
        
        // ShopManager에 Start 버튼 등록
        if (startButton != null && ShopManager.Instance != null)
        {
            ShopManager.Instance.SetStartButton(startButton);
            Debug.Log("[BuildingSceneUI] Start 버튼을 ShopManager에 등록했습니다.");
        }
        else if (startButton == null)
        {
            Debug.LogWarning("[BuildingSceneUI] Start 버튼을 찾을 수 없습니다.");
        }
    }
    
    /// <summary>
    /// Start 버튼 찾기
    /// </summary>
    private void FindStartButton()
    {
        // 인스펙터에서 할당되지 않은 경우 자동으로 찾기
        if (startButton == null)
        {
            // 여러 가능한 이름으로 찾기
            string[] buttonNames = { "Start", "StartButton", "start", "GameStart" };
            
            foreach (string buttonName in buttonNames)
            {
                GameObject buttonObj = GameObject.Find(buttonName);
                if (buttonObj != null)
                {
                    startButton = buttonObj.GetComponent<Button>();
                    if (startButton != null)
                    {
                        Debug.Log($"[BuildingSceneUI] Start 버튼을 찾았습니다: {buttonName}");
                        break;
                    }
                }
            }
            
            // 태그로 찾기 시도
            if (startButton == null)
            {
                GameObject buttonObj = GameObject.FindWithTag("StartButton");
                if (buttonObj != null)
                {
                    startButton = buttonObj.GetComponent<Button>();
                    if (startButton != null)
                    {
                        Debug.Log("[BuildingSceneUI] Start 버튼을 태그로 찾았습니다.");
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 외부에서 Start 버튼 수동 설정
    /// </summary>
    /// <param name="button">설정할 버튼</param>
    public void SetStartButton(Button button)
    {
        startButton = button;
        
        // ShopManager에도 등록
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.SetStartButton(startButton);
        }
    }
    
    /// <summary>
    /// Start 버튼 참조 반환
    /// </summary>
    /// <returns>Start 버튼 컴포넌트</returns>
    public Button GetStartButton()
    {
        if (startButton == null)
        {
            FindStartButton();
        }
        return startButton;
    }
}