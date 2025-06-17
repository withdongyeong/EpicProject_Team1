using UnityEngine;

public class HealSkill : SkillBase
{
    [SerializeField] protected int _healAmount = 25;
    [SerializeField] protected GameObject _healEffectPrefab;

    private PlayerHp _playerHp;

    public int HealAmount { get => _healAmount; set => _healAmount = value; }

    /// <summary>
    /// 타일 발동 - 플레이어 체력 회복
    /// </summary>
    protected override void Activate(GameObject user)
    {
        base.Activate(user);
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

}
