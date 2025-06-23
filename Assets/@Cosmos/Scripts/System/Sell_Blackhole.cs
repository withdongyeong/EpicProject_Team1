using TMPro;
using UnityEngine;

public class Sell_Blackhole : MonoBehaviour
{
    Collider2D _collider;
    TextMeshProUGUI _sellText;
    InfoPanel _tileInfo;
    SpriteRenderer _sr;
    Animator _animator;
    [SerializeField] Color _pointColor;

    private void Awake()
    {
        DragManager.Instance.AssignSell(this);
        _collider = GetComponent<Collider2D>();
        _sellText = transform.GetComponentInChildren<TextMeshProUGUI>();
        _sellText.gameObject.SetActive(false);
        _tileInfo = FindAnyObjectByType<InfoPanel>(FindObjectsInactive.Include);
        if (_tileInfo == null)
        {
            Debug.LogError("InfoPanel 없음.");
        }
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _animator.speed = 0.6f;
    }

    private void Update()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (_collider.OverlapPoint(mouseWorldPos))
        {
            _sr.color = _pointColor;
            SoundManager.Instance.BlackHoleReady();
            _animator.speed = 1.3f;
        }
        else
        {
            _sr.color = Color.white;
            SoundManager.Instance.BlackHoleNotReady();
            _animator.speed = 0.6f;
        }
    }


    public bool CheckSell(TileObject tile)
    {
        if(_collider.bounds.Contains(tile.transform.position))
        {
            SoundManager.Instance.UISoundClip("BlackHoleStartActivate");
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ActivateSellText(int price)
    {
        _sellText.gameObject.SetActive(true);
        _sellText.text = $"여기에 놓아 {price}G에 판매";
    }

    public void DisableSellText()
    {
        _sellText.gameObject.SetActive(false);
        _tileInfo.Hide();
    }    
}
