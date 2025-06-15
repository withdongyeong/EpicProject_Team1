using UnityEngine;

public class StarBase : MonoBehaviour
{
    
    protected StarCell starCell;
    protected TileObject tileObject;
    protected TileInfo tileInfo;
    protected SkillUseManager skillUseManager;
    
    
    private void Awake()
    {
        skillUseManager = SkillUseManager.Instance;
        starCell = GetComponent<StarCell>();
        if (starCell == null)
        {
            Debug.LogError("StarCell component not found in parent of StarBase.");
        }
    }
    public virtual void Activate()
    {
        tileObject = starCell.GetTileObject();
        tileInfo = tileObject.GetTileData();
    }
}
