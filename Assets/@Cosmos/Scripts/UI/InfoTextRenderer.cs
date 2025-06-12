using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class InfoTextRenderer : MonoBehaviour
{

    /// <summary>
    /// 안에 들어있는 게임 오브젝트들은 구분선과 설명 텍스트를 가지고 있습니다. FireGimmickText면 앞의 Fire가 키가 됩니다.
    /// </summary>
    Dictionary<string, GameObject> _textDict = new();

    private GameObject descriptionTextPrefab; // 설명 텍스트


    private void Awake()
    {
        LoadToDict();
        descriptionTextPrefab = Resources.Load<GameObject>("Prefabs/UI/InfoUI/DescriptionText");
    }

    /// <summary>
    /// 여기에 타일 데이터.디스크립션 넣어주면 여기서 정보 가공해서 후처리 합니다
    /// </summary>
    /// <param name="tileInfo">tiledata.description 넣어주시면 됩니다</param>
    public void InstantiateDescriptionText(string tileInfo)
    {
        List<string> tags = Parse(tileInfo);
        TextUIResizer descriptionText = Instantiate(descriptionTextPrefab, transform).GetComponent<TextUIResizer>();
        descriptionText.SetText(tileInfo);
        foreach(string tag in tags)
        {
            if(_textDict.TryGetValue(tag,out GameObject prefab))
            {
                TextUIResizer textUIResizer = Instantiate(prefab, transform).GetComponentInChildren<TextUIResizer>();
                textUIResizer.SetText();
            }

        }

    }


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
        Debug.Log(_textDict.Count + "개의 기믹텍스트 로드 완료");
    }


}
