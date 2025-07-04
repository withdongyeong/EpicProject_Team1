using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    private BaseBoss _boss;
    private int _damage = 40;

    public void Init(string name)
    {
        _boss = transform.parent.gameObject.GetComponent<BaseBoss>();
        switch (name)
        {
            case "Cloud":
                transform.parent.GetComponent<BossDebuffs>().IsCloudy = true;
                break;
            case "RainCloud":
                StartCoroutine(RainCloudCoroutine());
                break;
            case "ThunderstormCloud":
                break;
            default:
                Debug.LogWarning($"Unknown cloud type: {name}. No skill activated.");
                break;
        }
    }

    public void Activate(string name)
    {
        switch (name)
        {
            case "Cloud":
                break;
            case "RainCloud":
                break;
            case "ThunderstormCloud":
                ActiveThunderstormCloudSkill();
                break;
            default:
                Debug.LogWarning($"Unknown cloud type: {name}. No skill activated.");
                break;
        }
    }

    /// <summary>
    /// 비 구름 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator RainCloudCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // 5초 대기
            if (_boss != null)
            {
                _boss.AddDebuff(BossDebuff.Mark); // 낙인 상태 이상 추가
            }
            else
            {
                Debug.LogWarning("Boss is null, stopping RainCloudCoroutine.");
                yield break; // 보스가 없으면 코루틴 종료
            }
        }
    }

    /// <summary>
    /// 번개 구름 스킬 활성화
    /// </summary>
    private void ActiveThunderstormCloudSkill()
    {
        if (projectilePrefab != null)
        {
            Vector3 direction = (transform.parent.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Quaternion lookRotation = Quaternion.LookRotation(Vector3.forward, direction);
            Quaternion clockwise90 = Quaternion.Euler(0, 0, 90);
            projectileObj.transform.rotation = lookRotation * clockwise90;
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction, Projectile.ProjectileTeam.Player, _damage);
            if( transform.parent.GetComponent<BaseBoss>().GetDebuffCount(BossDebuff.Mark) > 0)
            {
                projectile.BossDebuff = BossDebuff.Mark; // 낙인 상태 이상 적용
            }
        }
    }
}
