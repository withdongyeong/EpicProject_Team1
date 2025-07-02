using UnityEngine;

public class SurviveQuest : GuideQuest
{
    [SerializeField]
    private float secondsSurvived = 0;
    
    public override void OnStart()
    {
        instructionText = "가이드 2\n ㄴ 10초간 생존하세요\n (0/10초)";
        GuideHandler.instance.questText.text = instructionText;
    }

    public override bool IsCompleted()
    {
        
        secondsSurvived += Time.deltaTime;
        secondsSurvived = Mathf.Clamp(secondsSurvived, 0, 10); // 10초 이상은 안되게
        instructionText = $"가이드 2\n ㄴ 10초간 생존하세요\n ({(int)secondsSurvived}/10초)";
        GuideHandler.instance.questText.text = instructionText;
        return secondsSurvived >= 1;
    }

    public override void OnComplete()
    {
        TimeScaleManager.Instance.ResetTimeScale();
        SceneLoader.LoadGuideBuilding();
        // 게임 격자 다시 상점 자리로 원위치
        GridManager.Instance.transform.position = new Vector3(0, 0, 0);
    }
}
