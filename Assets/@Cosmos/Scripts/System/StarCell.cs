using UnityEngine;

public class StarCell : Cell
{
    private StarBase starSkill;
    private CombinedStarCell _combinedStarCell;

    private void Awake()
    {
        
    }

    private void Start()
    {
        _combinedStarCell = transform.parent.GetComponent<CombinedStarCell>();
        starSkill = _combinedStarCell.GetStarSkill();
    }

    public Vector3Int GetStarCellPosition()
    {
        return GridManager.Instance.WorldToGridPosition(transform.position);
    }

    
    // 스타셀 효과를 적용받고 있는 타일 오브젝트를 반환합니다.
    public TileObject GetTileObject()
    {
        if(GridManager.Instance.GetCellData(GetStarCellPosition()) == null)
        {
            Debug.LogError("스타셀 위치에 해당하는 CellData가 없습니다.");
            return null;
        }
        return GridManager.Instance.GetCellData(GetStarCellPosition()).GetCombineCell().GetTileObject();
    }

    public StarBase GetStarSkill()
    {
        return starSkill;
    }

    public CombinedStarCell GetCombinedStarCell()
    {
        return _combinedStarCell;
    }

}
