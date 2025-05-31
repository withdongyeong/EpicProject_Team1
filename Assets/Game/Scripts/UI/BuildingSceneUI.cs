using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BuildingSceneUI : MonoBehaviour
{
    public static BuildingSceneUI Instance { get; private set; }
    
    [SerializeField] private Button startButton;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        FindStartButton();
    }
    
    private void FindStartButton()
    {
        if (startButton == null)
            startButton = GameObject.Find("Start")?.GetComponent<Button>();
            
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        else
            Debug.LogError("StartButton을 찾을 수 없습니다");
    }
    
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "BuildingScene")
            FindStartButton();
    }
    
    public void StartGame()
    {
        if (InventoryManager.Instance != null)
            InventoryManager.Instance.CollectTilesFromGrid();
        //TODO: 이거 게임플레이 테스트씬으로 들어가요 조심하세요
        //SceneManager.LoadScene("GameplayTestScene");

        //테스트용 전투씬 입니다. 추후에 수정할 예정입니다. 
        //SceneManager.LoadScene("YDH_GameplayScene_Slime");
        SceneManager.LoadScene("YDH_GameplayScene_GoblinKing");
        //SceneManager.LoadScene("YDH_GameplayScene_Treant");
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}