using UnityEngine;
using UnityEngine.Localization;

public class RotateQuest : GuideQuest
{
    [SerializeField]
    public int tilesRotated = 0;

    private LocalizedString instructionTextLocalized;
    private LocalizedString titleTextLocalized;
    private LocalizedString subTitleTextLocalized;
    private LocalizedString contentTextLocalized;
    private LocalizedString goalTextLocalized;

    public override void SetTexts()
    {
        instructionTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Instruction4");
        titleTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Title4");
        subTitleTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_SubTitle4");
        contentTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Content4");
        goalTextLocalized = new LocalizedString("EpicProject_Table", "UI_Quest_Goal4");

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
        int MaxtilesRotated = Mathf.Min(5, tilesRotated);

        // 언어 설정에 따라 번역된 문자열 가져오기
        instructionTextLocalized.StringChanged += (text) => {
            instructionText = text.Replace("{0}", MaxtilesRotated.ToString());
        };
        GuideHandler.instance.questText.text = instructionText;
        return tilesRotated >= 5 && !DragManager.Instance.IsDragging;
    }

    public override void OnComplete()
    {
        
    }
}
