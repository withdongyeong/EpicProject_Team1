using UnityEngine;
using System.Collections;

public class TurtreePattern1 : IBossAttackPattern
{
    private GameObject _slimeFloorPrefeb;

    public string PatternName => "TurtreePattern1";
    public TurtreePattern1(GameObject SlimeFloorPrefeb)
    {
        _slimeFloorPrefeb = SlimeFloorPrefeb;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        //boss.AttackAnimation();
        yield return TreeCirclePattern(boss);
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _slimeFloorPrefeb != null;
    }

    //동글동글
    public IEnumerator TreeCirclePattern(BaseBoss boss)
    {
        yield return 0;
    }
}
