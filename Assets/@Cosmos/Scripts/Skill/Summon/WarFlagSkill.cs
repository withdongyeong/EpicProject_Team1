using UnityEngine;
using System.Collections;

public class WarFlagSkill : SwordSkill
{
    private PlayerController playerController;
    private GameObject warFlagPrefab;

    protected override void Awake()
    {
        base.Awake();
        warFlagPrefab = Resources.Load<GameObject>("Prefabs/Anim/WarFlag");
        if (warFlagPrefab == null)
        {
            Debug.LogError("WarFlag prefab not found in Resources.");
        }
    }

    protected override void Activate(GameObject user)
    {
        base.Activate(user);
        playerController = FindAnyObjectByType<PlayerController>();
        SummonWarFlag();
        if (targetEnemy != null)
        {
            StartCoroutine(SummonSwordWithDelay());
        }
    }

    private void SummonWarFlag()
    {
        GameObject warFlag = Instantiate(warFlagPrefab, playerController.transform.position + new Vector3(0, 1.3f, 0), Quaternion.identity);
        warFlag.transform.SetParent(playerController.transform); // 플레이어의 자식으로 설정
    }

    private IEnumerator SummonSwordWithDelay()
    {
        yield return new WaitForSeconds(1f); // 1초 딜레이 후 소환
        SummonSword();
        yield return new WaitForSeconds(1f); // 1초 딜레이 후 소환
        SummonSword();
    }
}