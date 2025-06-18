using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class InfoTextRenderer : MonoBehaviour
{

    /// <summary>
    /// 안에 들어있는 게임 오브젝트들은 구분선과 설명 텍스트를 가지고 있습니다. FireGimmickText면 앞의 Fire가 키가 됩니다.
    /// </summary>
    Dictionary<string, GameObject> _textDict = new();

    Dictionary<TileCategory, string> _categotyDict = new Dictionary<TileCategory, string>
    {
        { TileCategory.Weapon, "무기<sprite name=\"Weapon\">" },
        { TileCategory.MagicCircle, "마법진<sprite name=\"MagicCircle\">" },
        { TileCategory.Armor, "방어구<sprite name=\"Armor\">" },
        { TileCategory.Consumable, "소모품<sprite name=\"Potion\">" },
        { TileCategory.Accessory, "장신구<sprite name=\"Trinket\">" },
        { TileCategory.Summon, "소환수<sprite name=\"Summon\">" }
    };

    Dictionary<string, string> _synergyDict = new Dictionary<string, string>
    {
        { "Totem", "토템<sprite name=\"Totem\">" },
        { "Sword", "검<sprite name=\"Sword\">" },
        { "Fire", "화염<sprite name=\"Fire\">" },
        { "Ice", "얼음<sprite name=\"Ice\">" }
    };

    private GameObject descriptionTextPrefab; // 설명 텍스트
    private GameObject synergyTextPrefab; // 시너지 텍스트

    private void Awake()
    {
        LoadToDict();
        descriptionTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/DescriptionText");
        synergyTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/SynergyText");
    }

    /// <summary>
    /// 여기에 타일 데이터.디스크립션 넣어주면 여기서 정보 가공해서 후처리 합니다
    /// </summary>
    /// <param name="tileInfo">tiledata.description 넣어주시면 됩니다</param>
    public void InstantiateDescriptionText(TileObject tileInfo)
    {
        //태그 프리팹 추가
        List<string> tags = Parse(tileInfo.GetTileData().Description);
        if(tags.Count > 0)
        {
            TextUIResizer synergyText = Instantiate(synergyTextPrefab, transform).GetComponent<TextUIResizer>();
            string synergy = "";
            foreach (string tag in tags)
            {
                if(_synergyDict.ContainsKey(tag))
                {
                    synergy = synergy + _synergyDict[tag] + " ";
                }
                
            }
            synergy += _categotyDict[tileInfo.GetTileData().TileCategory] + " "; // 카테고리 태그 추가
            synergyText.SetText(synergy);
        }

        //설명 텍스트 추가
        TextUIResizer descriptionText = Instantiate(descriptionTextPrefab, transform).GetComponent<TextUIResizer>();
        descriptionText.SetText(tileInfo.GetTileData().Description);

        //태그 설명 추가
        foreach(string tag in tags)
        {
            if(_textDict.TryGetValue(tag,out GameObject prefab))
            {
                //TextUIResizer textUIResizer = Instantiate(prefab, transform).GetComponentInChildren<TextUIResizer>();
                //textUIResizer.SetText();
                Instantiate(prefab, transform);
            }

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

    /// <summary>
    /// GimmickText들을 load해서 dictonary에 넣는 메서드입니다
    /// </summary>
    private void LoadToDict()
    {
        GameObject[] prefabs = Resources.LoadAll<GameObject>("Prefabs/UI/InfoUI/GimmickText");
        foreach(GameObject prefab in prefabs)
        {
            if(prefab.name.EndsWith("GimmickText"))
            {
                string tag = prefab.name.Substring(0, prefab.name.Length - "GimmickText".Length);
                if(!_textDict.ContainsKey(tag))
                {
                    _textDict.Add(tag, prefab);
                }
            }
            else
            {
                Debug.LogError(prefab.name + "이자식 끝이 GimmickText가 아닌데예");
            }
        }
    }

    //TODO: 나중에 다중 언어를 지원할때 여기서 시너지(#토템 이런것들)번역을 담은 스크립터블 오브젝트를 불러오면 됩니다
}
