using UnityEngine;

public class PhantomSkill : SkillBase
{
    private BaseBoss targetEnemy; // 보스 인스턴스 참조
    private BossDebuffs bossDebuffs; // 보스의 상태 이상 관리 클래스 참조
    private GameObject phantomAnim;

    protected override void Awake()
    {
        base.Awake();
        EventBus.SubscribeGameStart(OnGameStart);
        phantomAnim = Resources.Load<GameObject>("Prefabs/Anim/Phantom"); // 리소스에서 애니메이션 프리팹 로드
    }

    private void OnGameStart()
    {
        if(tileObject.IsPlaced)
        {
            targetEnemy = FindAnyObjectByType<BaseBoss>();
            bossDebuffs = targetEnemy.GetComponent<BossDebuffs>();
            if (targetEnemy == null)
            {
                Debug.LogError("PhantomSkill: Target enemy (BaseBoss) not found in the scene.");
            }
            if (bossDebuffs.MaxCurseCount < 45)
            {
                bossDebuffs.MaxCurseCount = 45; // 저주 상태 이상 최대치 설정
            }
        }
        
    }

    protected override void Activate()
    {
        base.Activate();
        LoseAllCurse(); // 모든 저주 상태 이상을 제거
        if (phantomAnim != null)
        {
            Instantiate(phantomAnim, tileObject.transform.position + new Vector3(0.5f, 0.5f, 0), Quaternion.identity); // 애니메이션 생성
        }
    }

    /// <summary>
    /// 모든 저주 상태 이상 제거 - 보스의 저주 상태 이상을 모두 제거합니다.
    /// </summary>
    private void LoseAllCurse()
    {
        bossDebuffs.RemoveAllDebuff(BossDebuff.Curse); // 보스의 모든 저주 상태 이상 제거
    }

    /// <summary>
    /// 타일 버프 적용 - 마법진 버프를 받으면 스킬이 강화됨
    /// </summary>
    /// <param name="buffData"></param>
    public override void ApplyStatBuff(TileBuffData buffData)
    {
        base.ApplyStatBuff(buffData);
        if (buffData.TileStat == BuffableTileStat.MagicCircle)
        {
            if (FindAnyObjectByType<BossDebuffs>().MaxCurseCount < 60)
            {
                FindAnyObjectByType<BossDebuffs>().MaxCurseCount = 60; // 저주 상태 이상 최대치 설정
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.UnsubscribeGameStart(OnGameStart);
    }
}