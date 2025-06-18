using UnityEngine;
using System.Collections;

public class SlimeFloorPattern3 : IBossAttackPattern
{
    private GameObject _slimeFloorPrefeb;

    public string PatternName => "SlimeFloorPattern3";
    public SlimeFloorPattern3(GameObject SlimeFloorPrefeb)
    {
        _slimeFloorPrefeb = SlimeFloorPrefeb;
    }

    public IEnumerator Execute(BaseBoss boss)
    {
        yield return SlimeFloorPattern(boss);
    }

    public bool CanExecute(BaseBoss boss)
    {
        return _slimeFloorPrefeb != null;
    }

    public IEnumerator SlimeFloorPattern(BaseBoss boss)
    {
        yield return 0;
    }
}
