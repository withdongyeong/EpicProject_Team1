using UnityEngine;

public class SurviveQuest : GuideQuest
{
    [SerializeField]
    private float secondsSurvived = 0;
    
    
    public override void SetTexts()
    {
        instructionText = "가이드 2\n ㄴ 10초간 생존하세요\n (0/10초)";
        titleText = "- 가이드 2 -";
        subTitleText = "살아남기";
        contentText = "적은 당신을 공격합니다\n" +
                      "적의 공격을 피해 살아남으세요";
        goalText = "- 완료 조건 -\n" +
                   "10초간 생존하기 (0/10초)\n";
    }
    
    public override void OnStart()
    {
        
    }

    public override bool IsCompleted()
    {
        
        secondsSurvived += Time.deltaTime;
        secondsSurvived = Mathf.Clamp(secondsSurvived, 0, 10); // 10초 이상은 안되게
        instructionText = $"가이드 2\n ㄴ 10초간 생존하세요\n ({(int)secondsSurvived}/10초)";
        GuideHandler.instance.questText.text = instructionText;
        return secondsSurvived >= 10;
    }

    public override void OnComplete()
    {
        TimeScaleManager.Instance.ResetTimeScale();
        
    }
}
