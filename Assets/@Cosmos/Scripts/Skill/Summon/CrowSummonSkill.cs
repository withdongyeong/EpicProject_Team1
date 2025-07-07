using UnityEngine;
using UnityEngine.SceneManagement;

public class CrowSummonSkill : NonActivateSkill
{
    private GameObject _crowPrefab;
    private Crow _currentCrow;

    protected override void Awake()
    {
        base.Awake();
        EventBus.SubscribeSceneLoaded(SpawnCrow);

    }

    protected override void Start()
    {
        base.Start();
        _crowPrefab = Resources.Load<GameObject>("Prefabs/Summons/Crow/Crow");

    }

    protected void SpawnCrow(Scene scene, LoadSceneMode mode)
    {
        if (SceneLoader.IsInStage() && tileObject.IsPlaced)
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.UnsubscribeSceneLoaded(SpawnCrow);
    }
}
