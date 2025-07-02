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
        OrcMage = 3,
        Bomber = 4,
        GuardianGolem = 5,
        Reaper = 6
    }
    
    public StageDataSO[] stageDataList; // 스테이지 데이터 리스트
    
    [SerializeField]
    private int stageNum;
    public StageDataSO currentStageData;

    public int StageNum => stageNum;
    protected override void Awake()
    {
        base.Awake();
        stageDataList = Resources.LoadAll<StageDataSO>("StageDataSO");
        stageNum = 1; // 0은 튜토리얼 스테이지 입니다.
        EventBus.SubscribeBossDeath(TestStageNumPlus);
    }

    
    public void TestStageNumPlus() // 테스트용 스테이지 번호 증가
    {
        stageNum++;
        if (stageNum >= stageDataList.Length)
            stageNum = stageDataList.Length - 1;
        Debug.Log($"현재 스테이지 번호: {stageNum}");
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
