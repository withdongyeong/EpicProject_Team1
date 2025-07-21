using UnityEngine;
using UnityEngine.UI;

public class RetryButton : MonoBehaviour
{
    private Button _button;

    void Start()
    {
        _button = GetComponent<Button>();

        if (_button == null)
        {
            Debug.LogWarning("RetryButton: Button 컴포넌트가 없음.");
            return;
        }

        if (LifeManager.Instance != null && LifeManager.Instance.Life < 0)
        {
            _button.interactable = false;
        }
    }
}