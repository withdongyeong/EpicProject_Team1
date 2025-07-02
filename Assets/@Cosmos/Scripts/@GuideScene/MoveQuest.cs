using System;
using UnityEngine;

public class MoveQuest : GuideQuest
{
    private bool isW = false;
    private bool isA = false;
    private bool isS = false;
    private bool isD = false;
    private int moveCount = 0;
    

    public override void OnStart()
    {
        instructionText = "가이드 1\n ㄴ 캐릭터를 움직이세요\n (0/4)";
        GuideHandler.instance.questText.text = instructionText;
    }

    public override bool IsCompleted()
    {
        if(Input.GetKeyDown(KeyCode.W) && !isW)
        {
            isW = true;
            moveCount++;
        }
        if(Input.GetKeyDown(KeyCode.A) && !isA)
        {
            isA = true;
            moveCount++;
        }
        if(Input.GetKeyDown(KeyCode.S) && !isS)
        {
            isS = true;
            moveCount++;
        }
        if(Input.GetKeyDown(KeyCode.D) && !isD)
        {
            isD = true;
            moveCount++;
        }
        instructionText = $"가이드 1\n ㄴ 캐릭터를 움직이세요\n ({moveCount}/4)";
        GuideHandler.instance.questText.text = instructionText;
        return moveCount >= 4;
    }

    public override void OnComplete()
    {
        
    }
}
