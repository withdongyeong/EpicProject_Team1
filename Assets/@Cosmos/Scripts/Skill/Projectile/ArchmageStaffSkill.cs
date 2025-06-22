using UnityEngine;
using System.Collections;

public class ArchmageStaffSkill : ProjectileSkill
{
    private bool isNearbyGoddess = false;
    private bool isNearbyWatch = false;
    private int axodiaCount = 0;

    public void AddAdjacentTile(string tile)
    {
        if (tile.Contains("Fire") && !isNearbyGoddess)
        {
            isNearbyGoddess = true;
            AddAxodia();
        }
        else if (tile.Contains("Frost") && !isNearbyWatch)
        {
            isNearbyWatch = true;
            AddAxodia();
        }
    }

    private void AddAxodia()
    {
        axodiaCount++;
        // Axodia가 추가될 때마다 데미지와 쿨타임을 조정합니다.
        if (axodiaCount >= 2)
        {
            damage = 10;
            cooldown *= 0.8f; // 쿨타임 감소
        }
        else if (axodiaCount >= 1)
        {
            damage += 5;
        }
    }

    protected override void ClearStarBuff()
    {
        base.ClearStarBuff();
        isNearbyGoddess = false;
        isNearbyWatch = false;
        axodiaCount = 0; // Axodia 카운트 초기화
    }

    protected override void FireProjectile()
    {
        // Axodia가 2개 이상일 때는 3개의 투사체를 발사합니다.
        if (axodiaCount >= 2)
        {
            base.FireProjectile();
            base.FireProjectile();
        }
        base.FireProjectile();
    }
}
