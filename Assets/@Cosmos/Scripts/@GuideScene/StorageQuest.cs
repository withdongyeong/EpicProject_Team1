using UnityEngine;

public class StorageQuest : GuideQuest
{
    [SerializeField]
    public int count = 0;
    
    public override void SetTexts()
    {
        instructionText = "가이드 5\n ㄴ 별자리를 보관해보세요\n (0/4개)";
        titleText = "- 가이드 5 -";
        subTitleText = "별자리 보관하기";
        contentText = "별자리는 영역의 양쪽 공간에\n드래그 하여 보관할 수 있습니다\n보관한 별자리는 \n전투에 사용할 수 없습니다\n";
        goalText = "- 완료 조건 -\n" +
                   "별자리 4개 보관하기 (0/4개)\n";
    }
    public override void OnStart()
    {
        
  
    }

    public override bool IsCompleted()
    {
        instructionText = $"가이드 5\n ㄴ 별자리를 보관해보세요\n ({count}/4개)";
        GuideHandler.instance.questText.text = instructionText;
        return count >= 4;
    }

    public override void OnComplete()
    {
        
    }
}
