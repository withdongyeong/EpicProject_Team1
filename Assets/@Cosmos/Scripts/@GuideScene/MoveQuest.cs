using System;
using UnityEngine;

public class MoveQuest : GuideQuest
{
    
    private int moveCount = 0;
    
    public override void SetTexts()
    {
        instructionText = "가이드 1\n ㄴ 캐릭터를 움직이세요\n (0/5)";
        titleText = "- 가이드 1 -";
        subTitleText = "이동하기";
        contentText = "캐릭터는\n" +
                      "WASD 또는 ↑↓←→ \n" +
                      "로 움직일 수 있습니다 \n 캐릭터를 이동시켜보세요";
        goalText = "- 완료 조건 -\n" +
                   "캐릭터 이동하기 (0/5)\n";
    }
    public override void OnStart()
    {
        
    }

    public override bool IsCompleted()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            
            moveCount++;
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow) ||Input.GetKeyDown(KeyCode.A))
        {
            
            moveCount++;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {

            moveCount++;
        }

        if(Input.GetKeyDown(KeyCode.RightArrow) ||Input.GetKeyDown(KeyCode.D))
        {
            
            moveCount++;
        }
        instructionText = $"가이드 1\n ㄴ 캐릭터를 움직이세요\n ({moveCount}/5)";
        GuideHandler.instance.questText.text = instructionText;
        return moveCount >= 5;
    }

    public override void OnComplete()
    {
        
    }
}
