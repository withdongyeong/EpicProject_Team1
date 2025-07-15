using UnityEngine;
using UnityEngine.Localization;

public class PlaceQuest : GuideQuest
{
    [SerializeField]
    public int tilesPlaced = 0;
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
        tilesPlaced = 0;
        starActivated = 0;
    }

    public override bool IsCompleted()
    {
        // 언어 설정에 따라 번역된 문자열 가져오기
        instructionTextLocalized.Arguments = new object[] { tilesPlaced, starActivated };
        instructionTextLocalized.RefreshString();

        GuideHandler.instance.questText.text = instructionText;
        return tilesPlaced >= 3 && starActivated >= 3;
    }

    public override void OnComplete()
    {
        
    }
    
}
