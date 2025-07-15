using UnityEngine;

public class NewMonoBehaviourScript : BaseTotem
{
    private BaseBoss _targetEnemy;

    private void Awake()
    {
       
    }

    public override void InitializeTotem(int totemPower)
    {
        _targetEnemy = FindAnyObjectByType<BaseBoss>();
        base.InitializeTotem(totemPower);
    }

    protected override void ActivateTotem(TotemContext context)
    {
        base.ActivateTotem(context);
        for (int i = 0; i < 10; i++)
        {
            _targetEnemy.AddDebuff(BossDebuff.TemporaryCurse);
        }

    }

    protected override void ActivateTotemBetter(TotemContext context)
    {
        for (int i = 0; i < 20; i++)
        {
            _targetEnemy.AddDebuff(BossDebuff.TemporaryCurse);
        }
    }


}
