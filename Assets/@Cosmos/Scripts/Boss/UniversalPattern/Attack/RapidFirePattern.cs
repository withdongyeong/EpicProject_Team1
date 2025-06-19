using UnityEngine;
using System.Collections;

public class RapidFirePattern : IBossAttackPattern
{
    private GameObject _projectilePrefab;
    private int _shotCount;
    private float _shotInterval;

    public string PatternName => "Rapid Fire";

    public RapidFirePattern(GameObject projectilePrefab, int shotCount = 4, float shotInterval = 0.4f)
    {
        _projectilePrefab = projectilePrefab;
        _shotCount = shotCount;
        _shotInterval = shotInterval;
    }
    public IEnumerator Execute(BaseBoss boss)
    {
        yield return boss.StartCoroutine(ExecuteRapidFire(boss));
    }

    public bool CanExecute(BaseBoss boss)
    {
        return boss.BombHandler.PlayerController != null && _projectilePrefab != null;
    }

    /// <summary>
    /// ���� �߻� ����
    /// </summary>
    private IEnumerator ExecuteRapidFire(BaseBoss boss)
    {
        for (int i = 0; i < _shotCount; i++)
        {
            Vector3 direction = (boss.BombHandler.PlayerController.transform.position - boss.transform.position).normalized;
            GameObject projectileObj = TileObject.Instantiate(_projectilePrefab, boss.transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.Initialize(direction, Projectile.ProjectileTeam.Enemy);
            }

            yield return new WaitForSeconds(_shotInterval);
        }
    }
}
