using UnityEngine;

public class RotateQuest : GuideQuest
{
    [SerializeField]
    public int tilesRotated = 0;

    public override void SetTexts()
    {
        instructionText = "가이드 4\n ㄴ 별자리를 회전시켜보세요\n (0/5개)";
        titleText = "- 가이드 4 -";
        subTitleText = "별자리를 회전하기";
        contentText = "별자리를 회전시켜보세요\n" +
                      "별자리를 회전시키는 방법은\n별자리를 클릭한 후,\nR을 누르거나, \n마우스 오른쪽 버튼을 클릭하면 됩니다";
        goalText = "- 완료 조건 -\n" +
                   "별자리 회전시키기 (0/5)\n";
    }
    
    public override void OnStart()
    {
        
    }

    public override bool IsCompleted()
    {
        instructionText = $"가이드 4\n ㄴ 별자리를 회전시켜보세요\n ({Mathf.Min(tilesRotated,5)}/5개)";
        GuideHandler.instance.questText.text = instructionText;
        return tilesRotated >= 5 && !DragManager.Instance.IsDragging;
    }

    public override void OnComplete()
    {
        
    }
}
