using TMPro;
using UnityEngine;

public class LifeText : MonoBehaviour
{

    private void Awake()
    {
        GetComponent<TextMeshProUGUI>().text = "목숨: " + LifeManager.Instance.Life;
    }


}
