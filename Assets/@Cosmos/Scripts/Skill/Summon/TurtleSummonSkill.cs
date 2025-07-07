using UnityEngine;

public class TurtleSummonSkill : SkillBase
{
    private GameObject _turtlePrefab;
    private TurtleBase _currentTurtle;

    protected override void Awake()
    {
        base.Awake();
        _turtlePrefab = Resources.Load<GameObject>("Prefabs/Summons/Turtle/Turtle");
        EventBus.SubscribeGameStart(SpawnTurtle);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    

    private void SpawnTurtle()
    {
        if(tileObject.IsPlaced)
        {
            Vector3 spawnPos = transform.TransformPoint(new Vector3(0.5f, 0.5f));
            Quaternion rotate = transform.parent.rotation;
            _currentTurtle = Instantiate(_turtlePrefab, spawnPos, rotate).GetComponent<TurtleBase>();
        }
        
    }

    protected override void Activate()
    {
        base.Activate();
        _currentTurtle.Fire();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.UnsubscribeGameStart(SpawnTurtle);
    }
}
