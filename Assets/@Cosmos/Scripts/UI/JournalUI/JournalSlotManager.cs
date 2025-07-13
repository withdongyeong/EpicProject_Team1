using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JournalSlotManager : Singleton<JournalSlotManager>
{
    private int _unlockLevel;

    private GameObject _journalSlotPrefab;

    private GameObject _safePrefab;


    private Transform _slotParent; //슬롯의 부모, 그러니까 StoreSlotController가 붙은 쯤의 위치입니다.

    //이 밑은 희귀도에 따라서 분류된 리스트입니다
    private List<GameObject> _normalStoreTiles = new();
    private List<GameObject> _rareStoreTiles = new();
    private List<GameObject> _epicStoreTiles = new();
    private List<GameObject> _legendaryStoreTiles = new();
    private List<GameObject> _mythicStoreTiles = new();
    private List<GameObject> _lockedStoreTiles = new();

    //리스트에 접근하는 프로퍼티입니다
    public List<GameObject> NormalStoreTiles => _normalStoreTiles;
    public List<GameObject> RareStoreTiles => _rareStoreTiles;
    public List<GameObject> EpicStoreTiles => _epicStoreTiles;
    public List<GameObject> LegendaryStoreTiles => _legendaryStoreTiles;
    public List<GameObject> MythicStoreTiles => _mythicStoreTiles;



    protected override void Awake()
    {
        base.Awake();
        _journalSlotPrefab = Resources.Load<GameObject>("Prefabs/UI/Journal/JournalSlot");
        _slotParent = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);
        EventBus.SubscribeSceneLoaded(CloseJournalOnSceneChange);
    }




    private void Start()
    {
        //DontDestroyOnLoad(Instantiate(_eventSystem));
        SetStoreTileList();
        InstantiateAllJournalSlots();
        gameObject.SetActive(false);
    }

    public void SetStoreTileList()
    {
        //현재 해금된 애들. 이 번호보다 작거나 같으면 해금된거에요
        _unlockLevel = GameManager.Instance.CurrentUnlockLevel;

        List<GameObject> allTilePrefabs = new();

        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/WeaponTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/BookTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/SummonTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/EquipTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/PotionTile"));
        allTilePrefabs.AddRange(Resources.LoadAll<GameObject>("Prefabs/Tiles/TrinketTile"));
        _safePrefab = Resources.Load<GameObject>("Prefabs/Tiles/WeaponTile/GuideStaffTile");

        foreach (GameObject tilePrefab in allTilePrefabs)
        {
            TileInfo tileInfo = tilePrefab.GetComponent<TileObject>().GetTileData();
            if (tileInfo.UnlockInt <= _unlockLevel)
            {
                TileGrade grade = tileInfo.TileGrade;
                if (grade == TileGrade.Normal)
                {
                    if (tileInfo.TileName != "GuideStaffTile")
                    {
                        _normalStoreTiles.Add(tilePrefab);
                    }

                }
                else if (grade == TileGrade.Rare)
                {
                    _rareStoreTiles.Add(tilePrefab);
                }
                else if (grade == TileGrade.Epic)
                {
                    _epicStoreTiles.Add(tilePrefab);
                }
                else if (grade == TileGrade.Legendary)
                {
                    _legendaryStoreTiles.Add(tilePrefab);
                }
                else
                {
                    _mythicStoreTiles.Add(tilePrefab);
                }
            }
            else
            {
                _lockedStoreTiles.Add(tilePrefab);
            }

        }
    }

    private void InstantiateAllJournalSlots()
    {
        //먼저 저널 슬롯 싹 비웁니다
        for (int i = _slotParent.childCount - 1; i >= 0; i--)
        {
            Destroy(_slotParent.GetChild(i).gameObject);
        }

        InstantiateJournalSlotList(_normalStoreTiles);
        InstantiateJournalSlotList(_rareStoreTiles);
        InstantiateJournalSlotList(_epicStoreTiles);
        InstantiateJournalSlotList(_legendaryStoreTiles);
        InstantiateJournalSlotList(_mythicStoreTiles);
    }

    private void InstantiateJournalSlotList(List<GameObject> objects)
    {
        foreach(GameObject tile in objects)
        {
            JournalSlot journalSlot = Instantiate(_journalSlotPrefab, _slotParent).GetComponentInChildren<JournalSlot>();
            journalSlot.SetSlot(tile);
            journalSlot.GetComponent<Image>().SetNativeSize();

        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseJournal();
        }
    }


    public void CloseJournal()
    {
        if (DragManager.Instance.GetCurrentDragObject() == null)
            gameObject.SetActive(false);    
    }

    public void CloseJournalOnSceneChange(Scene scene, LoadSceneMode mode)
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(CloseJournalOnSceneChange);
    }

}
