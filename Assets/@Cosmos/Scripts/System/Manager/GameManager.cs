using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private bool isInTutorial = false; // 튜토리얼 진행 중 ?
    public bool IsInTutorial => isInTutorial;

    /// <summary>
    /// 현재 해금된 리스트입니다 0이면 아무것도 해금 안된겁니다.
    /// </summary>
    private int unlockedInt;
    /// <summary>
    /// 현재 해금된 리스트입니다 0이면 아무것도 해금 안된겁니다.
    /// </summary>
    public int UnlockedInt => unlockedInt;

    /// <summary>
    /// 해금 된 애들 저장 위치입니다.
    /// </summary>
    private string savePath;

    protected override void Awake()
    {
        base.Awake();
        EventBus.Init(); // 꼭 한 번만 호출되게
        EventBus.SubscribeSceneLoaded(OnSceneLoaded);
        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow); // 혹은 FullScreen
        savePath = Path.Combine(Application.persistentDataPath, "unlockData.json");
        CheckUnlockSaveFile();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10))
        {
            ResetUnlockSaveFile();
        }

        ShowSetting();
        
        // Alt + Enter 감지 → 비율 깨짐 방지
        if (Input.GetKeyDown(KeyCode.Return) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }


    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(OnSceneLoaded);
        SceneLoader.LoadSceneWithName("InitializeScene");
    }
    
    private void ShowSetting(){
        if(Input.GetKeyDown(KeyCode.Escape) && (SceneLoader.IsInBuilding() || SceneLoader.IsInTitle()))
        {
            SceneLoader.ToggleSetting();
        }    
    }
      

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetTutorial();
    }

    public void LoadTitle()
    {
        TimeScaleManager.Instance.ResetTimeScale();
        GridManager.Instance.ResetGridCompletely();
        GoldManager.Instance.SetCurrentGold(16);
        LifeManager.Instance.ResetLifeManager();
        StageSelectManager.Instance.ResetManager();
        CheckUnlockSaveFile();
        for(int i =0; i<5; i++)
        {
            StoreLockManager.Instance.RemoveStoreLock(i);
        }      
        SceneLoader.LoadTitle();
    }
    public void SetTutorial()
    {
        if (SceneLoader.IsInGuide())
        {
            isInTutorial = true;
        }
        else
        {
            isInTutorial = false;
        }
    }

    /// <summary>
    /// 해금된걸 적용하는 함수입니다. 만약 파일이 없거나 손상되어있으면 0으로 초기화입니다
    /// </summary>
    private void CheckUnlockSaveFile()
    {
        if(File.Exists(savePath))
        {
            string context = File.ReadAllText(savePath);
            if (!int.TryParse(context, out unlockedInt))
            {
                unlockedInt = 0;
                File.WriteAllText(savePath, unlockedInt.ToString());
            }

        }
        else
        {
            unlockedInt = 0;  
            File.WriteAllText(savePath, unlockedInt.ToString());  
        }
    }

    /// <summary>
    /// 해금 단계를 현재에서 진행시키는 함수입니다. 해금 단계가 같거나 낮은int가 들어오면 false, 높으면 true가 반환됩니다
    /// </summary>
    /// <param name="num">진행시킬 해금 단계 입니다</param>
    /// <returns>해금 단계가 바뀌면(더 높아지면) true입니다.</returns>
    public bool UpdateUnlockSaveFIle(int num)
    {
        if(num > unlockedInt)
        {
            File.WriteAllText(savePath, num.ToString());
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ResetUnlockSaveFile()
    {
        unlockedInt = 0;
        File.WriteAllText(savePath, unlockedInt.ToString());
    }



}
