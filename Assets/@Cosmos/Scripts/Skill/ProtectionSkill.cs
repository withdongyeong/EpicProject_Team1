using UnityEngine;

public class ProtectionSkill : SkillBase
{
    [SerializeField] private int _protectionAmount = 5;

    private PlayerProtection _playerProtection;

    /// <summary>
    /// 타일 발동 - 플레이어 보호 효과 부여
    /// </summary>
    protected override void Activate(GameObject user)
    {
        _playerProtection = FindAnyObjectByType<PlayerProtection>();
        if (_playerProtection != null)
        {
            GrantProtection();
        }
    }

    /// <summary>
    /// 플레이어에게 보호 상태 부여
    /// </summary>
    private void GrantProtection()
    {
        _playerProtection.SetProtection(true, _protectionAmount);
    }
}
