using UnityEngine;

public class TotemSummonSkill : SkillBase
{
    [SerializeField] private GameObject _totem;

    [SerializeField] int _totemPower;

    private TotemHandler _totemHandler;

    protected override void Activate(GameObject user)
    {
        base.Activate(user);
        _totemHandler = FindAnyObjectByType<TotemHandler>();
        if (_totemHandler != null)
        {
            var summonedTotem = Instantiate(_totem, _totemHandler.transform);
            summonedTotem.transform.localPosition = Vector3.zero;
            summonedTotem.GetComponent<BaseTotem>().InitializeTotem(_totemPower);
        }
    }
}
