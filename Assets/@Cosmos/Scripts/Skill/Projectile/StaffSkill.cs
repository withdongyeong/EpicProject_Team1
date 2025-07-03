using UnityEngine;

public class StaffSkill : ProjectileSkill
{
    protected override void Awake()
    {
        base.Awake();
        projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectiles/Staff"); // Staff 투사체 프리팹 로드
    }
}
