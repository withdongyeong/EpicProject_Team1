
using UnityEngine;

public class SwordSkill : SkillBase
{
    public int damage = 10; // 기본 공격력

    protected BaseBoss targetEnemy;
    private GameObject swordPrefab;
    private GameObject summonedSword;

    protected override void Awake()
    {
        base.Awake();
        swordPrefab = Resources.Load<GameObject>("Prefabs/Swords/Sword");
    }

    protected override void Activate()
    {
        base.Activate();
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
            if (swordPrefab == null) return;

            GameObject selectedSwordPrefab = swordPrefab;

            // 검 소환
            summonedSword = Instantiate(selectedSwordPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
            SwordController swordController = summonedSword.GetComponent<SwordController>();
            SwordHandler swordHandler = FindAnyObjectByType<SwordHandler>();
            if (swordController != null)
            {
                swordController.Damage = damage; // 공격력 설정
                swordController.SetTileName(tileObject.GetTileData().TileName);
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
