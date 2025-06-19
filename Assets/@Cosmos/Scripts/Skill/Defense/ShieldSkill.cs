using UnityEngine;

public class ShieldSkill : SkillBase
{
    [SerializeField] private int _shieldAmount = 1;

    private PlayerShield _playerShield;

    /// <summary>
    /// 타일 발동 - 플레이어 방어 효과 부여
    /// </summary>
    protected override void Activate()
    {
        base.Activate();
        _playerShield = FindAnyObjectByType<PlayerShield>();
        if (_playerShield != null)
        {
            GrantShield();
        }
    }

    /// <summary>
    /// 플레이어에게 방어 상태 부여
    /// </summary>
    private void GrantShield()
    {
        _playerShield.SetShield(true, _shieldAmount);
    }
}
