using UnityEngine;
using UnityEngine.Localization;

public class SurviveQuest : GuideQuest
{
    [SerializeField]
    private float secondsSurvived = 0;

    public LocalizedString instructionTextLocalized;
    public LocalizedString titleTextLocalized;
    public LocalizedString subTitleTextLocalized;
    public LocalizedString contentTextLocalized;
    public LocalizedString goalTextLocalized;

    public override void SetTexts()
    {
        // 언어 설정에 따라 번역된 문자열 가져오기
        instructionTextLocalized.StringChanged += (text) => {
            instructionText = text.Replace("{0}", "0");
        };

        titleTextLocalized.StringChanged += (text) => titleText = text;
        subTitleTextLocalized.StringChanged += (text) => subTitleText = text;
        contentTextLocalized.StringChanged += (text) => contentText = text;
        goalTextLocalized.StringChanged += (text) => goalText = text;
    }
    
    public override void OnStart()
    {
        
    }

    public override bool IsCompleted()
    {
        
        secondsSurvived += Time.deltaTime;
        secondsSurvived = Mathf.Clamp(secondsSurvived, 0, 10); // 10초 이상은 안되게

        int Int_secondsSurvived = (int)secondsSurvived;

        instructionTextLocalized.StringChanged += (text) => {
            instructionText = text.Replace("{0}", Int_secondsSurvived.ToString());
        };

        GuideHandler.instance.questText.text = instructionText;
        return secondsSurvived >= 10;
    }

    public override void OnComplete()
    {
        TimeScaleManager.Instance.ResetTimeScale();
        
    }
}
