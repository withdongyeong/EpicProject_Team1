using UnityEngine;

public class SellQuest : GuideQuest
{
    [SerializeField]
    public int count = 0;
    
    public override void SetTexts()
    {
        instructionText = "가이드 6\n ㄴ 별자리를 판매해보세요\n (0/4개)";
        titleText = "- 가이드 6 -";
        subTitleText = "별자리를 판매하기";
        contentText = "별자리 드래그 중에\n하단에 판매 영역이 활성화 됩니다\n별자리를 끌어다 판매 영역에 두면\n별자리가 판매되고 \n골드를 획득할 수 있습니다";
        goalText = "- 완료 조건 -\n" +
                   "별자리 4개 판매하기 (0/4개)\n";
    }
    public override void OnStart()
    {
   
    }

    public override bool IsCompleted()
    {
        instructionText = $"가이드 6\n ㄴ 별자리를 판매해보세요\n ({count}/4개)";
        GuideHandler.instance.questText.text = instructionText;
        return count >= 4;
    }

    public override void OnComplete()
    {
        GuideHandler.instance.fightButton.interactable = true;
    }
}
