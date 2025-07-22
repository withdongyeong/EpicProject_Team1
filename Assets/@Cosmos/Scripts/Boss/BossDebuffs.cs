using System;
using UnityEngine;
using System.Collections;

//화상, 기절
public enum BossDebuff
{
    None,// 상태 이상 없음
    Burning,
    Frostbite,
    Mark,
    Curse,
    Pain,
    Freeze, // 동결 상태 이상
    TemporaryCurse,
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

    // 동결 효과 코루틴 참조
    private Coroutine freezeCoroutine;

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
                if (debuffs[(int)BossDebuff.Burning] >= 30) return; // 화상 상태 이상은 최대 30개까지만 허용
                // 구름 상태에서는 20% 확률로화상 상태 이상 추가하지 않음
                if (isCloudy)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < 0.2f)
                    {
                        break;
                    }
                }
                debuffs[(int)BossDebuff.Burning]++; // 화상 상태 이상 카운트 증가
                PlayDebuffAnim(BossDebuff.Burning); // 화상 상태 이상 애니메이션 재생
                SetBurningAnimLevel(); // 화상 애니메이션 레벨 설정
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
                if (debuffs[(int)BossDebuff.Frostbite] >= 10)
                    ApplyFreezingEffect(); // 동상 상태 이상이 5개 이상일 때 동결 효과 적용
                break;
            case BossDebuff.Mark:
                if (debuffs[(int)BossDebuff.Mark] >= 1) return; // Mark 상태 이상은 최대 1개까지만 허용
                debuffs[(int)BossDebuff.Mark]++; // Mark 상태 이상 카운트 증가
                PlayDebuffAnim(BossDebuff.Mark); // Mark 상태 이상 애니메이션 재생
                break;
            case BossDebuff.Curse:
                if (debuffs[(int)BossDebuff.Curse] >= maxCurseCount) return; // Curse 상태 이상은 최대 maxCurseCount까지만 허용
                debuffs[(int)BossDebuff.Curse]++; // Curse 상태 이상 카운트 증가
                PlayDebuffAnim(BossDebuff.Curse); // Curse 상태 이상 애니메이션 재생
                SetCurseAnimLevel();
                break;
            case BossDebuff.Pain:
                debuffs[(int)BossDebuff.Pain]++; // Pain 상태 이상 카운트 증가
                PlayDebuffAnim(BossDebuff.Pain);
                break;
            case BossDebuff.TemporaryCurse:
                debuffs[(int)BossDebuff.TemporaryCurse]++; //TemporaryCurse 상태 이상 카운트 증가
                StartCoroutine(RemoveTemporaryCurse(1));
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
            if (debuff == BossDebuff.Mark)
            {
                // Mark 상태 이상이 제거되면 애니메이션 중지
                GameObject markEffect = GameObject.Find("MarkEffect");
                if (markEffect != null)
                {
                    Destroy(markEffect); // Mark 이펙트 제거
                }
            }
            else if (debuff == BossDebuff.Burning)
            {
                // 화상 상태 이상이 제거되면 애니메이션 레벨 초기화
                SetBurningAnimLevel();
            }
            else if (debuff == BossDebuff.Curse)
            {
                // 저주 상태 이상이 제거되면 애니메이션 레벨 초기화
                SetCurseAnimLevel();
            }
        }
    }

    /// <summary>
    /// 모든 상태 이상을 보스에게서 제거합니다.
    /// </summary>
    /// <param name="debuff"></param>
    public void RemoveAllDebuff(BossDebuff debuff)
    {
        if (debuffs[(int)debuff] > 0)
        {
            if (debuff == BossDebuff.Mark)
            {
                // Mark 상태 이상이 제거되면 애니메이션 중지
                GameObject markEffect = GameObject.Find("MarkEffect");
                if (markEffect != null)
                {
                    Destroy(markEffect); // Mark 이펙트 제거
                }
                else if (debuff == BossDebuff.Burning)
                {
                    // 화상 상태 이상이 제거되면 애니메이션 레벨 초기화
                    SetBurningAnimLevel();
                }
                else if (debuff == BossDebuff.Curse)
                {
                    // 저주 상태 이상이 제거되면 애니메이션 레벨 초기화
                    SetCurseAnimLevel();
                    if (debuffs[(int)BossDebuff.Curse] >= 60)
                    {
                        SteamAchievement.Achieve("ACH_CON_CURSE"); // 저주 상태 이상 60개 제거 시 업적
                    }
                }
            }
            debuffs[(int)debuff] = 0; // 상태 이상 카운트 초기화
            bossHPUI.UpdateDebuffUI(debuff, 0);
        }
    }


    private IEnumerator RemoveTemporaryCurse(int num)
    {
        yield return new WaitForSeconds(4f);
        debuffs[(int)BossDebuff.TemporaryCurse]--;
        if(debuffs[(int)BossDebuff.TemporaryCurse] < 0)
        {
            debuffs[(int)BossDebuff.TemporaryCurse] = 0;
        }
        bossHPUI.UpdateDebuffUI(BossDebuff.TemporaryCurse, debuffs[(int)BossDebuff.TemporaryCurse]);
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
    public void ApplyBurningEffect()
    {
        if (debuffs[(int)BossDebuff.Burning] <= 0)
        {
            return; // 화상 상태 이상이 없으면 적용하지 않음
        }
        // 화상 효과 적용 로직
        boss.TakeDamage(debuffs[(int)BossDebuff.Burning]); // 화상 상태 이상에 따라 데미지 적용
        RemoveDebuff(BossDebuff.Burning);
    }

    /// <summary>
    /// 동상 효과를 보스에게 적용합니다.
    /// </summary>
    private void ApplyFrostbiteEffect()
    {
        if (debuffs[(int)BossDebuff.Frostbite] >= 10)
        {
            ApplyFreezingEffect();
        }
    }

    /// <summary>
    /// 빙결 효과를 적용하고 이펙트 생성
    /// </summary>
    private void ApplyFreezingEffect()
    {
        SoundManager.Instance.PlayTileSoundClip("FreezeActivate");

        // 기존 디버프 처리
        debuffs[(int)BossDebuff.Frostbite] = 0;
        bossHPUI.UpdateDebuffUI(BossDebuff.Frostbite, 0);
        if (!boss.Unstoppable)
        {
            if (!boss.IsHandBoss)
            {
                boss.StopAttack(2f); // 2초 동안 공격 중지
            }
            else
            {
                boss.IncreasedDamageTaken(2f); // 손 보스의 경우 2초 동안 받는 데미지 증가
            }

                // 애니메이터 일시중지 (원래 속도 저장)
                float originalAnimatorSpeed = boss.Animator.speed;
            boss.Animator.speed = 0f;

            // 2초 후 애니메이터 재생 재개
            freezeCoroutine = StartCoroutine(ResumeAnimatorAfterFreeze(originalAnimatorSpeed));

            PlayDebuffAnim(BossDebuff.Freeze, 2); // 빙결 이펙트 재생
        }
        else
        {
            boss.IncreasedDamageTaken(2f); //  unstoppable의 경우 2초 동안 받는 데미지 증가
        }
    }

    /// <summary>
    /// 동결 효과를 즉시 중단합니다.
    /// </summary>
    public void InterruptFrostEffect()
    {
        // 동결 코루틴 중지
        if (freezeCoroutine != null)
        {
            StopCoroutine(freezeCoroutine);
            freezeCoroutine = null;
        }

        // 애니메이터 속도 복원
        boss.Animator.speed = 1f; // 기본 속도로 복원

        // 동결 상태 이상 제거
        debuffs[(int)BossDebuff.Frostbite] = 0;
        bossHPUI.UpdateDebuffUI(BossDebuff.Frostbite, 0);

        // 동결 이펙트 오브젝트 제거
        GameObject freezeEffect = GameObject.Find("FreezeEffect");
        if (freezeEffect != null)
        {
            Destroy(freezeEffect);
        }

        // 보스 공격 중지 해제
        if(!boss.Unstoppable && !boss.IsHandBoss)
        {
            boss.StopAttack(0f); // 즉시 공격 재개
        }
        else
        {
            boss.IncreasedDamageTaken(0f); // 손 보스의 경우 받는 데미지 증가 해제
        }
        
    }

    private IEnumerator ResumeAnimatorAfterFreeze(float originalSpeed)
    {
        yield return new WaitForSeconds(2f);
        boss.Animator.speed = originalSpeed; // 원래 속도로 복원
        freezeCoroutine = null;
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
    /// 화염과 동상을 저주로 변환합니다.
    /// </summary>
    public void TurnEveryDebuffsToCurse()
    {
        // 모든 상태 이상을 저주로 변환
        for (int i = 0; i < debuffs.Length; i++)
        {
            if (debuffs[i] > 0 && ((BossDebuff)i == BossDebuff.Burning || (BossDebuff)i == BossDebuff.Frostbite))
            {
                BossDebuff debuffType = (BossDebuff)i;
                int count = debuffs[i];
                for (int j = 0; j < count; j++)
                {
                    //if (debuffs[(int)BossDebuff.Curse] >= maxCurseCount)
                    //{
                    //    return; // 저주 상태 이상이 최대치에 도달하면 변환 중지
                    //}
                    RemoveDebuff(debuffType); // 기존 상태 이상 제거
                    AddDebuff(BossDebuff.Curse); // 저주 상태 이상 추가
                }
            }
        }
    }

    /// <summary>
    /// 지정된 상태 이상 애니메이션을 재생합니다.
    /// </summary>
    /// <param name="debuff">재생할 상태 이상 타입</param>
    private void PlayDebuffAnim(BossDebuff debuff)
    {
        if (GameObject.Find($"{debuff}Effect") != null)
        {
            return; // 이미 이펙트가 존재하면 중복 생성 방지
        }
        // 상태 이상 타입에 따라 프리팹 경로 동적 생성
        string prefabPath = $"Effect/{debuff}Effect";
        GameObject effectPrefab = Resources.Load<GameObject>(prefabPath);

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, boss.transform.position, Quaternion.identity);
            effect.transform.SetParent(boss.transform);
            effect.name = $"{debuff}Effect"; // 이펙트 오브젝트에 이름 지정

            if (debuff == BossDebuff.Burning)
            {
                effect.transform.position = boss.transform.position + new Vector3(0.2f, 0.5f, 0); // 화상 이펙트 위치 조정
            }
        }
    }

    /// <summary>
    /// 시간이 정해진 상태 이상 애니메이션을 재생합니다.
    /// </summary>
    /// <param name="debuff"></param>
    /// <param name="time"></param>
    private void PlayDebuffAnim(BossDebuff debuff, float time)
    {
        // 상태 이상 타입에 따라 프리팹 경로 동적 생성
        string prefabPath = $"Effect/{debuff}Effect";
        GameObject effectPrefab = Resources.Load<GameObject>(prefabPath);

        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, boss.transform.position, Quaternion.identity);
            effect.transform.SetParent(boss.transform);
            effect.name = $"{debuff}Effect"; // 이펙트 오브젝트에 이름 지정
            Destroy(effect, time); // 지정된 시간 후 이펙트 제거
        }
    }

    /// <summary>
    /// 화상 상태 이상 애니메이션 레벨을 설정합니다.
    /// </summary>
    private void SetBurningAnimLevel()
    {
        // 화상 상태 이상 애니메이션 레벨 설정
        GameObject burningEffect = GameObject.Find("BurningEffect");
        if (burningEffect == null)
        {
            Debug.LogWarning("BurningEffect not found. Cannot set Burning animation level.");
            return;
        }
        Animator animator = burningEffect.GetComponent<Animator>();
        if (animator != null)
        {
            int burningLevel = debuffs[(int)BossDebuff.Burning];
            if (burningLevel <= 0)
            {
                Destroy(burningEffect); // 화상 상태 이상이 없으면 이펙트 제거
            }
            animator.SetInteger("BurningLevel", burningLevel);
        }
    }

    /// <summary>
    /// 저주 상태 이상 애니메이션 레벨을 설정합니다.
    /// </summary>
    private void SetCurseAnimLevel()
    {
        // 저주 상태 이상 애니메이션 레벨 설정
        GameObject curseEffect = GameObject.Find("CurseEffect");
        if (curseEffect == null)
        {
            Debug.LogWarning("CurseEffect not found. Cannot set Curse animation level.");
            return;
        }
        Animator animator = curseEffect.GetComponent<Animator>();
        if (animator != null)
        {
            int curseLevel = debuffs[(int)BossDebuff.Curse];
            if (curseLevel <= 0)
            {
                Destroy(curseEffect); // 저주 상태 이상이 없으면 이펙트 제거
            }
            animator.SetInteger("CurseLevel", curseLevel);
        }
    }
}