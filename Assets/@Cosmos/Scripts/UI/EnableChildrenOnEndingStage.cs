using UnityEngine;

public class EnableChildrenOnEndingStage : MonoBehaviour
{
    private bool init = false;

    private void Start()
    {
        if(!init)
        {
            EventBus.SubscribeStageChange(OnStageChange);
        }
        Debug.Log("시작");
        OnStageChange();
    }

    private void OnStageChange()
    {
        if (StageSelectManager.Instance != null && StageSelectManager.Instance.StageNum >= 11)
        {
            Debug.Log("들어옴");
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeStageChange(OnStageChange);
    }
}