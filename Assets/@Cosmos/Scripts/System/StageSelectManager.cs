using System;
using UnityEngine;




/// <summary>

/// </summary>
public class StageSelectManager : Singleton<StageSelectManager>
{
    private enum StageName
    {
        Slime = 0,
        Arachne = 1,
        OrcMage = 2,
    }
    
    public StageDataSO[] stageDataList; // 스테이지 데이터 리스트
    
    [SerializeField]
    private int stageNum;
    public static StageDataSO currentStageData;

    public int StageNum => stageNum;
    protected override void Awake()
    {
        base.Awake();
        stageDataList = Resources.LoadAll<StageDataSO>("StageDataSO");
        stageNum = 0;
        EventBus.SubscribeBossDeath(TestStageNumPlus); // 이것도 버티컬 선형적 스테이지 용 
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
    }
    
    public void StageSelect()
    {
        currentStageData = stageDataList[stageNum];
        if (currentStageData == null)
        {
            Debug.LogWarning("[StageSelectManager] 선택된 스테이지 데이터가 없습니다. 스테이지를 설정해주세요.");
            return;
        }
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
