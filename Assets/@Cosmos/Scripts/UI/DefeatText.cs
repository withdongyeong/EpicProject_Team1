using UnityEngine;
using TMPro;

public class DefeatText : MonoBehaviour
{
    void Start()
    {
        if (LifeManager.Instance != null && LifeManager.Instance.Life == 0)
        {
            var tmp = GetComponent<TextMeshProUGUI>();
            if (tmp != null)
                tmp.enabled = false;
        }
    }
}