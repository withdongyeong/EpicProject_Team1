using UnityEngine;

public class StarBase : MonoBehaviour
{
    
    protected CombinedStarCell combinedStarCell;
    protected TileObject tileObject;
    protected TileInfo tileInfo;


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

    /// <summary>
    /// 별 칸의 조건을 만족하는지 여부를 반환합니다.
    /// </summary>
    /// <param name="skillBase"></param>
    /// <returns></returns>
    public virtual bool CheckCondition(SkillBase skillBase) => true; // 기본 구현
}