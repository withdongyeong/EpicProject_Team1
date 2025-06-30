using UnityEngine;

public class CrowSummonSkill : SkillBase
{
    private GameObject _crowPrefab;
    private Crow _currentCrow;
    

    protected override void Start()
    {
        base.Start();
        _crowPrefab = Resources.Load<GameObject>("Prefabs/Summons/Crow/Crow");

    }

    protected override void Activate()
    {
        base.Activate();
        if (_currentCrow == null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0.5f, 0.5f);
            _currentCrow = Instantiate(_crowPrefab, spawnPos, Quaternion.identity).GetComponent<Crow>();
            _currentCrow.Init(this);
        }
    }

    public void DestoryCrow()
    {
        _currentCrow = null;
    }
}
