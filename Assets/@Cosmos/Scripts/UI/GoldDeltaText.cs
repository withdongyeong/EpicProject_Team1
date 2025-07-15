using System.Collections;
using TMPro;
using UnityEngine;

public class GoldDeltaText : MonoBehaviour
{
    private int _currentGold;

    private TextMeshProUGUI _goldText;

    private float _duration = 2f;

    private Coroutine _coroutine;

    private void Awake()
    {
        _currentGold = GoldManager.Instance.CurrentGold;
        _goldText = GetComponent<TextMeshProUGUI>();
        EventBus.SubscribeGoldChanged(ChangeText);
    }

    private void ChangeText(int gold)
    {
        int delta = gold - _currentGold;
        if(delta != 0)
        {
            if(_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }    
            if (delta > 0)
            {
                _goldText.color = Color.green;
                _goldText.text = $"+{delta} G";
            }
            else
            {
                _goldText.color = Color.red;
                _goldText.text = $"{delta} G";
            }
            _currentGold = gold;
            _coroutine = StartCoroutine(FadeText());
        }
        
    }

    IEnumerator FadeText()
    {
        Color color = _goldText.color;
        float t = 0;

        while (t < _duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / _duration);
            _goldText.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
    }

    private void OnDestroy()
    {
        EventBus.UnsubscribeGoldChanged(ChangeText);
    }
}
