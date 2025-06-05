using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class StoreSlotController : MonoBehaviour
{
    public GameObject[] storeObjects;
    private StoreSlot[] storeSlots;
    
    private void Awake()
    {
        storeSlots = GetComponentsInChildren<StoreSlot>();
    }

    private void Start()
    {
        SetupStoreSlots();
    }


    private void SetupStoreSlots()
    {
        Debug.Log(storeSlots.Length);
        for (int i = 0; i < storeSlots.Length; i++)
        {
        
            int randomCost = Random.Range(10, 100);
            int randomIndex = Random.Range(0, storeObjects.Length);
            storeSlots[i].SetSlot(randomCost, storeObjects[randomIndex]);
        
        }
    }

    public void ResetSlotBtn()
    {
        SetupStoreSlots();
    }
}
