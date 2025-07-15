using TMPro;
using UnityEngine;

public class DifficultyText : MonoBehaviour
{
    private int _difficulty = 1;
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        UpdateText();
    }

    public void SetDifficulty(int difficulty)
    {
        _difficulty = difficulty;
        UpdateText();
    }

    private void UpdateText()
    {
        switch(_difficulty)
        {
            case 0:
                _text.text = "<color=green>보스 공격 속도 30% 감소</color>";
                break;
            case 1:
                _text.text = "기준 난이도";
                break;
            case 2:
                _text.text = "<color=red>보통 난이도 클리어 시 해금\r\n" +
                    "보스 공격 속도 30% 증가\r\n" +
                    "보스 체력 30% 증가";
                break;
            case 3:
                _text.text = "<color=red>보통 난이도 클리어 시 해금\r\n" +
                    "보스 공격 속도 30% 증가\r\n" +
                    "보스 체력 50% 증가\r\n" +
                    "플레이어 시작 체력 1 (최대 체력은 100)</color>";
                break;
            default:
                _text.text = "Unknown";
                break;
        }
    }
}
