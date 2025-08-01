using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class InfoTextRenderer : MonoBehaviour
{

    /// <summary>
    /// 안에 들어있는 게임 오브젝트들은 구분선과 설명 텍스트를 가지고 있습니다. FireGimmickText면 앞의 Fire가 키가 됩니다.
    /// </summary>
    // Dictionary<string, GameObject> _textDict = new();

    private Dictionary<TileCategory, LocalizedString> _categoryDict = new()
    {
        { TileCategory.Weapon, new LocalizedString("EpicProject_Table", "Tile_TileCategoty_Weapon") },
        { TileCategory.MagicCircle, new LocalizedString("EpicProject_Table", "Tile_TileCategoty_MagicCircle") },
        { TileCategory.Armor, new LocalizedString("EpicProject_Table", "Tile_TileCategoty_Armor") },
        { TileCategory.Consumable, new LocalizedString("EpicProject_Table", "Tile_TileCategoty_Consumable") },
        { TileCategory.Trinket, new LocalizedString("EpicProject_Table", "Tile_TileCategoty_Trinket") },
        { TileCategory.Summon, new LocalizedString("EpicProject_Table", "Tile_TileCategoty_Summon") },
        { TileCategory.Planet, new LocalizedString("EpicProject_Table", "Tile_TileCategoty_Planet") },
    };

    Dictionary<string, LocalizedString> _synergyDict = new Dictionary<string, LocalizedString>
    {
        { "Totem", new LocalizedString("EpicProject_Table", "Tile_TileSynergy_Totem") },
        { "Sword", new LocalizedString("EpicProject_Table", "Tile_TileSynergy_Sword") },
        { "Fire", new LocalizedString("EpicProject_Table", "Tile_TileSynergy_Fire") },
        { "Ice", new LocalizedString("EpicProject_Table", "Tile_TileSynergy_Ice") },
        { "Shield", new LocalizedString("EpicProject_Table", "Tile_TileSynergy_Shield") },
        { "Barrier", new LocalizedString("EpicProject_Table", "Tile_TileSynergy_Barrier") },
        {"Curse", new LocalizedString("EpicProject_Table", "Tile_TileSynergy_Curse") },
        {"Cloud",new LocalizedString("EpicProject_Table", "Tile_TileSynergy_Cloud") },
    };

    private GameObject descriptionTextPrefab; // 설명 텍스트
    private GameObject synergyTextPrefab; // 시너지 텍스트
    private GameObject categoryTextPrefab; // 종류 텍스트
    private GameObject linePrefab; // 구분선 프리팹

    private void Awake()
    {
        //LoadToDict();
        descriptionTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/DescriptionText");
        synergyTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/SynergyText");
        linePrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/Line");
        categoryTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/CategoryText");
    }

    /// <summary>
    /// 여기에 타일 데이터.디스크립션 넣어주면 여기서 정보 가공해서 후처리 합니다
    /// </summary>
    /// <param name="tileInfo">tiledata.description 넣어주시면 됩니다</param>
    public void InstantiateDescriptionText(TileInfo tileInfo)
    {
        bool hasTag = false;
        //태그 프리팹 추가
        List<string> tags = Parse(tileInfo.Description);
        string synergy = "";
        if (tags.Count > 0)
        {
            foreach (string tag in tags)
            {
                if(_synergyDict.ContainsKey(tag))
                {
                    _synergyDict[tag].StringChanged += (text) => synergy = synergy + text + " ";
                    hasTag = true;
                }
            }
        }
        if(hasTag)
        {
            TextUIResizer synergyText = Instantiate(synergyTextPrefab, transform).GetComponent<TextUIResizer>();
            synergyText.SetText(synergy);
        }

        
        //설명 텍스트 추가
        TextUIResizer descriptionText = Instantiate(descriptionTextPrefab, transform).GetComponent<TextUIResizer>();
        LocalizedString localized_description;
        if (tileInfo.TileName == "???")
        {
            descriptionText.SetText(LocalizeManager.Instance.Local_TileUnlockConditions(tileInfo.UnlockInt));
        }
        else
        {
            localized_description = new LocalizedString("EpicProject_Table", "Tile_TileDescription_" + tileInfo.TileName);
            localized_description.StringChanged += (text) => descriptionText.SetText(text);
        }


        //태그 설명 추가
        foreach (string tag in tags)
        {
            if (tagDescription.ContainsKey(tag))
            {
                Instantiate(linePrefab, transform); // 구분선 추가
                TextUIResizer tagText = Instantiate(descriptionTextPrefab, transform).GetComponent<TextUIResizer>();
                tagDescription[tag].StringChanged += (text) => tagText.SetText(text);
            }
        }

        //카테고리 텍스트 추가
        if (_categoryDict.TryGetValue(tileInfo.TileCategory, out LocalizedString categoryTextValue))
        {
            TextMeshProUGUI categoryText = Instantiate(categoryTextPrefab, transform).GetComponent<TextMeshProUGUI>();
            categoryTextValue.StringChanged += (text) => categoryText.text = text;
        }
    }

    /// <summary>
    /// 안에 string을 넣으면 아이콘을 불러오기 위한 구문을(sprite name ="") 싹 다 긁어와서 안의 내용물을 가져옵니다.
    /// </summary>
    /// <param name="input">무슨 아이콘을 썼는지 파싱당할 string 입니다</param>
    /// <returns></returns>  
    private List<string> Parse(string input)
    {
        List<string> result = new();
        var regex = new Regex(@"<sprite name=""(.*?)"">");

        var matches = regex.Matches(input);

        foreach (Match match in matches)
        {
            string name = match.Groups[1].Value;
            if(!result.Contains(name)) 
            {
                result.Add(name);
                //Debug.Log(name);
            }
        }
        return result;
    }

    Dictionary<string, LocalizedString> tagDescription = new Dictionary<string, LocalizedString>
    {
        { "Fire", new LocalizedString("EpicProject_Table", "Tile_TileTagDescription_Fire") },
        { "Ice", new LocalizedString("EpicProject_Table", "Tile_TileTagDescription_Ice")},
        { "Sword", new LocalizedString("EpicProject_Table", "Tile_TileTagDescription_Sword") },
        { "Totem", new LocalizedString("EpicProject_Table", "Tile_TileTagDescription_Totem") },
        { "Shield", new LocalizedString("EpicProject_Table", "Tile_TileTagDescription_Shield") },
        { "Barrier", new LocalizedString("EpicProject_Table", "Tile_TileTagDescription_Barrier") },
        {"Pain", new LocalizedString("EpicProject_Table", "Tile_TileTagDescription_Pain")},
        {"Mark", new LocalizedString("EpicProject_Table", "Tile_TileTagDescription_Mark") },
        {"Curse", new LocalizedString("EpicProject_Table", "Tile_TileTagDescription_Curse") },
        {"Nebula", new LocalizedString("EpicProject_Table", "Tile_TileTagDescription_Nebula") },
        {"Freeze", new LocalizedString("EpicProject_Table", "Tile_TileTagDescription_Freeze") }
    };
}
