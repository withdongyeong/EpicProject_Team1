using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스프라이트 상태 제어: Idle, Damaged, Death
/// 자식 오브젝트에 붙여서 상태 전환 시 스프라이트 교체
/// </summary>
public class PlayerSpriteStateController : MonoBehaviour
{
    public Sprite idleSprite;
    public Sprite damagedSprite;
    public Sprite deathSprite;

    private Image _image;
    private Sprite _current;

    private void Awake()
    {
        _image = GetComponent<Image>();
        if (_image == null)
            Debug.LogError("PlayerSpriteStateController: Image 컴포넌트가 필요합니다.");
    }

    public void SetIdle()
    {
        if (_image != null && idleSprite != null)
        {
            _image.sprite = idleSprite;
            _current = idleSprite;
        }
    }

    public void SetDamaged()
    {
        if (_image != null && damagedSprite != null)
        {
            _image.sprite = damagedSprite;
            _current = damagedSprite;
        }
    }

    public void SetDeath()
    {
        if (_image != null && deathSprite != null)
        {
            _image.sprite = deathSprite;
            _current = deathSprite;
        }
    }

    public Sprite GetCurrentSprite()
    {
        return _current;
    }
}