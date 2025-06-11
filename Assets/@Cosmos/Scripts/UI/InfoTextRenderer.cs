using System.Collections.Generic;
using UnityEngine;

public class InfoTextRenderer : MonoBehaviour
{
    public enum TokenType { Text, Icon }


    public struct Token
    {
        public TokenType type;
        public string content;

        public Token(TokenType type, string content)
        {
            this.type = type;
            this.content = content;
        }
    }

    private Dictionary<string,Sprite> iconMap;

    private void Awake()
    {
        LoadAllIcon();
    }

    private void LoadAllIcon()
    {
        iconMap = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Arts/UI/Icons");

        foreach (Sprite sprite in sprites)
        {
            string key = sprite.name.ToLower(); // ex: "sword", "fire"
            if (!iconMap.ContainsKey(key))
            {
                iconMap[key] = sprite;
            }
            else
            {
                Debug.LogWarning($"중복 아이콘 키: {key}");
            }
            Debug.Log($"아이콘 {iconMap.Count}개 로드 완료");
        }
    }

    public List<Token> Parse(string input)
    {
        var tokens = new List<Token>();
        var parts = input.Split(' ');

        foreach (var part in parts)
        {
            if (part.StartsWith(":") && part.EndsWith(":"))
            {
                string iconName = part.Trim(':');
                tokens.Add(new Token(TokenType.Icon, iconName));
            }
            else
            {
                tokens.Add(new Token(TokenType.Text, part));
            }
        }

        return tokens;
    }


}
