using UnityEngine;
using UnityEngine.Localization;

public class SurviveQuest : GuideQuest
{
    [SerializeField]
    private float secondsSurvived = 0;

    private LocalizedString instructionTextLocalized;
    private LocalizedString titleTextLocalized;
    private LocalizedString subTitleTextLocalized;
    private LocalizedString contentTextLocalized;
    private LocalizedString goalTextLocalized;

    public override void SetTexts()
    {
        instructionTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Instruction2");
        titleTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Title2");
        subTitleTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_SubTitle2");
        contentTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Content2");
        goalTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Goal2");

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
