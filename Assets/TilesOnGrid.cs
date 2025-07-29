using UnityEngine;

public class TilesOnGrid : MonoBehaviour
{

    public void SetTileObjectStarEffect()
    {
        GameManager.Instance.LogHandler.EnforcedTileNum = 0;
        foreach (TileObject tile in GetComponentsInChildren<TileObject>())
        {
            tile.ShowStarCell();
            tile.SetStarEffect();
            tile.HideStarCell();
        }
    }

    public int GetEnforcedStarNum()
    {
        int result = 0;
        foreach (TileObject tile in GetComponentsInChildren<TileObject>())
        {
            result += tile.EnforcedStarCount;
        }
        return result;
    }

}
