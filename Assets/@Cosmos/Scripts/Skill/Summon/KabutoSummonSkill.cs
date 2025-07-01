using UnityEngine;

public class KabutoSummonSkill : SkillBase
{
    private GameObject _kabutoPrefab;
    private Kabuto _currentKabuto;

    protected override void Start()
    {
        base.Start();
        _kabutoPrefab = Resources.Load<GameObject>("Prefabs/Summons/Kabuto/Kabuto");
    }
    protected override void Activate()
    {
        base.Activate();
        if (_currentKabuto == null)
        {
            _currentKabuto = Instantiate(_kabutoPrefab, transform.position + Vector3.up+Vector3.right, Quaternion.identity).GetComponent<Kabuto>();
            _currentKabuto.Init(this);
        }
        else
        {
            _currentKabuto.TryCharge();
        }
    }

    public void DestoryKabuto()
    {
        _currentKabuto = null;
    }
}
