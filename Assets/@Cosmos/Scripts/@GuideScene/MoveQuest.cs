using UnityEngine;
using UnityEngine.Localization;

public class MoveQuest : GuideQuest
{
    public LocalizedString instructionTextLocalized;
    public LocalizedString titleTextLocalized;
    public LocalizedString subTitleTextLocalized;
    public LocalizedString contentTextLocalized;
    public LocalizedString goalTextLocalized;

    public LocalizedString Test;

    private int moveCount = 0;


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

    public override bool IsCompleted()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) moveCount++;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) moveCount++;
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) moveCount++;
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) moveCount++;

        instructionTextLocalized.StringChanged += (text) =>
        {
            instructionText = text.Replace("{0}", moveCount.ToString());
        };

        GuideHandler.instance.questText.text = instructionText;
        return moveCount >= 5;
    }

    public override void OnComplete()
    {
        SteamAchievement.Achieve("TEST_ACHV_01");
    }
}
