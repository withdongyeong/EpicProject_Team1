using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GuardianGolemWallCreationPattern : MonoBehaviour
{
    public GameObject WallObject;
    public int DeleteCount;

    private float countdown = 0;
    private List<Vector3Int> AllWallPointList;

    private float beat;
    private string PatternName = "4_5";

    private List<Vector3Int> _singlePointShape;   
    void Start()
    {
        DeleteCount = 0;

        float bpm = GlobalSetting.Instance.GetBossBpm(4);
        beat = 60f / bpm;
        
        _singlePointShape = new List<Vector3Int>
        {
            new Vector3Int(0, 0, 0)
        };
        AllWallPointList = new List<Vector3Int>();
        StartCoroutine(CallFunctionEverybomMinute());
    }

    private IEnumerator CallFunctionEverybomMinute()
    {
        // 노말 기준 4.8초
        yield return new WaitForSeconds(8 * beat);

        StartCoroutine(WallCreation());

        while (DeleteCount < 2)
        {
            // 노말 기준 25.2초
            countdown =+ 42 * beat;
              
            while (countdown > 0)
            {
                countdown -= Time.deltaTime;
                yield return null;
            }

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
                                              warningDuration: 1f, explosionDuration: 1000.0f, damage: 0, warningType: WarningType.Type3, patternName:PatternName);

            GetComponent<BaseBoss>().BombHandler.ExecuteFixedBomb(_singlePointShape, new Vector3Int(8 - DeleteCount, y, 0), WallObject,
                                  warningDuration: 1f, explosionDuration: 1000.0f, damage: 0, warningType: WarningType.Type3, patternName:PatternName);
        }

        yield return new WaitForSeconds(1f);

        foreach(var rightpoint in RightPointList)
        {
            AllWallPointList.Add(rightpoint);
            GridManager.Instance.AddUnmovableGridPosition(rightpoint);
        }

        foreach(var leftpoint in LeftPointList)
        {
            AllWallPointList.Add(leftpoint);
            GridManager.Instance.AddUnmovableGridPosition(leftpoint);
        }

        DeleteCount++;
    }

    private void OnDestroy()
    {
        WallClear();
    }

    private void WallClear()
    {
        foreach(var wallPoint in AllWallPointList)
        {
            GridManager.Instance.RemoveUnmovableGridPosition(wallPoint);
        }
    }
}
