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
            _currentTurtle = Instantiate(_turtlePrefab).GetComponent<TurtleBase>();
        }
        else
        {
            _currentTurtle.Fire();
            _currentTurtle = null;
        }
        
    }
}
