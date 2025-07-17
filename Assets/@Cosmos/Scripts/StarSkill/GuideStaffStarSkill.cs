using UnityEngine;

public class GuideStaffStarSkill : EnhancementStarSkill
{
    private bool isQuestCompleted = false;
    public void QuestClear()
    {
        if (!isQuestCompleted)
        {
            FindAnyObjectByType<StarQuest>().starActivated += 1;
            isQuestCompleted = true;
        }
    }
    public void QuestReset() {
        if (isQuestCompleted)
        {
            FindAnyObjectByType<StarQuest>().starActivated -= 1;
            isQuestCompleted = false;
        }
    }
}
