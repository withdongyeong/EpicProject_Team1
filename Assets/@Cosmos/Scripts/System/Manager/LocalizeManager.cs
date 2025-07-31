using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Linq;

public class LocalizeManager : Singleton<LocalizeManager>
{
    private string[] languageCodes = { "ko", "zh", "ja", "en", "zh-Hant", "ru"};

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        OnLanguageChanged(SaveManager.LanguageIndex);
    }

    public int LocalizedIndex
    {
        get
        {
            string currentCode = LocalizationSettings.SelectedLocale.Identifier.Code;
            return System.Array.IndexOf(languageCodes, currentCode);
        }
    }

    public void OnLanguageChanged(int index)
    {
        // 선택된 인덱스가 유효한지 확인
        if (index < 0 || index >= 6) return;

        string selectedCode = languageCodes[index];

        Locale locale = LocalizationSettings.AvailableLocales.Locales
            .FirstOrDefault(l => l.Identifier.Code == selectedCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
        SaveManager.SaveLanguageIndex(LocalizedIndex);
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
