using System.Collections.Generic;
using UnityEngine;

public class CombinedStarCell : MonoBehaviour
{
    private StarBase starSkill;
    // 자식 스타셀을 저장하는 리스트
    private List<StarCell> starCell = new List<StarCell>();
    // 인접 효과를 받는 타일 오브젝트를 저장하는 리스트
    private List<TileObject> adjacentTileObjects = new List<TileObject>();

    public List<StarCell> StarCell => starCell;

    private void Awake()
    {
        gameObject.SetActive(true);
        starSkill = GetComponent<StarBase>();
        if (starSkill == null)
        {
            Debug.LogError("스타스킬이 없는데요 ???? ");
        }

        for (int i = 0; i < transform.childCount; i++)
        {
            StarCell starCellComponent = transform.GetChild(i).GetComponent<StarCell>();
            if (starCellComponent != null)
            {
                starCell.Add(starCellComponent);
            }
        }
    }

    public StarBase GetStarSkill()
    {
        return starSkill;
    }

    /// <summary>
    /// 인접 효과를 받는 타일 오브젝트 리스트를 업데이트합니다.
    /// </summary>
    public void UpdateAdjacentTileObjects()
    {
        for (int i = 0; i < starCell.Count; i++)
        {
            Vector3Int starCellGridPos = GridManager.Instance.WorldToGridPosition(starCell[i].transform.position);
            TileObject tileObject = GridManager.Instance.GetCellData(starCellGridPos).GetCombineCell().GetTileObject();
            if (tileObject != null && !adjacentTileObjects.Contains(tileObject))
            {
                adjacentTileObjects.Add(tileObject);
            }
        }
    }
}
