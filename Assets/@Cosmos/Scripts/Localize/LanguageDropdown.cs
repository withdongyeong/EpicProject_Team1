using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.Localization.Settings;

public class FontChangingDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public List<TMP_FontAsset> fontsPerOption;

    private GameObject dropdownList;

    public string[] languageCodes = { "ko", "zh", "ja", "en" };
    void Start()
    {
        dropdown.onValueChanged.AddListener(OnLanguageChanged);

        // 현재 선택된 언어에 따라 드롭다운 초기 선택값 설정
        var currentCode = LocalizationSettings.SelectedLocale.Identifier.Code;
        int index = System.Array.IndexOf(languageCodes, currentCode);
        if (index >= 0)
            dropdown.value = index;

        dropdown.onValueChanged.AddListener(OnValueChanged);
        StartCoroutine(AttachDropdownOpenHandler());
    }

    void OnLanguageChanged(int index)
    {
        if (index < 0 || index >= languageCodes.Length) return;

        string selectedCode = languageCodes[index];

        var locale = LocalizationSettings.AvailableLocales.Locales
            .FirstOrDefault(l => l.Identifier.Code == selectedCode);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
            Debug.Log("언어 변경됨: " + locale.Identifier.Code);
        }
    }

    // 드롭다운이 열릴 때 동적으로 생성된 오브젝트에 접근
    IEnumerator AttachDropdownOpenHandler()
    {
        while (true)
        {
            // 드롭다운이 열렸는지 확인
            var list = GameObject.Find("Dropdown List");
            if (list != null && list != dropdownList)
            {
                dropdownList = list;
                ApplyFontsToDropdownItems();
            }
            yield return null;
        }
    }

    void ApplyFontsToDropdownItems()
    {
        var texts = dropdownList.GetComponentsInChildren<TMP_Text>(true);
        for (int i = 0; i < texts.Length; i++)
        {
            if (i < fontsPerOption.Count)
            {
                texts[i].font = fontsPerOption[i];
            }
        }
    }

    void OnValueChanged(int index)
    {
        Debug.Log($"선택된 항목: {index}");
    }
}