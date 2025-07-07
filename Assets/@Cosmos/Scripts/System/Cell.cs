using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    
    private CombineCell combinedCell;
    private void Awake()
    {
        combinedCell = GetComponentInParent<CombineCell>();
        if (combinedCell == null)
        {
            Debug.LogError("CombineCell component not found in children of Cell.");
        }
        combinedCell.OnStarListChanged += UpdateStarList;
    }


    public CombineCell GetCombineCell()
    {
        return combinedCell;
    }

    /// <summary>
    /// 자신이 위치한 Grid의 starList를 긁어오는 함수입니다.
    /// </summary>
    private void UpdateStarList()
    {
        Vector3Int gridPos = GridManager.Instance.WorldToGridPosition(transform.position);
        List<StarBase> starSkills = GridManager.Instance.GetStarSkills(gridPos);
        combinedCell.GetTileObject().AddToStarList(starSkills);
    }

    private void OnDestroy()
    {
        if (combinedCell != null)
        {
            combinedCell.OnStarListChanged -= UpdateStarList;
        } 
    }
}
