using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class GuardianGolemWallCreationPattern : MonoBehaviour
{
    public GameObject WallObject;
    private float countdown = 10f;
    private int DeleteCount;

    private List<Vector3Int> _singlePointShape;   
    void Start()
    {
        
        DeleteCount = 0;

        _singlePointShape = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0)
        };

        StartCoroutine(CallFunctionEverybomMinute());
    }

    private IEnumerator CallFunctionEverybomMinute()
    {
        while (true)
        {
            countdown = 10f;

            while (countdown > 0)
            {
                countdown -= Time.deltaTime;
                yield return null;

            }

            //애니메이션
            yield return new WaitForSeconds(5f); // 60초 대기

            StartCoroutine(WallCreation());
        }
    }

    private IEnumerator WallCreation()
    {
        List<Vector3Int> RightPointList = new List<Vector3Int>();
        List<Vector3Int> LeftPointList = new List<Vector3Int>();

        for (int y = 0; y < 9; y++)
        {
            RightPointList.Add(new Vector3Int(DeleteCount, y, 0));
            LeftPointList.Add(new Vector3Int(8 - DeleteCount, y, 0));

            GetComponent<BaseBoss>().BombHandler.ExecuteFixedBomb(_singlePointShape, new Vector3Int(DeleteCount, y, 0), WallObject,
                                              warningDuration: 0.8f, explosionDuration: 5.0f, damage: 0, warningType: WarningType.Type3);

            GetComponent<BaseBoss>().BombHandler.ExecuteFixedBomb(_singlePointShape, new Vector3Int(8 - DeleteCount, y, 0), WallObject,
                                  warningDuration: 0.8f, explosionDuration: 5.0f, damage: 0, warningType: WarningType.Type3);
        }

        yield return new WaitForSeconds(0.8f);

        foreach(var rightpoint in RightPointList)
        {
            GridManager.Instance.AddUnmovableGridPosition(rightpoint);
        }

        foreach(var leftpoint in LeftPointList)
        {
            GridManager.Instance.AddUnmovableGridPosition(leftpoint);
        }

        DeleteCount++;
    }
}
