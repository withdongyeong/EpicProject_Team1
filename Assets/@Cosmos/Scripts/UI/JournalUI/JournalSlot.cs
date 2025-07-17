using UnityEngine;
using UnityEngine.UI;

public class JournalSlot : MonoBehaviour
{
    protected GameObject objectPrefab;
    protected Image image;
    protected HoverTileInfo hoverTileInfo;
    [SerializeField]
    protected Image backgroundImage;

    protected virtual void Awake()
    {
        image = GetComponent<Image>();
        backgroundImage = transform.parent.GetComponent<Image>();
        hoverTileInfo = GetComponent<HoverTileInfo>();
    }

    public virtual GameObject GetObject()
    {
        return objectPrefab; // 오브젝트 반환
    }

    public virtual void SetSlot(GameObject prefab)
    {
        //Debug.Log(cost);
        this.objectPrefab = prefab;
        image.color = Color.white; // 초기 색상 설정
        image.sprite = prefab.GetComponent<TileObject>().GetTileSprite(); // 아이템 오브젝트의 스프라이트 설정
        //infoUI.SetTileObject(prefab.GetComponent<TileObject>()); // InfoUI에 TileObject 설정
        hoverTileInfo.SetTileObject(prefab.GetComponent<TileObject>());
        image.SetNativeSize();
        backgroundImage.GetComponent<RectTransform>().sizeDelta = image.rectTransform.sizeDelta; // 배경 이미지 크기 조정

    }
}
