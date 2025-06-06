using UnityEngine;

[CreateAssetMenu(fileName = "RarityGroup", menuName = "Shop/RarityGroup")]
public class StoreTileAppearList : ScriptableObject
{
    [SerializeField] private TileGrade gradeOfTiles;

    /// <summary>
    /// 타일들의 등급을 알려주는 enum입니다.
    /// </summary>
    public TileGrade GradeOfTiles => gradeOfTiles;

    
    [SerializeField] private GameObject[] tileList;

    /// <summary>
    /// 상점에 넣어줄 같은 등급의 타일들 리스트입니다.
    /// </summary>
    public GameObject[] TileList => tileList;
}
