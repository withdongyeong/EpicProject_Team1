using System;
using UnityEngine;
using DamageNumbersPro;

public class DamageTextHandler : MonoBehaviour
{
    public bool isVer2 = false; // true면 UI로 표시, false면 월드 공간에 표시
    //Assign prefab in inspector.(임시)
    public DamageNumber numberPrefab;
    public DamageNumber numberPrefabUI;
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
        if (isVer2)
        {
            Transform targetPos = GameObject.Find("TestDamagePos").transform;
            Vector2 screenPos = Camera.main.WorldToScreenPoint(targetPos.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, null,
                out Vector2 localPoint);
            DamageNumber damageNumber = numberPrefabUI.SpawnGUI(rectParent, localPoint, damage);
            //damageNumber.followedTarget = target.transform;
        }
        else
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(target.transform.position + Vector3.up * 3f);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, null,
                out Vector2 localPoint);
            DamageNumber damageNumber = numberPrefab.SpawnGUI(rectParent, localPoint, damage);
            damageNumber.followedTarget = target.transform;
            // You can customize the damage number here if needed
            // e.g., damageNumber.SetColor(Color.red);
        }
    }



    private void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(SetTarget);
    }
}
