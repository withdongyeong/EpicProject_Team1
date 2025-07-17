using UnityEngine;
using UnityEngine.Localization;

using System;
public class StarQuest : GuideQuest
{
    [SerializeField]
    public int starActivated = 0;

    private LocalizedString instructionTextLocalized;
    private LocalizedString titleTextLocalized;
    private LocalizedString subTitleTextLocalized;
    private LocalizedString contentTextLocalized;
    private LocalizedString goalTextLocalized;

    public override void SetTexts()
    {
        instructionTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Instruction3_1");
        titleTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Title3_1");
        subTitleTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_SubTitle3_1");
        contentTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Content3_1");
        goalTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Goal3_1");


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
        int i = Math.Min(3, starActivated);

        // 언어 설정에 따라 번역된 문자열 가져오기
        instructionTextLocalized.Arguments = new object[] { i };
        instructionTextLocalized.RefreshString();

        GuideHandler.instance.questText.text = instructionText;
        return starActivated >= 3;
    }

    public override void OnComplete()
    {
        
    }
}
