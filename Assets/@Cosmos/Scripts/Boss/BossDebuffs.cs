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
    /// 동상 효과가 5개 이상일 때 보스에게 적용되는 효과입니다.
    /// </summary>
    private void ApplyFreezingEffect()
    {
        debuffs[(int)BossDebuff.Frostbite] = 0;
        bossHPUI.UpdateDebuffUI(BossDebuff.Frostbite, 0);
        boss.StopAttack(1.5f);
    }
}