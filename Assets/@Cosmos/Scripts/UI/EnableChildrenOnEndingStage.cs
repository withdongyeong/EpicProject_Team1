using UnityEngine;

public class EnableChildrenOnEndingStage : MonoBehaviour
{
    private void Start()
    {
        if (StageSelectManager.Instance != null && StageSelectManager.Instance.StageNum == 10)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
}