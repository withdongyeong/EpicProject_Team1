using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class StoreSlotController : MonoBehaviour
{
    private StoreSlot[] storeSlots;

    //이 밑은 희귀도에 따라서 분류된 리스트입니다
    private List<GameObject> _normalStoreTiles = new();
    private List<GameObject> _rareStoreTiles = new();
    private List<GameObject> _epicStoreTiles = new();
    private List<GameObject> _legendaryStoreTiles = new();


    private void Awake()
    {
        storeSlots = GetComponentsInChildren<StoreSlot>();
        SetStoreTileList();

    }

    private void Start()
    {
        SetupStoreSlots();
    }


    private void SetupStoreSlots()
    {
        Debug.Log(storeSlots.Length);
        for (int i = 0; i < storeSlots.Length; i++)
        {
            float roll = Random.value * 100f;
            TileGrade chosenGrade;
            if(roll < GlobalSetting.Shop_NormalChance)
            {
                chosenGrade = TileGrade.Normal;
            }
            else if(roll < GlobalSetting.Shop_NormalChance + GlobalSetting.Shop_RareChance)
            {
                chosenGrade = TileGrade.Rare;
            }
            else if(roll < GlobalSetting.Shop_NormalChance + GlobalSetting.Shop_RareChance + GlobalSetting.Shop_EpicChance)
            {
                chosenGrade = TileGrade.Epic;
            }
            else
            {
                chosenGrade = TileGrade.Legendary;
            }
            Debug.Log(chosenGrade);

            List<GameObject> chosenList = _normalStoreTiles;
            switch(chosenGrade)
            {
                case TileGrade.Normal:
                    chosenList = _normalStoreTiles;
                    break;
                case TileGrade.Rare:
                    chosenList = _rareStoreTiles;
                    break;
                case TileGrade.Epic:
                    chosenList = _epicStoreTiles;
                    break;
                case TileGrade.Legendary:
                    chosenList = _legendaryStoreTiles;
                    break;

            }
            int randomIndex = Random.Range(0, chosenList.Count);
            GameObject chosenTile = chosenList[randomIndex];
            storeSlots[i].SetSlot(chosenTile.GetComponent<TileObject>().GetTileData().TileCost, chosenTile);
        
        }
    }

    public void ResetSlotBtn()
    {
        SetupStoreSlots();
    }

    /// <summary>
    /// 타일들을 리소스 파일에서 자동으로 로드해서 희귀도 따라서 분류하는 스크립트입니다
    /// </summary>
    private void SetStoreTileList()
    {
        List<GameObject> allTilePrefabs = new();

        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/BookTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/EquipTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/PotionTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/SummonTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/WeaponTile"));

        foreach(GameObject tilePrefab in allTilePrefabs)
        {
            if(tilePrefab.GetComponent<TileObject>().GetTileData().TileGrade == TileGrade.Normal)
            {
                _normalStoreTiles.Add(tilePrefab);
            }
            else if (tilePrefab.GetComponent<TileObject>().GetTileData().TileGrade == TileGrade.Rare)
            {
                _rareStoreTiles.Add(tilePrefab);
            }
            else if (tilePrefab.GetComponent<TileObject>().GetTileData().TileGrade == TileGrade.Epic)
            {
                _epicStoreTiles.Add(tilePrefab);
            }
            else
            {
                _legendaryStoreTiles.Add(tilePrefab);
            }

        }
    }
}
