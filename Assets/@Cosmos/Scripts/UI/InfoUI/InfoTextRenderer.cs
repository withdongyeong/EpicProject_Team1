using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class InfoTextRenderer : MonoBehaviour
{

    /// <summary>
    /// 안에 들어있는 게임 오브젝트들은 구분선과 설명 텍스트를 가지고 있습니다. FireGimmickText면 앞의 Fire가 키가 됩니다.
    /// </summary>
   // Dictionary<string, GameObject> _textDict = new();

    Dictionary<TileCategory, string> _categotyDict = new Dictionary<TileCategory, string>
    {
        { TileCategory.Weapon, "<color=#D5F1FF>무기<sprite name=\"Weapon\"></color>" },
        { TileCategory.MagicCircle, "<color=#D5F1FF>마법진<sprite name=\"MagicCircle\"></color>" },
        { TileCategory.Armor, "<color=#D5F1FF>방어구<sprite name=\"Armor\"></color>" },
        { TileCategory.Consumable, "<color=#D5F1FF>소모품<sprite name=\"Potion\"></color>" },
        { TileCategory.Trinket, "<color=#D5F1FF>장신구<sprite name=\"Trinket\"></color>" },
        { TileCategory.Summon, "<color=#D5F1FF>소환수<sprite name=\"Summon\"></color>" }
    };

    Dictionary<string, string> _synergyDict = new Dictionary<string, string>
    {
        { "Totem", "<color=#ECA03E>#토템<sprite name=\"Totem\"></color>" },
        { "Sword", "<color=#D1DBE5>#검<sprite name=\"Sword\"></color>" },
        { "Fire", "<color=#DB444C>#화염<sprite name=\"Fire\"></color>" },
        { "Ice", "<color=#0F88C1>#동상<sprite name=\"Ice\"></color>" },
        { "Shield", "<color=white>#방어막<sprite name=\"Shield\"></color>" },
        { "Barrier", "<color=white>#보호막<sprite name=\"Barrier\"></color>" },
        {"Curse", "<color=#AE3FF3>#저주<sprite name=\"Curse\"></color>" },
        {"Cloud","<color=white>#구름<sprite name=\"Cloud\"></color>" }
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
    public void InstantiateDescriptionText(TileObject tileInfo)
    {
        bool hasTag = false;
        //태그 프리팹 추가
        List<string> tags = Parse(tileInfo.GetTileData().Description);
        string synergy = "";
        if (tags.Count > 0)
        {
            foreach (string tag in tags)
            {
                if(_synergyDict.ContainsKey(tag))
                {
                    synergy = synergy + _synergyDict[tag] + " ";
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
        descriptionText.SetText(tileInfo.GetTileData().Description);

        //태그 설명 추가
        foreach (string tag in tags)
        {
            if (tagDescription.ContainsKey(tag))
            {
                Instantiate(linePrefab, transform); // 구분선 추가
                TextUIResizer tagText = Instantiate(descriptionTextPrefab, transform).GetComponent<TextUIResizer>();
                tagText.SetTagText(tagDescription[tag]);
            }
        }

        //카테고리 텍스트 추가
        TextMeshProUGUI categoryText = Instantiate(categoryTextPrefab, transform).GetComponent<TextMeshProUGUI>();
        if (_categotyDict.TryGetValue(tileInfo.GetTileData().TileCategory, out string categoryTextValue))
        {
            categoryText.text = categoryTextValue;
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
            }
        }
        return result;
    }

    Dictionary<string, string> tagDescription = new Dictionary<string, string>
    {
        { "Fire", "화염<sprite name=\"Fire\">: 매초 스택만큼 피해를 주고 스택이 1 감소합니다." },
        { "Ice", "동상<sprite name=\"Ice\">: 10번 중첩되면,적을 2초동안 빙결시킵니다." },
        { "Sword", "검<sprite name=\"Sword\">: 일정시간 동안 유지되는 소환수입니다. 검에게 명령을 내리면 모든 검이 동일한 명령을 수행합니다." },
        { "Totem", "토템<sprite name=\"Totem\">: 토템이 3개 쌓이면 효과를 발동하고 사라집니다. 3번째로 올라간 토템의 효과는 강화됩니다." },
        { "Shield", "방어막<sprite name=\"Shield\">: 1회의 피해를 막아줍니다." },
        { "Barrier", "보호막<sprite name=\"Barrier\">: 일정량의 피해를 막아줍니다. 매초 감소합니다." },
        {"Pain", "고통<sprite name=\"Pain\">: 받는 피해가 10% 증가합니다." },
        {"Mark", "낙인<sprite name=\"Mark\">: 다음 공격 한 번의 피해를 50% 증가하여 받습니다." },
        {"Curse", "저주<sprite name=\"Curse\">: 저주 아이템들이 사용하는 자원입니다. 기본적인 한도는 30입니다." },
    };
}
