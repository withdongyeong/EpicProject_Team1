using UnityEngine;
using System.Collections;

public class WarFlagSkill : SwordSkill
{
    protected override void Activate(GameObject user)
    {
        base.Activate(user);
        if (targetEnemy != null)
        {
            StartCoroutine(SummonSwordWithDelay());
        }
    }

    private IEnumerator SummonSwordWithDelay()
    {
        yield return new WaitForSeconds(1f); // 1초 딜레이 후 소환
        SummonSword();
        yield return new WaitForSeconds(1f); // 1초 딜레이 후 소환
        SummonSword();
    }
}
