using UnityEngine;

public class DisableOnEndingStage : MonoBehaviour
{
    private bool init = false;

    private void Start()
    {
        if (!init)
        {
            EventBus.SubscribeStageChange(OnStageChange);
        }
        OnStageChange();
    }

    private void OnStageChange()
    {
        if (StageSelectManager.Instance != null && StageSelectManager.Instance.StageNum >= 11)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeStageChange(OnStageChange);
    }
}