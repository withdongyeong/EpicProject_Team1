using UnityEngine;
using UnityEngine.Localization;

using System;
public class StarQuest : GuideQuest
{
    [SerializeField]
    public int starActivated = 0;

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
