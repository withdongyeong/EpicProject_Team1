using UnityEngine;

public class TilesOnGrid : MonoBehaviour
{
    
    public void SetTileObjectStarEffect()
    {
        foreach (TileObject tile in GetComponentsInChildren<TileObject>())
        {
            Debug.Log(tile.name);
            tile.ShowStarCell();
            tile.SetStarEffect();
            tile.HideStarCell();
        }
    }
}
