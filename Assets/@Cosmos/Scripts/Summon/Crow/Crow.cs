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
        SoundManager.Instance.PlayTileSoundClip("CrowHowling");
        _currentConsume = 0;
        BaseBoss boss = FindAnyObjectByType<BaseBoss>();
        boss.AddDebuff(BossDebuff.Mark);
    }

    private void OnDestroy()
    {
        _master.DestoryCrow();
        EventBus.UnSubscribeProtectionConsume(OnProtectionConsume);
    }
}
