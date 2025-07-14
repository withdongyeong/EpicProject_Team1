using UnityEngine;

public class TotalDamageManager : Singleton<TotalDamageManager>
{
    public float totalDamage;
    protected override void Awake()
    {
        base.Awake();
        totalDamage = 0;
        EventBus.SubscribeGameStart(ResetTotalDamage);
    }
    
    public void ResetTotalDamage()
    {
        totalDamage = 0;
    }
    
    public void AddDamage(float damage)
    {
        totalDamage += damage;
    }
    
    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(ResetTotalDamage);
    }
}
