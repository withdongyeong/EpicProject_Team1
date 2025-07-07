using System;
using UnityEngine;

public class FightQuest : GuideQuest
{
    [SerializeField]
    public int count = 0;

    public override void Awake()
    {
        base.Awake();
        EventBus.SubscribeBossDeath(Win);
    }
    
    public override void SetTexts()
    {
        instructionText = "가이드 7\n ㄴ 타일을 밟아 적과 싸우세요\n (0/1)";
        titleText = "- 가이드 7 -";
        subTitleText = "승리하기";
        contentText = "전투 준비를 마치고\n우측하단 [전투시작]을 \n눌러 전투를 시작하세요\n\n그리고 별자리를 밟아 \n적을 공격해 승리하세요";
        goalText = "- 완료 조건 -\n" +
                   "전투 승리 (0/1)\n";
    }

    public override void OnStart()
    {
        
      
    }
    
    public override bool IsCompleted()
    {
        instructionText = $"가이드 7\n ㄴ 타일을 밟아 적과 싸우세요\n ({count}/1)";
        GuideHandler.instance.questText.text = instructionText;
        return count >= 1;
    }

    public override void OnComplete()
    {
        
    }

    private void Win()
    {
        count++;
    }
    
    protected void OnDestroy()
    {
        EventBus.UnsubscribeBossDeath(Win);
    }
}
