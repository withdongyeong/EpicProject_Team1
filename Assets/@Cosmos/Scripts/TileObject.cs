using System;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public TileData data;
    [SerializeField]
    private TileInfo tileInfo;

    public string Description { get => tileInfo.Description; }

    private bool isInitialized = false;


    private void Awake()
    {
        InitializeTile();
    }
    
    private void InitializeTile()
    {
        if (data == null)
        {
            Debug.LogError("TileData is not assigned in TileObject.");
            return;
        }
        tileInfo = new TileInfo(data);
        
        if (tileInfo.TileSprite == null)
        {
            Debug.LogError("Tile sprite is not assigned in TileObject.");
        }
        isInitialized = true;
    }
    
    public TileInfo GetTileData()
    {
        if(!isInitialized)
        {
            InitializeTile();
        }
        return tileInfo;
    }

    public Sprite GetTileSprite()
    {
        return data.tileSprite;
    }
}

