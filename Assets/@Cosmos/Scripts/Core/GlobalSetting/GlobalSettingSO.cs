using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossBalance
{
    public int maxHP;
    public int weakDamage;
    public int strongDamage;
}

[CreateAssetMenu(fileName = "New Setting", menuName = "GlobalSetting/Setting")]
public class GlobalSettingSO : ScriptableObject
{
    [Header("타일 기본 속성")]
    public float basicDamage;

    [Header("소환 기믹 데이터")]
    public Vector2 summon_Offset;

    [Header("토템")]
    public Vector2 totem_Offset;
    public Vector2 totem_ActivatePos;
    public int totem_HeadInt;

    [Header("상점 등급 확률표")]
    public List<ShopChanceClass> shop_ChanceList;

    [Header("언어")]
    public SynergyTranslationSO language_SynergyTranslationSO;

    [Header("보스 밸런스 (튜토리얼 + 스테이지 1~10)")]
    public List<BossBalance> bossBalanceList = new(); // 인덱스 0 = 튜토리얼, 1~10 = 스테이지 1~10

    [Header("처음에 등장시킬 타일5개의 데이터")]
    public List<TileData> shop_FirstTileDataList;
}