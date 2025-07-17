using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StoreSlotController : MonoBehaviour
{
    private StoreSlot[] storeSlots;

    private int _rerollNum = 0; //도전과제용 리롤한 횟수입니다.

    private int _safeInt = 0;
    private int _unlockLevel;

    private GameObject _safePrefab; //만약 에러가 뜨면 슬롯에 넣어줄 안전장치 입니다. 현재는 지팡이입니다.

    private List<string> appeardTileNameList = new();

    //희귀도에 따라서 분류된 리스트를 모아놓은 리스트입니다
    private List<List<GameObject>> _storeTiles = new();

    //1스테이지 상점에 처음 나올 5개 타일입니다
    private List<GameObject> _firstStoreTiles = new();
    //가이드용
    public List<GameObject> guideList = new List<GameObject>();
    public bool isGuide = false;


    private void Awake()
    {
        storeSlots = GetComponentsInChildren<StoreSlot>();
        SetStoreTileList();

    }

    private void Start()
    {      
        SetupStoreSlots();

        //만약 1스테이지 못깬 사람의 첫 스테이지라면
        if (StageSelectManager.Instance.StageNum == 1 || GameManager.Instance.CurrentUnlockLevel < 1)
        {
            SetUpFirstStoreSlots();
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.D) && !SceneLoader.IsInGuide())
        {
            if(!DragManager.Instance.IsDragging)
            {
                ResetSlotBtn();
              
            }            
        }
    }
    
    private void SetupStoreSlots()
    {
        //슬롯들을 채워야 하면 각 확률 리스트들을 초기화해줍니다.
        SetStoreTileList();

        //조건 판별에 쓰이는 리스트를 초기화해줍니다.
        appeardTileNameList.Clear();
        
        
        for (int i = 0; i < storeSlots.Length; i++)
        {
            GameObject chosenTile;

            //먼저 잠금된 애들이 있으면 반영합니다.
            if (StoreLockManager.Instance.GetStoreLocks(i) != null)
            {
                chosenTile = StoreLockManager.Instance.GetStoreLocks(i);
            }
            else //확률로 정해지는 애들입니다.
            {
                float roll = Random.value * 100f;
                TileGrade chosenGrade;
                int stageNum;
                if(StageSelectManager.Instance.StageNum > 10)
                {
                    stageNum = 10;
                }
                else
                {
                    stageNum = StageSelectManager.Instance.StageNum;
                }

                //현재 확률
                ShopChanceClass chanceList = GlobalSetting.Shop_ChanceList[stageNum];
                
                if (roll < chanceList.shop_NormalChance)
                {
                    chosenGrade = TileGrade.Normal;
                }
                else if (roll < chanceList.shop_NormalChance + chanceList.shop_RareChance)
                {
                    chosenGrade = TileGrade.Rare;
                }
                else if (roll < chanceList.shop_NormalChance + chanceList.shop_RareChance + chanceList.shop_EpicChance)
                {
                    chosenGrade = TileGrade.Epic;
                }
                else if (roll < chanceList.shop_NormalChance + chanceList.shop_RareChance + chanceList.shop_EpicChance + chanceList.shop_LegendaryChance)
                {
                    chosenGrade = TileGrade.Legendary;
                }
                else
                {
                    chosenGrade = TileGrade.Mythic;
                }

                List<GameObject> chosenList = _storeTiles[(int)TileGrade.Normal];
                chosenList = _storeTiles[(int)chosenGrade];

                //가이드용
                if (isGuide)
                {
                    chosenList = guideList;
                    storeSlots[i].SetSlot(chosenList[0].GetComponent<TileObject>().GetTileData().TileCost, chosenList[0]);
                    storeSlots[i].GetComponent<Image>().SetNativeSize();
                    continue;
                }

                int randomIndex;
                //이건 선택된 리스트에 남은 애들이 있을때 입니다
                if (chosenList.Count > 0)
                {
                    randomIndex = Random.Range(0, chosenList.Count);
                }
                else //이건 리스트가 비어있으면 다시 채우는 과정입니다
                {
                    RefillList(chosenGrade);
                    chosenList = _storeTiles[(int)chosenGrade];
                    randomIndex = Random.Range(0, chosenList.Count);
                }

                chosenTile = chosenList[randomIndex];
                //선택된 타일이 상점에 등장해도 되는지 조건검사를 합니다.
                chosenTile = CheckTileCondition(chosenTile, chosenGrade);
                //중복이 안되도록 제거해줍니다
                if(_storeTiles[(int)chosenGrade].Contains(chosenTile))
                {
                    _storeTiles[(int)chosenGrade].Remove(chosenTile);
                }

            }
            
            storeSlots[i].SetSlot(chosenTile.GetComponent<TileObject>().GetTileData().TileCost, chosenTile);
            appeardTileNameList.Add(chosenTile.GetComponent<TileObject>().GetTileData().TileName);
            
            
            //이미지 비율을 맞추기 위한 코드입니다.
            //storeSlots[i].GetComponent<Image>().preserveAspect = true;
            storeSlots[i].GetComponent<Image>().SetNativeSize();
        }
    }

    private void SetUpFirstStoreSlots()
    {
        for(int i = 0; i < storeSlots.Length; i++)
        {
            if(i<_firstStoreTiles.Count)
            {
                GameObject chosenTile = _firstStoreTiles[i];
                storeSlots[i].SetSlot(chosenTile.GetComponent<TileObject>().GetTileData().TileCost, chosenTile);

                //이미지 비율을 맞추기 위한 코드입니다.
                //storeSlots[i].GetComponent<Image>().preserveAspect = true;
                storeSlots[i].GetComponent<Image>().SetNativeSize();
            }
        }
    }

    public void ResetSlotBtn()
    {
        SoundManager.Instance.UISoundClip("RerollActivate");
        if(GoldManager.Instance.UseCurrentGold(1))
        {
            SetupStoreSlots();
            _rerollNum++;
            if (_rerollNum >= 10)
            {
                SteamAchievement.Achieve("ACH_BLD_REROLL");
            }
        }
    }

    /// <summary>
    /// 타일들을 리소스 파일에서 자동으로 로드해서 희귀도 따라서 분류하는 스크립트입니다
    /// </summary>
    private void SetStoreTileList()
    {
        _storeTiles.Clear();
        _storeTiles.Add(JournalSlotManager.Instance.StoreTiles[0].ToList());
        _storeTiles.Add(JournalSlotManager.Instance.StoreTiles[1].ToList());
        _storeTiles.Add(JournalSlotManager.Instance.StoreTiles[2].ToList());
        _storeTiles.Add(JournalSlotManager.Instance.StoreTiles[3].ToList());
        _storeTiles.Add(JournalSlotManager.Instance.StoreTiles[4].ToList());
        _firstStoreTiles = JournalSlotManager.Instance.FirstStoreTiles.ToList();
        _safePrefab = Resources.Load<GameObject>("Prefabs/Tiles/WeaponTile/StaffTile");

    }

    /// <summary>
    /// 입력한 타일 데이터가 현재 상점에 등장해도 되는 타일인지 판단합니다. 
    /// </summary>
    /// <param name="tile">조건을 판단할 타일입니다</param>
    /// <param name="list">내가 조건을 판단할 타일을 뽑은 리스트입니다.</param>
    /// <returns></returns>
    private GameObject CheckTileCondition(GameObject tile, TileGrade grade)
    {
        _safeInt++;
        if (_safeInt > 10)
        {
            Debug.LogError("너무 많이 조건에 맞는 타일을 상점에 생성하려는 시도가 반복됨!");
            _safeInt = 0;
            return _safePrefab;
        }
        TileObject tileObject = tile.GetComponent<TileObject>();
        bool isAvailable = true;
        //필요한 타일이 있는지 검사합니다.
        if(tileObject.GetTileData().RequiredTile != null)
        {
            if(!GridManager.Instance.PlacedTileList.Contains(tileObject.GetTileData().RequiredTile.tileName))
            {
                isAvailable = false;
            }

        }

        //겹치면 안되는 타일이 있는지 검사합니다.
        if(tileObject.GetTileData().RejectTileList.Count > 0)
        {
            foreach(TileData tileData in tileObject.GetTileData().RejectTileList)
            {
                if (GridManager.Instance.PlacedTileList.Contains(tileData.tileName) || appeardTileNameList.Contains(tileData.tileName))
                {
                    isAvailable = false;
                }

            }    
            
        }

        if(isAvailable)
        {
            _safeInt = 0;
            return tile;
        }
        else
        {
            //만족 안했으므로 다시 돌립니다.
            //조건을 만족 안하는 타일을 리스트에서 제거합니다.
            if (_storeTiles[(int)grade].Contains(tile))
            {
                _storeTiles[(int)grade].Remove(tile);
            }
            if (_storeTiles[(int)grade].Count == 0)
            {
                RefillList(grade);
            }
            int randomIndex = Random.Range(0, _storeTiles[(int)grade].Count);
            GameObject chosenTile = _storeTiles[(int)grade][randomIndex];
            return CheckTileCondition(chosenTile, grade);
        }

    }

    private void RefillList(TileGrade grade)
    {
        List<GameObject> newList;
        newList = JournalSlotManager.Instance.StoreTiles[(int)grade].ToList();

        _storeTiles[(int)grade] = newList;
    }

    
   
}
