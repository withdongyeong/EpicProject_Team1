using UnityEngine;
using UnityEngine.UI;

public class EndingRetry : MonoBehaviour
{
    private bool init = false;

    private void Start()
    {
        if (!init)
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
            Button childButton = GetComponentInChildren<Button>(true); // 비활성 자식 포함해서 탐색
            if (childButton != null)
            {
                childButton.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogWarning("[EndingRetry] 자식에서 Button 컴포넌트를 찾을 수 없습니다.");
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeStageChange(OnStageChange);
    }

}