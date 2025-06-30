using UnityEngine;
using System.Collections;

public class FlamingSwordSkill : SwordSkill
{
    [SerializeField] private float burnDuration = 5f; // 불타는 지속 시간

    protected override void Activate()
    {
        StartCoroutine(BurnAllSword(burnDuration));
        base.Activate();
    }

    private IEnumerator BurnAllSword(float time)
    {
        SwordHandler swordHandler = FindAnyObjectByType<SwordHandler>();
        if (swordHandler != null)
        {
            swordHandler.IsBurning = true; // 불타는 상태로 설정
        }
        else
        {
            Debug.LogWarning("SwordHandler not found in the scene.");
        }
        // time 초 동안 불타는 상태 유지
        yield return new WaitForSeconds(time);
        if (swordHandler != null)
        {
            swordHandler.IsBurning = false; // 불타는 상태 해제
        }
        else
        {
            Debug.LogWarning("SwordHandler not found in the scene.");
        }
    }
}