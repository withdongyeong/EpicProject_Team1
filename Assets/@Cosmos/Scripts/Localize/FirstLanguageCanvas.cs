using UnityEngine;

public class FirstLanguageCanvas : MonoBehaviour
{

    public void OnFirstLanguageCanvas()
    {
        this.GetComponent<Canvas>().enabled = true;
    }

    public void OffFirstLanguageCanvas()
    {
        this.GetComponent<Canvas>().enabled = false;
    }
}
