using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Data", menuName = "Game/TileData")]
public class TileData : ScriptableObject
{
    [Header("Tile Info")]
    public string tileName;
    [TextArea]
    public string description;
    public TileCategory tileCategory;

    [Header("Tile Visuals")]
    public Sprite tileSprite;

    [Header("Gameplay Values")]
    public TileGrade tileGrade = TileGrade.Normal;
    public int tileCost = 1;
    public float tileCoolTime = 1;
    public TileData requiredTile;
    public TileData rejectTile;
    
}