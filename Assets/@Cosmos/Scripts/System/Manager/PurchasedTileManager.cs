using System.Collections.Generic;

public class PurchasedTileManager : Singleton<PurchasedTileManager>
{
    private List<string> _purchasedTiles = new();

    public List<string> PurchasedTiles => _purchasedTiles;

    public bool IsSaved = false;

    public List<string> StoreTiles = new();
    public List<bool> PurchasedBoolList = new();

    public void AddPurchasedTiles(string tileName)
    {
        _purchasedTiles.Add(tileName);
    }

    public void RemovePurchasedTiles(string tileName)
    {
        if(_purchasedTiles.Contains(tileName))
        {
            _purchasedTiles.Remove(tileName);
        }
    }

    public void SetPurchasedTiles(List<string> tileList)
    {
        _purchasedTiles = new List<string>(tileList);
    }

}
