using UnityEngine;

public class TotemSummonSkill : SkillBase
{
    [SerializeField] private GameObject _totem;

    [SerializeField] int _totemPower;

    private TotemManager _totemManager;

    protected override void Activate()
    {
        base.Activate();
        _totemManager = FindAnyObjectByType<TotemManager>();
        if (_totemManager != null)
        {
            var summonedTotem = Instantiate(_totem, _totemManager.transform);
            summonedTotem.transform.localPosition = Vector3.zero;
            summonedTotem.GetComponent<BaseTotem>().InitializeTotem(_totemPower);
        }
    }
}
