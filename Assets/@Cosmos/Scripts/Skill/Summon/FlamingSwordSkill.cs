using UnityEngine;

public class FlamingSwordSkill : SwordSkill
{
    protected override void Activate()
    {
        SwordHandler swordHandler = FindAnyObjectByType<SwordHandler>();
        base.Activate();
        swordHandler.BurnSword(1); // 검을 불태우는 메소드 호출
    }
}