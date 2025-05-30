using UnityEngine;

public class StaffTile : ProjectileTile
{
    [SerializeField] private GameObject staffProjectilePrefab;

    private void Awake()
    {
        projectilePrefab = staffProjectilePrefab;
        _damage = 20;
        _chargeTime = 3f;
    }
}
