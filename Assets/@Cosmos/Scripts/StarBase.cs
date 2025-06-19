using UnityEngine;

public class StarBase : MonoBehaviour
{
    
    protected CombinedStarCell combinedStarCell;
    protected TileObject tileObject;
    protected TileInfo tileInfo;

    [SerializeField] private float cooldownFactor = 0.1f;
    public float CooldownFactor => cooldownFactor;

    [SerializeField] protected StarBuff starBuff = new();

    public StarBuff StarBuff => starBuff;
    //protected SkillUseManager skillUseManager;
    
    
    protected virtual void Awake()
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
