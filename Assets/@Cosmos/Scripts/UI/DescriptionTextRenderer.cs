using System.Collections.Generic;
using UnityEngine;

public class DescriptionTextRenderer : MonoBehaviour
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
