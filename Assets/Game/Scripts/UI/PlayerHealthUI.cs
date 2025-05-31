using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾� ü�� UI ǥ��
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    private Slider _healthSlider;
    private PlayerHealth _playerHealth;

    void Start()
    {
        _healthSlider = GetComponent<Slider>();
        _playerHealth = FindAnyObjectByType<PlayerHealth>();
    
        if (_playerHealth != null)
        {
            // �̺�Ʈ ����
            _playerHealth.OnHealthChanged += UpdateHealthUI;
            
            // �����̴� �ʱ� ����
            _healthSlider.maxValue = _playerHealth.MaxHealth;
            _healthSlider.value = _playerHealth.CurrentHealth;
        }
        else
        {
            Debug.LogError("PlayerHealth ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    /// <summary>
    /// ü�� UI ������Ʈ
    /// </summary>
    private void UpdateHealthUI(int health)
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = health;
        }
    }
    
    private void OnDestroy()
    {
        if (_playerHealth != null)
        {
            _playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }
}