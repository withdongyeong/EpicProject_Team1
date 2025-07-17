using UnityEngine;
using UnityEngine.Localization;

public class SellQuest : GuideQuest
{
    [SerializeField]
    public int count = 0;

    private LocalizedString instructionTextLocalized;
    private LocalizedString titleTextLocalized;
    private LocalizedString subTitleTextLocalized;
    private LocalizedString contentTextLocalized;
    private LocalizedString goalTextLocalized;

    public override void SetTexts()
    {
        instructionTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Instruction6");
        titleTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Title6");
        subTitleTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_SubTitle6");
        contentTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Content6");
        goalTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Goal6");

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
        
        instructionTextLocalized.StringChanged += (text) => {
            instructionText = text.Replace("{0}", count.ToString());
        };
        GuideHandler.instance.questText.text = instructionText;
        return count >= 4;
    }

    public override void OnComplete()
    {
        GuideHandler.instance.fightButton.interactable = true;
    }
}
