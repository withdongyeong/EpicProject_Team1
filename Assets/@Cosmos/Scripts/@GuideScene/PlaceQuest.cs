using UnityEngine;

public class PlaceQuest : GuideQuest
{
    [SerializeField]
    public int tilesPlaced = 0;
    public override void OnStart()
    {
        instructionText = "가이드 3\n ㄴ 타일을 배치하세요\n (0/5개)";
        GuideHandler.instance.questText.text = instructionText;
    }

    public override bool IsCompleted()
    {
        instructionText = $"가이드 3\n ㄴ 타일을 배치하세요\n ({tilesPlaced}/5개)";
        GuideHandler.instance.questText.text = instructionText;
        return tilesPlaced >= 5;
    }

    public override void OnComplete()
    {
        
    }
}
