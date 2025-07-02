using System;
using UnityEngine;

public class MoveQuest : GuideQuest
{
    private bool isW = false;
    private bool isA = false;
    private bool isS = false;
    private bool isD = false;
    private int moveCount = 0;
    
    public override void SetTexts()
    {
        instructionText = "가이드 1\n ㄴ 캐릭터를 움직이세요\n (0/4)";
        titleText = "- 가이드 1 -";
        subTitleText = "이동하기";
        contentText = "캐릭터는\n" +
                      "WASD 또는 ↑↓←→ \n" +
                      "로 움직일 수 있습니다 \n 캐릭터를 이동시켜보세요";
        goalText = "- 완료 조건 -\n" +
                   "캐릭터 상하좌우 이동하기 (0/4)\n";
    }
    public override void OnStart()
    {
        
    }

    public override bool IsCompleted()
    {
        if((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !isW)
        {
            isW = true;
            moveCount++;
        }
        if((Input.GetKeyDown(KeyCode.LeftArrow) ||Input.GetKeyDown(KeyCode.A)) && !isA)
        {
            isA = true;
            moveCount++;
        }
        if((Input.GetKeyDown(KeyCode.DownArrow) ||Input.GetKeyDown(KeyCode.S)) && !isS)
        {
            isS = true;
            moveCount++;
        }
        if((Input.GetKeyDown(KeyCode.RightArrow) ||Input.GetKeyDown(KeyCode.D)) && !isD)
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
