using UnityEngine;

public class TESTSTAGESELECTBTN : MonoBehaviour
{
    public void OnClick()
    {
        StageSelectManager.Instance.StageSelect();
    }
}
