using UnityEngine;
using System.Collections;

public class ManaTurretStarSkill : StarBase
{
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject projectilePrefab;
    private BaseBoss targetEnemy;

    public int Damage { get => damage; set => damage = value; }
    
    public override void Activate(TileObject tileObject)
    {
        base.Activate(tileObject);

        // 이동불가 좌표에 위치 추가
        GridManager.Instance.AddUnmovableGridPosition(GridManager.Instance.WorldToGridPosition(transform.position));

        targetEnemy = FindAnyObjectByType<BaseBoss>();
        if (targetEnemy != null)
        {
            ActivateManaTurret();
        }
    }

    private void ActivateManaTurret()
    {
        // 무기 스킬이 먼저 나가도록 대기
        //yield return new WaitForSeconds(1f);
        if (tileInfo.TileCategory == TileCategory.Weapon && projectilePrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            Vector3 direction = (targetEnemy.transform.position - spawnPos).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
        }
    }
}
