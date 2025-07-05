using UnityEngine;
using UnityEngine.EventSystems;

public class StoreLocker : MonoBehaviour, IPointerClickHandler
{
    private StoreSlot storeSlot;

    
   

    private void Awake()
    {
        storeSlot = GetComponent<StoreSlot>();
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if(storeSlot.GetObject() != null && !storeSlot.IsPurchased)
            {
                if(StoreLockManager.Instance.GetStoreLocks(storeSlot.SlotNum) == null)
                {
                    StoreLockManager.Instance.AssignStoreLock(storeSlot.SlotNum, storeSlot.GetObject());
                    storeSlot.SetColor(Color.red);
                }
                else
                {
                    StoreLockManager.Instance.RemoveStoreLock(storeSlot.SlotNum);
                    storeSlot.SetColor(Color.white);
                }
                
            }
        }
    }
}
