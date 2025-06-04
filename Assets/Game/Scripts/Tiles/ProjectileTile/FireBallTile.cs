using System.Collections;
using UnityEngine;

/// <summary>
/// FireBallTile은 FireBoltTile을 상속받아 세 번의 발사체를 순차적으로 발사하는 타일입니다.
/// </summary>
public class FireBallTile : FireBoltTIle
{
    private void Awake()
    {
        _damage = 5;
        _chargeTime = 3f;
    }

    /// <summary>
    /// 세 번의 발사체를 0.5초 간격으로 발사합니다.
    /// </summary>
    public override void Activate()
    {
        base.Activate();
        if (GetState() == TileState.Activated && targetEnemy != null)
        {
            StartCoroutine(FireProjectileWithDelay());
        }
    }

    private IEnumerator FireProjectileWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 딜레이 후 발사
        FireProjectile();
        yield return new WaitForSeconds(0.5f); // 0.5초 딜레이 후 발사
        FireProjectile();
    }
}
