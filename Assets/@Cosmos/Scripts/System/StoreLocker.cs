using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoreLocker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private StoreSlot storeSlot;

    private GameObject descriptionText;

    private Sprite[] sprites;

    private Image image;

    private void Awake()
    {
        storeSlot = transform.parent.GetChild(0).GetComponent<StoreSlot>();
        GetComponent<Button>().onClick.AddListener(OnClick);
        descriptionText = transform.parent.GetChild(3).gameObject;
        sprites = Resources.LoadAll<Sprite>("Arts/BuildingScene/자물쇠-Sheet");
        image = GetComponent<Image>();
        storeSlot._onPurchase += OnPurchase;
    }

    private void Start()
    {
        if (StoreLockManager.Instance.GetStoreLocks(storeSlot.SlotNum) != null)
        {
            image.sprite = sprites[1];
        }
        else
        {
            image.sprite = sprites[0];
        }
    }

    private void OnClick()
    {
        if (storeSlot.GetObject() != null && !storeSlot.IsPurchased)
        {
            if (StoreLockManager.Instance.GetStoreLocks(storeSlot.SlotNum) == null)
            {
                StoreLockManager.Instance.AssignStoreLock(storeSlot.SlotNum, storeSlot.GetObject());
                image.sprite = sprites[1];
                
            }
            else
            {
                StoreLockManager.Instance.RemoveStoreLock(storeSlot.SlotNum);
                image.sprite = sprites[0];
            }

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionText.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionText.SetActive(false);
    }

    private void OnPurchase()
    {
        image.sprite = sprites[0];
    }

    private void OnDestroy()
    {
        if(storeSlot != null)
        {
            storeSlot._onPurchase -= OnPurchase;
        }
        
    }
}
