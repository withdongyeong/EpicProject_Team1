using UnityEngine;

public class PlaceQuest : GuideQuest
{
    [SerializeField]
    public int tilesPlaced = 0;
    
    public override void SetTexts()
    {
        instructionText = "가이드 3\n ㄴ 별자리를 설치하세요\n (0/5개)";
        titleText = "- 가이드 3 -";
        subTitleText = "별자리 설치하기";
        contentText = "별자리는 하단 상점에서 \n골드를 지불해 \n구매할 수 있습니다\n상점에서 별자리를 \n드래그 하여 영역에 설치하세요";
        goalText = "- 완료 조건 -\n" +
                   "별자리 5개 설치하기 (0/5개)\n";
    }
    public override void OnStart()
    {
        
    }

    public override bool IsCompleted()
    {
        instructionText = $"가이드 3\n ㄴ 별자리를 설치하세요\n ({tilesPlaced}/5개)";
        GuideHandler.instance.questText.text = instructionText;
        return tilesPlaced >= 5;
    }

    public override void OnComplete()
    {
        
    }
}
