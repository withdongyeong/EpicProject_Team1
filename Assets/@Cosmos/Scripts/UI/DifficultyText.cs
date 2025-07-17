using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class DifficultyText : MonoBehaviour
{
    private int _difficulty = 1;
    private TextMeshProUGUI _text;

    private LocalizedString _easydescription;
    private LocalizedString _nomaldescription;
    private LocalizedString _harddescription;
    private LocalizedString _helldescription;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _easydescription = new LocalizedString("EpicProject_Table", "UI_Text_EasyDescription");
        _nomaldescription = new LocalizedString("EpicProject_Table", "UI_Text_NomalDescription");
        _harddescription = new LocalizedString("EpicProject_Table", "UI_Text_HardDescription");
        _helldescription = new LocalizedString("EpicProject_Table", "UI_Text_HellDescription");

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
                _easydescription.StringChanged += (text) => {
                    _text.text = text;
                };
                break;
            case 1:
                _nomaldescription.StringChanged += (text) => {
                    _text.text = text;
                };
                break;
            case 2:
                _harddescription.StringChanged += (text) => {
                    _text.text = text;
                };
                break;
            case 3:
                _helldescription.StringChanged += (text) => {
                    _text.text = text;
                };
                break;
            default:
                _text.text = "Unknown";
                break;
        }
    }
}
