using UnityEngine.Localization;

public class LocalizeManager : Singleton<LocalizeManager>
{
    protected override void Awake()
    {
        base.Awake();
    }



    public string Local_Quset_Instruction_Text(string key)
    {
        string ReturnText = "";
        LocalizedString QuestText = new LocalizedString("EpicProject_Table", "UI_Quest_Instruction_" + key);
        QuestText.StringChanged += (value) =>
        {
            ReturnText = value;
        };
        return ReturnText;
    }

    public string Local_Quset_Title_Text(string key)
    {
        string ReturnText = "";
        LocalizedString QuestText = new LocalizedString("EpicProject_Table", "UI_Quest_Title_" + key);
        QuestText.StringChanged += (value) =>
        {
            ReturnText = value;
        };
        return ReturnText;
    }

    public string Local_Quset_SubTitle_Text(string key)
    {
        string ReturnText = "";
        LocalizedString QuestText = new LocalizedString("EpicProject_Table", "UI_Quest_SubTitle_" + key);
        QuestText.StringChanged += (value) =>
        {
            ReturnText = value;
        };
        return ReturnText;
    }

    public string Local_Quset_Content_Text(string key)
    {
        string ReturnText = "";
        LocalizedString QuestText = new LocalizedString("EpicProject_Table", "UI_Quest_Content_" + key);
        QuestText.StringChanged += (value) =>
        {
            ReturnText = value;
        };
        return ReturnText;
    }

    public string Local_Quset_Goal_Text(string key)
    {
        string ReturnText = "";
        LocalizedString QuestText = new LocalizedString("EpicProject_Table", "UI_Goal_Instruction_" + key);
        QuestText.StringChanged += (value) =>
        {
            ReturnText = value;
        };
        return ReturnText;
    }


    /// <summary>
    /// 타일 해금 텍스트 표시
    /// </summary>
    /// <param name="StageNumber"></param>
    /// <returns></returns>
    public string Local_TileUnlockConditions(int StageNumber)
    {
        string ReturnText = "";

        LocalizedString TileUnlockConditionsText = new LocalizedString("EpicProject_Table", "UI_Text_TileUnlockConditions");

        TileUnlockConditionsText.StringChanged += (value) =>
        {
            ReturnText = value.Replace("{0}", StageNumber.ToString());
        };

        return ReturnText;
    }

    public void TileCategoryTextChange(string tileCategory, System.Action<string> callback)
    {
        LocalizedString localizedString = new LocalizedString
        {
            TableReference = "EpicProject_Table",
            TableEntryReference = "Tile_TileCategoty_" + tileCategory
        };

        localizedString.StringChanged += (value) => callback?.Invoke(value);
    }
}
