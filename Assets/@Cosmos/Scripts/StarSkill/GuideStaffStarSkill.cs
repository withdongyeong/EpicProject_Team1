using UnityEngine;

public class GuideStaffStarSkill : EnhancementStarSkill
{
    private bool isQuestCompleted = false;
    public void QuestClear()
    {
        if (!isQuestCompleted)
        {
            Debug.Log("[튜토리얼] 가이드 스태프 스타 스킬 퀘스트 완료");
            isQuestCompleted = true;
        }
    }
    public void QuestReset()
        {
        if (isQuestCompleted)
        {
            Debug.Log("[튜토리얼] 가이드 스태프 스타 스킬 퀘스트 실패");
            isQuestCompleted = false;
        }
    }
}
