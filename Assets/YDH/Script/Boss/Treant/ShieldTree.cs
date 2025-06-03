using UnityEngine;

public class ShieldTree : BaseEnemy
{
    //실드 트리

    protected override void Start()
    {
        // 기본 스탯 설정
        MaxHealth = 40;
        PatternCooldown = 0.6f;

        // 부모 클래스 초기화 호출
        base.Start();
    }
}
