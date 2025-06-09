using UnityEngine;

public class HealTotem : BaseTotem
{
    private PlayerHealth _playerHealth;


    [SerializeField] protected GameObject _healEffectPrefab;


    public override void InitializeTotem(int totemPower)
    {
        _playerHealth = FindAnyObjectByType<PlayerHealth>();
        base.InitializeTotem(totemPower);
    }

    protected override void ActivateTotem(TotemContext context)
    {
        HealPlayer();
    }

    protected override void ActivateTotemBetter(TotemContext context)
    {
        base.ActivateTotemBetter(context);
    }

    /// <summary>
    /// 플레이어 체력 회복 처리
    /// </summary>
    private void HealPlayer()
    {
        // 플레이어 체력 회복
        _playerHealth.Heal(_totemPower);

        // 회복 이펙트 생성
        CreateHealEffect();
    }

    /// <summary>
    /// 회복 이펙트 생성
    /// </summary>
    private void CreateHealEffect()
    {
        if (_healEffectPrefab != null && _playerHealth != null)
        {
            // 플레이어 위치에 회복 이펙트 생성
            GameObject effectObj = Instantiate(
                _healEffectPrefab,
                _playerHealth.transform.position,
                Quaternion.identity
            );

            // 일정 시간 후 이펙트 제거
            Destroy(effectObj, 0.1f);
        }
    }
}
