using UnityEngine;

public class HealTotem : BaseTotem
{
    private PlayerHp _playerHp;


    [SerializeField] protected GameObject _healEffectPrefab;


    public override void InitializeTotem(int totemPower)
    {
        _playerHp = FindAnyObjectByType<PlayerHp>();
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
        _playerHp.Heal(_totemPower);

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
