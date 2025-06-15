using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizeTMPFont : LocalizedAssetEvent<
    TMP_FontAsset,
    LocalizedTMPFont,
    TMPFontEvent>
{
    protected override void UpdateAsset(TMP_FontAsset localizedAsset)
    {
        var tmp = GetComponent<TextMeshProUGUI>();
        if (tmp != null && localizedAsset != null)
        {
            tmp.font = localizedAsset;
        }
    }
}

// ✅ LocalizedAsset<T>를 상속해야 함
[System.Serializable]
public class LocalizedTMPFont : LocalizedAsset<TMP_FontAsset> { }

[System.Serializable]
public class TMPFontEvent : UnityEvent<TMP_FontAsset> { }
