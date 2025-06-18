using System;
using UnityEngine;


public enum GameState
{
    MainMenu,
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
    Legendary // 전설 타일
}

public enum TileCategory
{
    Weapon,      // 무기
    MagicCircle, // 마법진
    Armor,       // 방어구
    Consumable,  // 소모품
    Accessory,   // 장신구
    Summon,     // 소환수
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
    [SerializeField] private string tileName;
    [SerializeField] private string description;
    [SerializeField] private Sprite tileSprite;
    [SerializeField] private TileGrade tileGrade;
    [SerializeField] private int tileCost;
    [SerializeField] private string tileData;
    [SerializeField] private TileCategory tileCategory;

    public string TileName => tileName;
    public string Description => description;
    public Sprite TileSprite => tileSprite;
    public TileGrade TileGrade => tileGrade;
    public int TileCost => tileCost;
    public TileCategory TileCategory => tileCategory;

    public TileInfo(TileData data)
    {
        tileName = data.tileName;
        description = data.description;
        tileSprite = data.tileSprite;
        tileGrade = data.tileGrade;
        tileCost = data.tileCost;
        tileCategory = data.tileCategory;
    }
}

/// <summary>
/// 인접 효과에 의한 버프를 담는 클래스입니다.
/// </summary>
public class StarBuff
{

}
