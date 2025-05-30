using UnityEngine;

public class FireBoltTIle : ProjectileTile
{
    [SerializeField] private GameObject fireBoltProjectilePrefab;

    private void Awake()
    {
        projectilePrefab = fireBoltProjectilePrefab;
        _damage = 5;
        _chargeTime = 3f;
    }
}
