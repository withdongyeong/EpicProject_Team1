using UnityEngine;

public class TilesOnGrid : MonoBehaviour
{
    
    public void SetTileObjectStarEffect()
    {
        foreach (TileObject tile in GetComponentsInChildren<TileObject>())
        {
            tile.ShowStarCell();
            tile.SetStarEffect();
            tile.HideStarCell();
        }
    }
}
