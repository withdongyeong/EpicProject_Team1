using System;
using System.Collections;
using UnityEngine;

//화상, 기절
public enum BossDebuff
{
    None,// 상태 이상 없음
    Burning,
    Frostbite
}

/// <summary>
/// 보스의 상태 이상을 관리하는 클래스
/// </summary>
public class BossDebuffs : MonoBehaviour
{
    // 보스 인스턴스 참조
    private BaseBoss boss;

    // 상태 이상 UI
    private BossHPUI bossHPUI;

    // 상태 이상 배열
    private int[] debuffs = new int[Enum.GetValues(typeof(BossDebuff)).Length];

    // 상태 이상 배열 접근용 프로퍼티
    public int[] Debuffs => debuffs;

    private void Start()
    {
        boss = GetComponent<BaseBoss>();
        bossHPUI = FindAnyObjectByType<BossHPUI>();
    }

    /// <summary>
    /// 보스의 상태 이상을 추가합니다.
    /// </summary>
    /// <param name="debuff"></param>
    public void AddDebuff(BossDebuff debuff)
    {
        switch (debuff)
        {
            case BossDebuff.Burning:
                if (debuffs[(int)BossDebuff.Burning] >= 10) return; // 화상 상태 이상은 최대 5개까지만 허용
                debuffs[(int)BossDebuff.Burning]++; // 화상 상태 이상 카운트 증가
                break;
            case BossDebuff.Frostbite:
                if (debuffs[(int)BossDebuff.Frostbite] >= 5)
                    ApplyFreezingEffect(); // 동상 상태 이상이 5개 이상일 때 동결 효과 적용
                if (boss.IsStopped) return; // 보스가 이미 멈춰있으면 동상 상태 이상 추가하지 않음
                debuffs[(int)BossDebuff.Frostbite]++; // 동상 상태 이상 카운트 증가
                break;
            default:
                break; // 다른 상태 이상은 처리하지 않음
        }

        bossHPUI.UpdateDebuffUI(debuff, debuffs[(int)debuff]);
    }

    /// <summary>
    /// 보스의 상태 이상을 제거합니다.
    /// </summary>
    /// <param name="debuff"></param>
    public void RemoveDebuff(BossDebuff debuff)
    {
        if (debuffs[(int)debuff] > 0)
        {
            debuffs[(int)debuff]--; // 상태 이상 카운트 감소
            bossHPUI.UpdateDebuffUI(debuff, debuffs[(int)debuff]);
        }
    }

    /// <summary>
    /// 모든 상태 이상을 보스에게 적용합니다.
    /// </summary>
    public void ApplyAllDebuffs()
    {
        Debug.Log("Applying all debuffs to the boss...");
        // 화상 효과 적용
        if (debuffs[(int)BossDebuff.Burning] > 0)
        {
            Debug.Log($"Burning debuff count: {debuffs[(int)BossDebuff.Burning]}");

            ApplyBurningEffect();
        }

        // 동상 효과 적용
        if (debuffs[(int)BossDebuff.Frostbite] > 0)
        {
            ApplyFrostbiteEffect();
        }
    }

    /// <summary>
    /// 화상 효과를 보스에게 적용합니다.
    /// </summary>
    private void ApplyBurningEffect()
    {
        // 화상 효과 적용 로직
        boss.TakeDamage(debuffs[(int)BossDebuff.Burning]); // 화상 상태 이상에 따라 데미지 적용
        RemoveDebuff(BossDebuff.Burning);
        Debug.Log($"Burning effect applied. Remaining: {debuffs[(int)BossDebuff.Burning]}");
    }

    /// <summary>
    /// 동상 효과를 보스에게 적용합니다.
    /// </summary>
    private void ApplyFrostbiteEffect()
    {
        if (debuffs[(int)BossDebuff.Frostbite] >= 5)
        {
            ApplyFreezingEffect();
        }
    }

    /// <summary>
    /// 동상 효과 적용시 이펙트 생성
    /// </summary>
    private void ApplyFreezingEffect()
    {
        // 기존 디버프 처리
        debuffs[(int)BossDebuff.Frostbite] = 0;
        bossHPUI.UpdateDebuffUI(BossDebuff.Frostbite, 0);
        boss.StopAttack(1f);
    
        // 애니메이터 일시중지 (원래 속도 저장)
        float originalAnimatorSpeed = boss.Animator.speed;
        boss.Animator.speed = 0f;
    
        // 10초 후 애니메이터 재생 재개
        StartCoroutine(ResumeAnimatorAfterFreeze(originalAnimatorSpeed));
    
        // FreezeEffect 프리팹 소환
        GameObject freezeEffectPrefab = Resources.Load<GameObject>("Effect/FreezeEffect");
        if (freezeEffectPrefab != null)
        {
            GameObject freezeEffect = Instantiate(freezeEffectPrefab, boss.transform.position, Quaternion.identity);
        
            // 옵션: 이펙트를 boss의 자식으로 만들어서 함께 움직이게 하려면
            // freezeEffect.transform.SetParent(boss.transform);
        
            // 10초 후 이펙트 제거 (StopAttack과 동일한 시간)
            Destroy(freezeEffect, 1f);
        }
    }

    private IEnumerator ResumeAnimatorAfterFreeze(float originalSpeed)
    {
        yield return new WaitForSeconds(1f);
        boss.Animator.speed = originalSpeed; // 원래 속도로 복원
    }
}