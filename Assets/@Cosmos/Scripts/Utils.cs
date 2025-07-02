using System;
using System.Collections.Generic;
using UnityEngine;


public enum GameState
{
    MainMenu,
    Title,
    Building,
    StageSelect,
    Playing,
    Victory,
    Defeat
}

public enum SkillState
{
    Charging, // 충전 중
    Ready,    // 준비 완료
    Activated // 발동 중
}

public enum TileGrade
{
    Normal,   // 일반 타일
    Rare,     // 희귀 타일
    Epic,     // 에픽 타일
    Legendary, // 전설 타일
    Myth //신화 타일
}

public enum TileCategory
{
    Weapon,      // 무기
    MagicCircle, // 마법진
    Armor,       // 방어구
    Consumable,  // 소모품
    Trinket,   // 장신구
    Summon,     // 소환수
    None, //없음
}

public enum TileType
{
    Attack,
    Protection,
    Shield,
    Heal,
    ManaHeal,
    FireBall,
    Sword,
    Totem
}

public enum TileTag
{
    Weapon,
    MagicCircle,
    Summon,
    Armor,
    Trinket,
    Potion,
    Fire,
    Ice,
    Sword,
    Totem,
}

[Serializable]
public class TileInfo
{
    private string tileName;
    private string description;
    private Sprite tileSprite;
    private TileGrade tileGrade;
    private int tileCost;
    private float tileCoolTime;
    private string tileData;
    private TileCategory tileCategory;
    private TileData requiredTile;
    private TileData rejectTile;

    public string TileName => tileName;
    public string Description => description;
    public Sprite TileSprite => tileSprite;
    public TileGrade TileGrade => tileGrade;
    public int TileCost => tileCost;
    public float TileCoolTime => tileCoolTime;
    public TileCategory TileCategory => tileCategory;
    public TileData RequiredTile => requiredTile;
    public TileData RejectTile => rejectTile;

    public TileInfo(TileData data)
    {
        tileName = data.tileName;
        description = data.description;
        tileSprite = data.tileSprite;
        tileGrade = data.tileGrade;
        tileCost = data.tileCost;
        tileCoolTime = data.tileCoolTime;
        tileCategory = data.tileCategory;
        requiredTile = data.requiredTile;
        rejectTile = data.rejectTile;
    }
}

[Serializable] public class ShopChanceClass
{
    public float shop_NormalChance; //노말 등급의 확률입니다
    public float shop_RareChance; //레어 등급의 확률입니다
    public float shop_EpicChance; //에픽 등급의 확률입니다
    public float shop_LegendaryChance; //레전더리 등급의 확률입니다
}

/// <summary>
/// 버프될 수 있는 타일의 스탯을 의미합니다. 만약 버프시키고 싶은 타일의 스탯이 있다면, 여기에 넣으면 됩니다.
/// </summary>
public enum BuffableTileStat
{
    //쿨타임
    CoolTime,
    //데미지
    Damage,
    //힐량
    HealAmount,
    //마법진의 경우, 마법진의 효과를 의미합니다.
    MagicCircle, 
}

/// <summary>
/// 인접 효과에 의한 버프를 담는 클래스입니다.
/// </summary>
[Serializable] public class StarBuff
{

    /// <summary>
    /// 전투 시작시 발동시킬 함수들을 담을 액션입니다.
    /// </summary>
    private Action<SkillBase> action_OnGameStart;

    /// <summary>
    /// 전투 시작시 발동시킬 함수로 등록하는 메서드입니다.
    /// </summary>
    /// <param name="act">이 함수를 전투 시작시 실행시킵니다.</param>
    public void RegisterGameStartAction(Action<SkillBase> act)
    {
        action_OnGameStart += act;
    }

    /// <summary>
    /// 전투 시작시 발동시킬 함수를 등록 해제하는 메서드입니다.
    /// </summary>
    /// <param name="act"></param>
    public void UnregisterGameStartAction(Action<SkillBase> act)
    {
        action_OnGameStart -= act;
    }

    /// <summary>
    /// 전투 시작시 발동시킬 함수들을 담을 액션입니다.
    /// </summary>
    public Action<SkillBase> Action_OnGameStart => action_OnGameStart;

    /// <summary>
    /// 타일이 발동되었을때 발동시킬 함수들을 담을 액션입니다.
    /// </summary>
    private Action<SkillBase> action_OnActivate;

    /// <summary>
    /// 타일 발동시 발동시킬 함수로 등록하는 메서드입니다.
    /// </summary>
    /// <param name="act">이 함수를 타일 발동시 발동시킵니다.</param>
    public void RegisterActivateAction(Action<SkillBase> act)
    {
        action_OnActivate += act;
    }

    /// <summary>
    /// 타일 발동시 발동시킬 함수를 등록 해제하는 메서드입니다.
    /// </summary>
    /// <param name="act"></param>
    public void UnregisterActivateAction(Action<SkillBase> act)
    {
        action_OnActivate -= act;
    }

    /// <summary>
    /// 타일이 발동되었을때 발동시킬 함수들을 담을 액션 리스트입니다.
    /// </summary>
    public Action<SkillBase> Action_OnActivate => action_OnActivate;

}

/// <summary>
/// 버프되는 스탯, 버프되는 양을 담은 클래스입니다.
/// </summary>
[Serializable] public class TileBuffData
{

    /// <summary>
    /// 버프를 받을 스탯입니다.
    /// </summary>
    private BuffableTileStat _tileStat;

    /// <summary>
    /// 버프를 받을 스탯에 접근합니다.
    /// </summary>
    public BuffableTileStat TileStat => _tileStat;


    /// <summary>
    /// 버프를 받는 양입니다.
    /// </summary>
    private float _value;

    /// <summary>
    /// 버프를 받는 양에 접근합니다.
    /// </summary>
    public float Value => _value;

    /// <summary>
    ///  버프되는 조건, 버프되는 스탯, 버프되는 양을 담은 클래스입니다.
    /// </summary>
    /// <param name="condition">버프되는 조건입니다</param>
    /// <param name="value">버프되는 양입니다.</param>
    public TileBuffData(BuffableTileStat tileStat, float value)
    {
        _tileStat = tileStat;
        _value = value;
    }
}
