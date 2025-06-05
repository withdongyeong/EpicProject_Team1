using System;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField]
    private Sprite sprite;


    private void Awake()
    {
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
    }


    public Sprite GetSprite()
    {
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
        if (sprite == null)
        {
            Debug.LogError("Sprite is not assigned in ItemObject.");
            return null;
        }
        return sprite;
    }
}
