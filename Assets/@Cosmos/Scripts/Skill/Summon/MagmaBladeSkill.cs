using UnityEngine;

public class MagmaBladeSkill : SwordSkill
{
    protected override void Activate()
    {
        SwordHandler swordHandler = FindAnyObjectByType<SwordHandler>();
        swordHandler.BurnSword(2); // 검을 불태우는 메소드 호출
        swordHandler.EnchantSwordBySwordCount(); // 검의 개수에 따라 무기 강화
        base.Activate();
    }
}