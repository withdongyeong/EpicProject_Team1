using UnityEngine;
using System.Collections;

public class ManaTurretStarSkill : StarBase
{
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject projectilePrefab;
    private BaseBoss targetEnemy;

    public int Damage { get => damage; set => damage = value; }

    protected override void Awake()
    {
        base.Awake();
        starBuff.RegisterActivateAction(ActivateManaTurret);
        EventBus.SubscribeGameStart(HandleGameStart);
    }

    public void HandleGameStart()
    {
        targetEnemy = FindAnyObjectByType<BaseBoss>();
        GridManager.Instance.AddUnmovableGridPosition(GridManager.Instance.WorldToGridPosition(transform.position));
    }

    private void ActivateManaTurret(TileObject tileObject)
    {
        // 무기 스킬이 먼저 나가도록 대기
        //yield return new WaitForSeconds(1f);
        if (tileObject.GetTileData().TileCategory == TileCategory.Weapon && projectilePrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            Vector3 direction = (targetEnemy.transform.position - spawnPos).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
        }
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(HandleGameStart);
        GridManager.Instance.RemoveUnmovableGridPosition(GridManager.Instance.WorldToGridPosition(transform.position));
    }
}
