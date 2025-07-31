using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 일반 스테이지 번호 UI 컨트롤러
/// Stage가 1~10이고 InfiniteCount가 0일 때만 자식 UI 표시
/// 스테이지에 따라 OutLine 이미지 스프라이트 변경
/// </summary>
public class StageNum : MonoBehaviour
{
    [SerializeField] private Sprite[] stageSprites = new Sprite[4]; // 4개의 스테이지 스프라이트
    [SerializeField] private Image outlineImage; // OutLine 이미지 컴포넌트
    /// <summary>
    /// 컴포넌트 초기화 및 일반 스테이지 UI 표시 여부 설정
    /// </summary>
    void Start()
    {
        UpdateNormalStageUI();
    }

    /// <summary>
    /// 일반 스테이지 조건에 따른 자식 UI 활성화/비활성화 및 텍스트 업데이트
    /// Stage가 1~10이고 InfiniteCount == 0일 때만 표시
    /// </summary>
    private void UpdateNormalStageUI()
    {
        bool shouldShow = false;
        int stageNum = 0;

        if (StageSelectManager.Instance != null)
        {
            int infiniteCount = StageSelectManager.Instance.InfiniteModeCount;
            stageNum = StageSelectManager.Instance.StageNum;

            // 스테이지가 1~10이고 무한모드 카운트가 0일 때만 표시
            shouldShow = stageNum >= 1 && stageNum <= 10 && infiniteCount == 0;
        }

        // 모든 자식 오브젝트 활성화/비활성화
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(shouldShow);
        }

        // UI가 표시될 때 TMP 텍스트 및 OutLine 이미지 업데이트
        if (shouldShow)
        {
            UpdateNormalStageText(stageNum);
            UpdateOutlineImage(stageNum);
            Debug.Log($"[StageNum] 일반 스테이지 UI 표시 - 스테이지: {stageNum}");
        }
    }

    /// <summary>
    /// 자식에서 TMP를 찾아서 일반 스테이지 텍스트 업데이트
    /// </summary>
    /// <param name="stageNum">현재 스테이지 번호</param>
    private void UpdateNormalStageText(int stageNum)
    {
        TextMeshProUGUI stageText = GetComponentInChildren<TextMeshProUGUI>();
        if (stageText != null)
        {
            stageText.text = $"스테이지 {stageNum}";
        }
    }

    /// <summary>
    /// 스테이지 번호에 따라 OutLine 이미지 스프라이트 변경
    /// 스테이지 1: 스프라이트[0], 스테이지 2: 스프라이트[0]
    /// 스테이지 3,4: 스프라이트[1], 스테이지 5,6,7: 스프라이트[2]
    /// 스테이지 8,9,10: 스프라이트[3]
    /// </summary>
    /// <param name="stageNum">현재 스테이지 번호</param>
    private void UpdateOutlineImage(int stageNum)
    {
        if (outlineImage == null || stageSprites.Length < 4) return;

        int spriteIndex = 0;

        switch (stageNum)
        {
            case 1:
            case 2:
                spriteIndex = 0;
                break;
            case 3:
            case 4:
                spriteIndex = 1;
                break;
            case 5:
            case 6:
            case 7:
                spriteIndex = 2;
                break;
            case 8:
            case 9:
            case 10:
                spriteIndex = 3;
                break;
            default:
                spriteIndex = 0;
                break;
        }

        if (stageSprites[spriteIndex] != null)
        {
            outlineImage.sprite = stageSprites[spriteIndex];
        }
    }
}