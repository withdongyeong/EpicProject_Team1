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
    
    
    private void Awake()
    {
        //skillUseManager = SkillUseManager.Instance;
        combinedStarCell = GetComponent<CombinedStarCell>();
        if (combinedStarCell == null)
        {
            Debug.LogError("CombinedStarCell component not found in parent of StarBase.");
        }
        //실험용 구문입니다.
        starBuff.RegisterActivateAction(ActivateManaTurret);
    }

    //이것도 실험용 구문입니다.
    private void ActivateManaTurret(TileObject tile)
    {
        if(tile.GetTileData().TileCategory == TileCategory.Weapon)  
        {
            Debug.Log("마나포탑 발사");
        }
    }



    public virtual void Activate(TileObject tile)
    {
        tileObject = tile;
        tileInfo = tileObject.GetTileData();
    }
}
