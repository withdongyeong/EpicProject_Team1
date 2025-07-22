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
        OnStageChange();
    }

    private void OnStageChange()
    {
        if (StageSelectManager.Instance != null && StageSelectManager.Instance.StageNum >= 11)
        {
            if(StageSelectManager.Instance.StageNum == 11) AnalyticsManager.Instance.SessionClearEvent();
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