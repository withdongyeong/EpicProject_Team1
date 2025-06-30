using UnityEngine;

public class TurtleSummonSkill : SkillBase
{
    private GameObject _turtlePrefab;
    private TurtleBase _currentTurtle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        _turtlePrefab = Resources.Load<GameObject>("Prefabs/Summons/Turtle/Turtle");
    }

    protected override void Activate()
    {
        base.Activate();
        if(_currentTurtle == null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0.5f, 0.5f);
            _currentTurtle = Instantiate(_turtlePrefab,spawnPos,Quaternion.identity).GetComponent<TurtleBase>();
        }
        else
        {
            _currentTurtle.Fire();
            _currentTurtle = null;
        }
        
    }
}
