using System;
using UnityEngine;

//화상, 기절
public enum BossDebuff
{
    None,// 상태 이상 없음
    Burning,
    Frostbite,
    Mark,
    Curse,
    Pain,
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

    // 구름 상태 여부
    private bool isCloudy = false; 

    // 저주 상태 이상 최대치
    private int maxCurseCount = 30;

    // 상태 이상 배열 접근용 프로퍼티
    public int[] Debuffs => debuffs;

    // 구름 상태 여부 프로퍼티
    public bool IsCloudy { get => isCloudy; set => isCloudy = value; }

    // 저주 상태 이상 최대치 프로퍼티
    public int MaxCurseCount { get => maxCurseCount; set => maxCurseCount = value; }

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
                if (debuffs[(int)BossDebuff.Burning] >= 10) return; // 화상 상태 이상은 최대 10개까지만 허용
                // 구름 상태에서는 20% 확률로화상 상태 이상 추가하지 않음
                if (isCloudy)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
                    {
                        break;
                    }
                }
                debuffs[(int)BossDebuff.Burning]++; // 화상 상태 이상 카운트 증가
                break;
            case BossDebuff.Frostbite:
                if (boss.IsStopped) return; // 보스가 이미 멈춰있으면 동상 상태 이상 추가하지 않음
                // 구름 상태에서는 20% 확률로 동상 상태 이상 두배로 부여
                if (isCloudy)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
                    {
                        debuffs[(int)BossDebuff.Frostbite]++; // 동상 상태 이상 두배로 증가
                    }
                }
                debuffs[(int)BossDebuff.Frostbite]++; // 동상 상태 이상 카운트 증가
                if (debuffs[(int)BossDebuff.Frostbite] >= 5)
                    ApplyFreezingEffect(); // 동상 상태 이상이 5개 이상일 때 동결 효과 적용
                break;
            case BossDebuff.Mark:
                if (debuffs[(int)BossDebuff.Mark] >= 1) return; // Mark 상태 이상은 최대 1개까지만 허용
                debuffs[(int)BossDebuff.Mark]++; // Mark 상태 이상 카운트 증가
                break;
            case BossDebuff.Curse:
                if (debuffs[(int)BossDebuff.Curse] >= maxCurseCount) return; // Curse 상태 이상은 최대 maxCurseCount까지만 허용
                debuffs[(int)BossDebuff.Curse]++; // Curse 상태 이상 카운트 증가
                break;
            case BossDebuff.Pain:
                debuffs[(int)BossDebuff.Pain]++; // Pain 상태 이상 카운트 증가
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
        // 화상 효과 적용
        if (debuffs[(int)BossDebuff.Burning] > 0)
        {
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
        boss.StopAttack(1f);
    }

    /// <summary>
    /// 낙인 상태 이상이 있을 때 데미지를 증가시키는 효과를 적용합니다.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public int ApplyMarkEffect(int damage)
    {
        if (debuffs[(int)BossDebuff.Mark] > 0)
        {
            // 낙인 상태 이상이 있을 때 데미지 증가
            int markDamage = damage + (int)(damage * 0.5f); // 50% 추가 데미지
            RemoveDebuff(BossDebuff.Mark);
            return markDamage;
        }
        return damage; // 낙인 상태 이상이 없으면 원래 데미지 반환
    }

    /// <summary>
    /// 고통 상태 이상이 있을 때 데미지를 증가시키는 효과를 적용합니다.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns></returns>
    public int ApplyPainEffect(int damage)
    {
        if (debuffs[(int)BossDebuff.Pain] > 0)
        {
            // Pain 상태 이상이 있을 때 데미지 증가
            int painDamage = damage + (int)(damage * debuffs[(int)BossDebuff.Pain] * 0.1f); // 고통 스택당 10% 추가 데미지
            return painDamage;
        }
        return damage; // Pain 상태 이상이 없으면 원래 데미지 반환
    }

    /// <summary>
    /// 모든 상태 이상을 저주로 변환합니다.
    /// </summary>
    public void TurnEveryDebuffsToCurse()
    {
        // 모든 상태 이상을 저주로 변환
        for (int i = 0; i < debuffs.Length; i++)
        {
            if (debuffs[i] > 0 && (BossDebuff)i != BossDebuff.Curse)
            {
                BossDebuff debuffType = (BossDebuff)i;
                int count = debuffs[i];
                for (int j = 0; j < count; j++)
                {
                    //if(debuffs[(int)BossDebuff.Curse] >= maxCurseCount)
                    //{
                    //    return; // 저주 상태 이상이 최대치에 도달하면 변환 중지
                    //}
                    RemoveDebuff(debuffType); // 기존 상태 이상 제거
                    AddDebuff(BossDebuff.Curse); // 저주 상태 이상 추가
                }
            }
        }
    }
}