using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReaperDeathArea : MonoBehaviour
{
    public GameObject DeathAreaPrefeb;

    private List<Vector3Int> _singlePointShape;
    void Start()
    {
        _singlePointShape = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0)
        };
    }

    public void DeathAreaCreationStart()
    {
        StartCoroutine(DeathAriaCreation1());
    }

    public void DeathAreaCreation2Start()
    {
        StartCoroutine(DeathAriaCreation2());
    }

    private IEnumerator DeathAriaCreation1()
    {
        int PassX1 = Random.Range(0,4);
        int PassX2 = Random.Range(5, 9);

        for (int x = 0; x < 9; x++)
        {
            if (x == PassX1 || x == PassX2) continue;

            GetComponent<BaseBoss>().BombHandler.ExecuteFixedBomb(_singlePointShape, new Vector3Int(x, 4, 0), DeathAreaPrefeb,
                warningDuration: 0.8f, explosionDuration: 2000.0f, damage: 0, warningType: WarningType.Type3);
        }

        yield return 0;
    }

    private IEnumerator DeathAriaCreation2()
    {
        int PassY1 = Random.Range(0, 4);
        int PassY2 = Random.Range(5, 9);

        for (int y = 0; y < 9; y++)
        {
            if (y == PassY1 || y == PassY2) continue;

            GetComponent<BaseBoss>().BombHandler.ExecuteFixedBomb(_singlePointShape, new Vector3Int(4, y, 0), DeathAreaPrefeb,
                                             warningDuration: 0.8f, explosionDuration: 2000.0f, damage: 0, warningType: WarningType.Type3);
        }

        yield return 0;
    }
}
