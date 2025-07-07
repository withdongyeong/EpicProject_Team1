using UnityEngine;

public class DisableOnEndingStage : MonoBehaviour
{
    private void Start()
    {
        if (StageSelectManager.Instance != null && StageSelectManager.Instance.StageNum == 10)
        {
            gameObject.SetActive(false);
        }
    }
}