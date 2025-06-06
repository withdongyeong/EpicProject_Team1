using System;
using UnityEngine;
using System.Collections.Generic;
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



[Serializable]
public class TileInfo
{
    [SerializeField] private string tileName;
    [SerializeField] private string description;
    [SerializeField] private Sprite tileSprite;
    [SerializeField] private TileGrade tileGrade;
    [SerializeField] private int tileCost;

    public string TileName => tileName;
    public string Description => description;
    public Sprite TileSprite => tileSprite;
    public TileGrade TileGrade => tileGrade;
    public int TileCost => tileCost;

    public TileInfo(TileData data)
    {
        tileName = data.tileName;
        description = data.description;
        tileSprite = data.tileSprite;
        tileGrade = data.tileGrade;
        tileCost = data.tileCost;
    }
}
