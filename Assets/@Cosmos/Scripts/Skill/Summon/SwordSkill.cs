using UnityEngine;

public class SwordSkill : SkillBase
{
    public int damage = 10; // 기본 공격력

    protected BaseBoss targetEnemy;
    private GameObject[] swordPrefabs;
    private GameObject summonedSword;

    protected virtual void Awake()
    {
        swordPrefabs = Resources.LoadAll<GameObject>("Prefabs/Swords");
    }

    protected override void Activate(GameObject user)
    {
        base.Activate(user);
        // 타겟 적 찾기
        targetEnemy = FindAnyObjectByType<BaseBoss>();
        if (targetEnemy != null)
        {
            Debug.Log($"Target enemy found: {targetEnemy.name}");
            SummonSword();
        }
    }

    /// <summary>
    /// 검 소환
    /// </summary>
    public void SummonSword()
    {
        if (targetEnemy != null)
        {
            if (swordPrefabs == null || swordPrefabs.Length == 0) return;

            // 랜덤하게 검 종류 선택
            int randomIndex = Random.Range(0, swordPrefabs.Length);
            GameObject selectedSwordPrefab = swordPrefabs[randomIndex];

            if (selectedSwordPrefab == null)
            {
                Debug.LogWarning($"Sword prefab at index {randomIndex} is null!");
                return;
            }

            // 검 소환
            summonedSword = Instantiate(selectedSwordPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            SwordController swordController = summonedSword.GetComponent<SwordController>();
            SwordHandler swordHandler = FindAnyObjectByType<SwordHandler>();
            if (swordController != null)
            {
                swordController.Damage = damage; // 공격력 설정
            }
            else
            {
                Debug.LogWarning("SwordController component not found on the summoned sword prefab.");
            }

            // 타겟 적에게 검 발사
            swordHandler.ActivateSkill(targetEnemy.transform.position);
        }
    }
}
