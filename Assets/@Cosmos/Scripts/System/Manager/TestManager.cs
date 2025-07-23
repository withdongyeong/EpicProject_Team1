using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class TestManager : MonoBehaviour
{
    public bool isTestMode = false; // 테스트 모드 활성화 여부
    public bool isCheatEnabled = false;
    public int stageNum = 1;
    private Canvas canvas;
    public Text stageNumText;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        canvas = GetComponentInChildren<Canvas>();
        isCheatEnabled = false; // 초기에는 치트 모드 비활성화
        canvas.enabled = false;
    }

    private void Update()
    {
        if(!isTestMode) return;
        if(Input.GetKeyDown(KeyCode.PageDown)) TestModeToggle();
        if (!isCheatEnabled) return;
        if(Input.GetKeyDown(KeyCode.F1)) GoldUp();
        if(Input.GetKeyDown(KeyCode.F2)) BossHpSet10();
        if(Input.GetKeyDown(KeyCode.F3)) BossHpSetFull();
        if(Input.GetKeyDown(KeyCode.F4)) PlayerHpSet10();
        if(Input.GetKeyDown(KeyCode.F5)) PlayerHp99999();
        if(Input.GetKeyDown(KeyCode.F6)) SetStageNum();
        if(Input.GetKeyDown(KeyCode.F7)) DeleteTutorialData();
        if(Input.GetKeyDown(KeyCode.F8)) DeleteSaveData();
        if (Input.GetKeyDown(KeyCode.F9)) AddPlayerLife();
        if (Input.GetKeyDown(KeyCode.F10)) SteamAchievementReset();


    }

    private void TestModeToggle()
    {
        isCheatEnabled = !isCheatEnabled;
        if (isCheatEnabled)
        {
            canvas.enabled = true;
            stageNum = StageSelectManager.Instance.StageNum;
            stageNumText.text = "다음에 만나는 보스번호 : " + stageNum;
        }     
        else
        {
            canvas.enabled = false;
        }
    }
    private void GoldUp()
    {
        GoldManager.Instance.ModifyCurrentGold(100);
    }

    private void BossHpSet10()
    {
        var boss = FindAnyObjectByType<BaseBoss>();
        if (boss != null)
        {
            boss.TakeDamage(boss.CurrentHealth - 10);
        }
    }
    
    private void BossHpSetFull()
    {
        var boss = FindAnyObjectByType<BaseBoss>();
        if (boss != null)
        {
            boss.TestBossHpSet(boss.MaxHealth);
        }
    }
    
    private void PlayerHpSet10()
    {
        
        var player = FindAnyObjectByType<PlayerHp>();
        if (player != null)
        {
            player.TestHpSetting(10);
            
        }
    }
    
    private void PlayerHp99999()
    {
        
        var player = FindAnyObjectByType<PlayerHp>();
        if (player != null)
        {
            player.TestHpSetting(99999);
            
        }
    }
    
    private void DeleteTutorialData()
    {
        
        PlayerPrefs.DeleteKey(SaveKeys.IsTutorialCompleted);
        PlayerPrefs.Save();
        SaveManager.LoadAll();
    }
    
    private void DeleteSaveData()
    { 
        
        SaveManager.DeleteAllSaves();
    }

    private void SetStageNum()
    {
        stageNum++;
        if (stageNum > 10) stageNum = 1; // Assuming there are 10 stages, loop back to 1
        StageSelectManager.Instance.SetStageNum(stageNum);
        stageNumText.text = "다음에 만나는 보스번호 : " + stageNum;
    }

    private void AddPlayerLife()
    {
        LifeManager.Instance.AddLife(1);
    }
    
    private void SteamAchievementReset()
    {
        SteamUserStats.ResetAllStats(true);
        SteamUserStats.StoreStats();
    }
}
