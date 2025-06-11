using TMPro;
using UnityEngine;

public class PlayerShieldUI : MonoBehaviour
{
    TextMeshProUGUI _text;
    PlayerShield _playerShield;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerShield = FindAnyObjectByType<PlayerShield>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_text != null && _playerShield != null)
        {
            _text.text = _playerShield.ShieldAmount.ToString();    
        }
    }
}
