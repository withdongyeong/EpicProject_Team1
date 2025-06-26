using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    private int damage = 20;

    public void Activate(string name)
    {
        switch (name)
        {
            case "Cloud":
                ActiveCloudSkill();
                break;
            case "RainCloud":
                ActiveRainCloudSkill();
                break;
            case "ThunderstormCloud":
                ActiveThunderstormCloudSkill();
                break;
            default:
                Debug.LogWarning($"Unknown cloud type: {name}. No skill activated.");
                break;
        }
    }

    private void ActiveCloudSkill()
    {
        transform.parent.GetComponent<BossDebuffs>().IsCloudy = true;
    }

    private void ActiveRainCloudSkill()
    {
        
    }

    private void ActiveThunderstormCloudSkill()
    {
        if (projectilePrefab != null)
        {
            Vector3 direction = (transform.parent.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
            Quaternion clockwise90 = Quaternion.Euler(0, 0, -90);
            projectileObj.transform.rotation = lookRotation * clockwise90;
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, damage);
        }
    }
}
