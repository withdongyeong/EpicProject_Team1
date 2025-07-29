using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontChangingDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public List<TMP_FontAsset> fontsPerOption;

    private GameObject dropdownList;


    void Start()
    {
        //언어 변경
        dropdown.onValueChanged.AddListener(LocalizeManager.Instance.OnLanguageChanged);

        //현재 언어에 따라 드롭다운 위치 변경
        if (LocalizeManager.Instance.LocalizedIndex >= 0)
            dropdown.value = LocalizeManager.Instance.LocalizedIndex;

        StartCoroutine(AttachDropdownOpenHandler());
    }

    // 드롭다운이 열릴 때 동적으로 생성된 오브젝트에 접근
    IEnumerator AttachDropdownOpenHandler()
    {
        while (true)
        {
            // 드롭다운이 열렸는지 확인
            var list = transform.Find("Dropdown List");
            
            if (list != null && list.gameObject != dropdownList)
            {
                dropdownList = list.gameObject;
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
}