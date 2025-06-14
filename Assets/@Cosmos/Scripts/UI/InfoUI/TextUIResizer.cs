using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextUIResizer : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private float padding = 10f;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    //public void SetText(string text)
    //{
    //    textMeshPro.text = text;
    //    textMeshPro.ForceMeshUpdate();
    //    LayoutRebuilder.ForceRebuildLayoutImmediate(textMeshPro.rectTransform);
    //}

    public void SetText(string text)
    {
        textMeshPro.text = text;
        textMeshPro.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(textMeshPro.rectTransform);
    }

    public void SetText()
    {
        textMeshPro.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(textMeshPro.rectTransform);
    }
}