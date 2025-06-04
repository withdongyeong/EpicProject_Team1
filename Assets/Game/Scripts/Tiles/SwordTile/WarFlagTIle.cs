using System.Collections;
using UnityEngine;

public class WarFlagTIle : SwordTile
{
    public override void Activate()
    {
        base.Activate();
        if (GetState() == TileState.Activated && targetEnemy != null)
        {
            StartCoroutine(SummonSwordWithDelay());
        }
    }

    private IEnumerator SummonSwordWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 딜레이 후 소환
        SummonSword();
        yield return new WaitForSeconds(0.5f); // 0.5초 딜레이 후 소환
        SummonSword();
    }
}
