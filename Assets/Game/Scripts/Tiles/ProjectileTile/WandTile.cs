using UnityEngine;

public class WandTile : ProjectileTile
{
    [SerializeField] private GameObject wandProjectilePrefab;

    private void Awake()
    {
        projectilePrefab = wandProjectilePrefab;
        _damage = 10;
        _chargeTime = 1.5f; // WandTile은 빠른 충전 시간
    }
}
