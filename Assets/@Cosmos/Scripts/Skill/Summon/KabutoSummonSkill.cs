using UnityEngine;
using UnityEngine.SceneManagement;

public class KabutoSummonSkill : SkillBase
{
    private GameObject _kabutoPrefab;
    private Kabuto _currentKabuto;

    protected override void Awake()
    {
        base.Awake();
        EventBus.SubscribeGameStart(SpawnKabuto);
        _kabutoPrefab = Resources.Load<GameObject>("Prefabs/Summons/Kabuto/Kabuto");

    }

    protected override void Start()
    {
        base.Start();
        
    }
    protected override void Activate()
    {
        base.Activate();
        _currentKabuto.TryCharge();
    }

    private void SpawnKabuto()
    {
        _currentKabuto = Instantiate(_kabutoPrefab, transform.position + Vector3.up + Vector3.right, Quaternion.identity).GetComponent<Kabuto>();
        _currentKabuto.Init(this);

    }

    public void DestoryKabuto()
    {
        _currentKabuto = null;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.UnsubscribeGameStart(SpawnKabuto);
    }


}
