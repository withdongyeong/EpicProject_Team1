using UnityEngine;
using UnityEngine.UI;

public class LockedJournalSlot : JournalSlot
{
    protected HoverLockedTileInfo hoverLockedTileInfo;

    protected override void Awake()
    {
        image = GetComponent<Image>();
        backgroundImage = transform.parent.GetComponent<Image>();
        hoverLockedTileInfo = GetComponent<HoverLockedTileInfo>();
    }

    public override void SetSlot(GameObject prefab)
    {
        this.objectPrefab = prefab;
        image.color = Color.white; // 초기 색상 설정
        image.sprite = Resources.Load<Sprite>("Arts/Objects/question");
        TileObject tileObject = prefab.GetComponent<TileObject>();
        switch (tileObject.GetTileData().TileGrade)
        {
            case TileGrade.Normal:
                image.color = Color.white;
                break;
            case TileGrade.Rare:
                image.color = new Color(0.7f, 0.9f, 1f);
                break;
            case TileGrade.Epic:
                image.color = new Color(0.9f, 0.7f, 1f); // 보라
                break;
            case TileGrade.Legendary:
                image.color = new Color(1f, 0.7f, 0.6f);
                break;
            case TileGrade.Mythic:
                image.color = new Color(0.65f, 0.9f, 0.85f);
                break;
            default:
                image.color = Color.white;
                break;

        }
        string fileName = "Locked" + tileObject.GetTileData().TileGrade.ToString() + "Tile";
        string filePath = "TileData/" + fileName;
        TileData tileData = Resources.Load<TileData>(filePath);  
        hoverLockedTileInfo.SetTileObject(new TileInfo(tileData));
        image.SetNativeSize();
        backgroundImage.GetComponent<RectTransform>().sizeDelta = image.rectTransform.sizeDelta; // 배경 이미지 크기 조정
    }
}
