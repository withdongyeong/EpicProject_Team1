using System;
using UnityEngine;

public class TileObject : MonoBehaviour
{
    public TileData data;
    [SerializeField]
    private TileInfo tileInfo;
    private GameObject combinedStarCell;

    public string Description { get => tileInfo.Description; }
    public GameObject CombinedStarCell { get => combinedStarCell; }

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
        combinedStarCell = GetComponentInChildren<CombinedStarCell>() ? GetComponentInChildren<CombinedStarCell>().gameObject : null;
        if (combinedStarCell != null)
        {
            //combinedStarCell.SetActive(false); // 스타셀의 부모 오브젝트 비활성화
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

