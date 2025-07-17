using System;
using UnityEngine;




/// <summary>

/// </summary>
public class StageSelectManager : Singleton<StageSelectManager>
{
    private enum StageName
    {
        Guide = 0, // 튜토리얼 스테이지
        Slime = 1,
        Arachne = 2,
        Bomber = 3,
        GuardianGolem = 4,
        Turtree = 5,
        OrcMage = 6,
        Reaper = 7,
        LightningKnight = 8,
        BigHand = 9,
        LastBoss = 10,
    }
    
    public StageDataSO[] stageDataList; // 스테이지 데이터 리스트
    
    [SerializeField]
    private int stageNum;
    public StageDataSO currentStageData;

    public int StageNum => stageNum;


    private Sprite[] stageUISprites;
    public Sprite[] StageUISprites => stageUISprites;
    protected override void Awake()
    {
        base.Awake();
        stageDataList = Resources.LoadAll<StageDataSO>("StageDataSO");
        stageUISprites = Resources.LoadAll<Sprite>("Arts/Stage/스테이지이동-Sheet");
        stageNum = 1; // 0은 튜토리얼 스테이지 입니다.
        EventBus.SubscribeBossDeath(TestStageNumPlus);
    }

    public void ResetManager()
    {
        stageNum = 1;
    }

    public void SetStageNum(int num) //테스트용
    {
        if (num < 0 || num >= stageDataList.Length)
        {
            Debug.LogWarning("[StageSelectManager] 잘못된 스테이지 번호입니다. 범위를 벗어났습니다.");
            return;
        }
        stageNum = num;
    }
    
    public void TestStageNumPlus() // 테스트용 스테이지 번호 증가
    {
        SaveManager.SaveUnlockLevel(StageNum);
        stageNum++;     
        if (stageNum >= stageDataList.Length)
            stageNum = stageDataList.Length - 1;
        Debug.Log($"현재 스테이지 번호: {stageNum}");       
        EventBus.PublishStageChange();
    }
    
    public void StageSet(string bossName) // 보스 이름으로 스테이지 설정
    {
        Enum.TryParse(bossName, out StageName stageName);
        stageNum = (int)stageName;
        currentStageData = stageDataList[stageNum];
        if (currentStageData == null)
        {
            Debug.LogWarning("[StageSelectManager] 선택된 스테이지 데이터가 없습니다. 스테이지를 설정해주세요.");
            return;
        }

        if (currentStageData.enemyPrefab == null)
            currentStageData.enemyPrefab = stageDataList[0].enemyPrefab;   
        if (currentStageData.backgroundSprite == null)
            currentStageData.backgroundSprite = stageDataList[0].backgroundSprite;
        if (currentStageData.bgmClip == null)
            currentStageData.bgmClip = stageDataList[0].bgmClip;
    }
    
    public void StageSelect()
    {
        currentStageData = stageDataList[stageNum];
        if (currentStageData == null)
        {
            Debug.LogWarning("[StageSelectManager] 선택된 스테이지 데이터가 없습니다. 스테이지를 설정해주세요.");
            return;
        }

        if (currentStageData.enemyPrefab == null)
            currentStageData.enemyPrefab = stageDataList[0].enemyPrefab;   
        if (currentStageData.backgroundSprite == null)
            currentStageData.backgroundSprite = stageDataList[0].backgroundSprite;
        if (currentStageData.bgmClip == null)
            currentStageData.bgmClip = stageDataList[0].bgmClip;
        
        
        SceneLoader.LoadStage();
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeBossDeath(TestStageNumPlus);
    }

}
