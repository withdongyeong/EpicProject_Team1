using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReaperDeathAria : MonoBehaviour
{
    public GameObject DeathAriaPrefeb;

    private List<Vector3Int> _singlePointShape;
    void Start()
    {
        _singlePointShape = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0)
        };
        StartCoroutine(CallFunctionEverybomMinute());
    }

    private IEnumerator CallFunctionEverybomMinute()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(DeathAriaCreation());
    }

    private IEnumerator DeathAriaCreation()
    {
        List<Vector3Int> PointList = new List<Vector3Int>();

        for (int x = 1; x <= 7; x++)
        {
            for(int y = 1; y <= 7; y++)
            {
                if(x == 1 || x == 7 || y == 1 || y == 7)
                {
                    PointList.Add(new Vector3Int(x, y, 0));

                    GetComponent<BaseBoss>().BombHandler.ExecuteFixedBomb(_singlePointShape, new Vector3Int(x, y, 0), DeathAriaPrefeb,
                                             warningDuration: 0.8f, explosionDuration: 2000.0f, damage: 0, warningType: WarningType.Type3);
                }
            }
        }

        yield return 0;
    }
}
