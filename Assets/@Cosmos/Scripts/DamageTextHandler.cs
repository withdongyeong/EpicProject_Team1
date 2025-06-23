using System;
using UnityEngine;
using DamageNumbersPro;

public class DamageTextHandler : MonoBehaviour
{
    //Assign prefab in inspector.(임시)
    public DamageNumber numberPrefab;
    public RectTransform rectParent;
    private BaseBoss target;
    private void Awake()
    {
        EventBus.SubscribeGameStart(SetTarget);
    }

    private void SetTarget()
    {
        target = FindAnyObjectByType<BaseBoss>();
    }


    public void SpawnDamageText(int damage)
    {
        
        
        Vector2 screenPos = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up * 3f);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, null,
            out Vector2 localPoint);
        DamageNumber damageNumber = numberPrefab.SpawnGUI(rectParent, localPoint, damage);
        damageNumber.followedTarget = target.transform;
        // You can customize the damage number here if needed
        // e.g., damageNumber.SetColor(Color.red);
    
    }



    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(SetTarget);
    }
}
