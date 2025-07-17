using UnityEngine;
using UnityEngine.Localization;

public class MoveQuest : GuideQuest
{
    private LocalizedString instructionTextLocalized;
    private LocalizedString titleTextLocalized;
    private LocalizedString subTitleTextLocalized;
    private LocalizedString contentTextLocalized;
    private LocalizedString goalTextLocalized;

    private int moveCount = 0;


    public override void SetTexts()
    {
        instructionTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Instruction1");
        titleTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Title1");
        subTitleTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_SubTitle1");
        contentTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Content1");
        goalTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Goal1");


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
        if(moveCount > 5) moveCount = 5; // 최대 5회로 제한
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
