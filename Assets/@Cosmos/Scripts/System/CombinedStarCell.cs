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
}
