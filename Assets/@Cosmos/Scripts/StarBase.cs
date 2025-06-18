using UnityEngine;

public class StarBase : MonoBehaviour
{
    
    protected CombinedStarCell combinedStarCell;
    protected TileObject tileObject;
    protected TileInfo tileInfo;
    //protected SkillUseManager skillUseManager;
    
    
    private void Awake()
    {
        //skillUseManager = SkillUseManager.Instance;
        combinedStarCell = GetComponent<CombinedStarCell>();
        if (combinedStarCell == null)
        {
            Debug.LogError("CombinedStarCell component not found in parent of StarBase.");
        }
    }
    public virtual void Activate(TileObject tile)
    {
        tileObject = tile;
        tileInfo = tileObject.GetTileData();
    }
}
