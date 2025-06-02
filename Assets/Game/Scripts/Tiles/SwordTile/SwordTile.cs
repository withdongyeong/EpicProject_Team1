using UnityEngine;

public class SwordTile : BaseTile
{
    private int _damage = 10; // 기본 공격력
    protected BaseBoss targetEnemy;
    private GameObject[] _swordPrefabs;

    private void Awake()
    {
        _swordPrefabs = Resources.LoadAll<GameObject>("Prefabs/Swords");
    }

    private void Start()
    {
        // 타겟 적 찾기
        targetEnemy = FindAnyObjectByType<BaseBoss>();
        if (targetEnemy == null)
        {
            Debug.LogWarning("No target enemy found for SwordTile!");
        }
    }

    public override void Activate()
    {
        base.Activate();
        Debug.Log("SwordTile activated, checking for target enemy...");
        if (GetState() == TileState.Activated && targetEnemy != null)
        {
            Debug.Log("SwordTile activated, summoning sword...");
            SummonSword();
        }
    }

    /// <summary>
    /// 검 소환
    /// </summary>
    private void SummonSword()
    {
        if (targetEnemy != null)
        {
            if (_swordPrefabs == null || _swordPrefabs.Length == 0) return;

            // 랜덤하게 검 종류 선택
            int randomIndex = Random.Range(0, _swordPrefabs.Length);
            GameObject selectedSwordPrefab = _swordPrefabs[randomIndex];

            if (selectedSwordPrefab == null)
            {
                Debug.LogWarning($"Sword prefab at index {randomIndex} is null!");
                return;
            }

            // 검 소환
            GameObject newSword = Instantiate(selectedSwordPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            SwordController swordController = newSword.GetComponent<SwordController>();
            if (swordController != null)
            {
                swordController.Damage = _damage; // 공격력 설정
                swordController.ActivateSkill(targetEnemy.transform.position); // 적 1회 공격
            }
            else
            {
                Debug.LogWarning("SwordController component not found on the summoned sword prefab.");
            }
        }
    }

    public override void ModifyTilePropertiesByItemData(InventoryItemData itemData)
    {
        base.ModifyTilePropertiesByItemData(itemData);
        _damage = itemData.Damage;
    }
}
