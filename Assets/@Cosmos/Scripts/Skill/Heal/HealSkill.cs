using UnityEngine;

public class HealSkill : SkillBase
{
    [SerializeField] protected int _healAmount = 25;
    [SerializeField] protected int _enchantedHealAmount = 25;
    [SerializeField] protected GameObject _healEffectPrefab;
    private bool isEnchanted = false; // 스킬이 강화되었는지 여부를 나타내는 변수

    private PlayerHp _playerHp;

    public int HealAmount { get => _healAmount; set => _healAmount = value; }

    /// <summary>
    /// 타일 발동 - 플레이어 체력 회복
    /// </summary>
    protected override void Activate()
    {
        base.Activate();
        _playerHp = FindAnyObjectByType<PlayerHp>();
        if (_playerHp != null)
        {
            HealPlayer();
        }
    }

    /// <summary>
    /// 플레이어 체력 회복 처리
    /// </summary>
    private void HealPlayer()
    {
        if (isEnchanted)
        {
            // 스킬이 강화된 상태에서는 회복량 증가
            _playerHp.Heal(_enchantedHealAmount);
        }
        // 플레이어 체력 회복
        _playerHp.Heal(_healAmount);

        // 회복 이펙트 생성
        CreateHealEffect();
    }

    /// <summary>
    /// 회복 이펙트 생성
    /// </summary>
    private void CreateHealEffect()
    {
        if (_healEffectPrefab != null && _playerHp != null)
        {
            // 플레이어 위치에 회복 이펙트 생성
            GameObject effectObj = Instantiate(
                _healEffectPrefab,
                _playerHp.transform.position,
                Quaternion.identity,
                _playerHp.transform
            );

            // 일정 시간 후 이펙트 제거
            Destroy(effectObj, 0.6f);
        }
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
            isEnchanted = true; // 스킬이 강화됨
        }
    }

    /// <summary>
    /// 스킬 강화 해제 - 스킬이 강화된 상태를 초기화
    /// </summary>
    protected override void ClearStarBuff()
    {
        base.ClearStarBuff();
        isEnchanted = false; // 스킬 강화 비활성화
    }

}
