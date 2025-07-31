using TMPro;
using UnityEngine;
using UnityEngine.Localization;

/// <summary>
/// 무한모드 스테이지 번호 UI 컨트롤러
/// InfiniteCount > 0이고 StageNum이 11일 때만 자식 UI 표시
/// </summary>
public class InfiniteStageNum : MonoBehaviour
{
    /// <summary>
    /// 컴포넌트 초기화 및 무한모드 UI 표시 여부 설정
    /// </summary>
    void Start()
    {
        UpdateInfiniteStageUI();
    }

    /// <summary>
    /// 무한모드 조건에 따른 자식 UI 활성화/비활성화 및 텍스트 업데이트
    /// InfiniteCount > 0이고 StageNum == 11일 때만 표시
    /// </summary>
    private void UpdateInfiniteStageUI()
    {
        bool shouldShow = false;
        int infiniteCount = 0;

        if (StageSelectManager.Instance != null)
        {
            infiniteCount = StageSelectManager.Instance.InfiniteModeCount;
            int stageNum = StageSelectManager.Instance.StageNum;

            // 무한모드 카운트가 0보다 크고 스테이지가 11(LastBoss)일 때만 표시
            shouldShow = infiniteCount > 0 && stageNum == 11;
        }

        // 모든 자식 오브젝트 활성화/비활성화
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(shouldShow);
        }

        // UI가 표시될 때 TMP 텍스트 업데이트
        if (shouldShow)
        {
            UpdateInfiniteStageText(infiniteCount);
            Debug.Log($"[InfiniteStageNum] 무한모드 UI 표시 - 도전 횟수: {infiniteCount}");
        }
    }

    /// <summary>
    /// 자식에서 TMP를 찾아서 무한모드 텍스트 업데이트
    /// </summary>
    /// <param name="infiniteCount">현재 무한모드 도전 횟수</param>
    private void UpdateInfiniteStageText(int infiniteCount)
    {
        TextMeshProUGUI infiniteText = GetComponentInChildren<TextMeshProUGUI>();
        LocalizedString infiniteLocal = new LocalizedString("EpicProject_Table", "UI_Text_NowInfinityMode");

        if (infiniteText != null)
        {
            infiniteLocal.StringChanged += (value) =>
            {
                infiniteText.text = value.Replace("{0}", infiniteCount.ToString());
            };
        }
    }
}