using UnityEngine;
using UnityEngine.Localization;

public class SellQuest : GuideQuest
{
    [SerializeField]
    public int count = 0;

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
        GuideHandler.instance.canSell = true;
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
        GuideHandler.instance.canSell = false;
    }
}
