using UnityEngine;

public class Crow : MonoBehaviour
{
    private int _requireProtection = 15;
    private int _currentConsume = 0;
    private CrowSummonSkill _master;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        EventBus.SubscribeProtectionConsume(OnProtectionConsume);
    }

    public void Init(CrowSummonSkill skill)
    {
        _master = skill;
    }


    private void OnProtectionConsume(int num)
    {
        _currentConsume += num;
        if(_currentConsume >= _requireProtection)
        {
            Activate();
        }
    }

    private void Activate()
    {
        BaseBoss boss = FindAnyObjectByType<BaseBoss>();
        //TODO: 이거 화상말고 낙인으로 바꿀것
        boss.AddDebuff(BossDebuff.Burning);
        _master.DestoryCrow();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EventBus.UnSubscribeProtectionConsume(OnProtectionConsume);
    }
}
