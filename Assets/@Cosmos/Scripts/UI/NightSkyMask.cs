using UnityEngine;
using UnityEngine.UI;

public class NightSkyMask : MonoBehaviour
{
    private SpriteRenderer targetRenderer;
    private SpriteMask spriteMask;

    private void Awake()
    {
        targetRenderer = GetComponent<SpriteRenderer>();
        spriteMask = GetComponent<SpriteMask>();
    }

    private void LateUpdate()
    {
        if(spriteMask.sprite != targetRenderer.sprite)
        {
            spriteMask.sprite = targetRenderer.sprite;
        }
    }
}
