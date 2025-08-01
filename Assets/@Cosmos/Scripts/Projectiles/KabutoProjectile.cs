using UnityEngine;

public class KabutoProjectile : Projectile
{
    private CircleCollider2D coll2D;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        coll2D = GetComponent<CircleCollider2D>();
        coll2D.enabled = false;
        _isRainbow = true;
    }

    public override void Initialize(Vector3 dir, ProjectileTeam projectileTeam, int givenDamage = 10, bool isRainbow = false)
    {
        base.Initialize(dir, projectileTeam, givenDamage, isRainbow);
        coll2D.enabled = true;
        transform.localScale = new Vector3(3, 3, 0);
    }

}
