using UnityEngine;

public class WeaponFade : MonoBehaviour
{
    public float fadeDuration = 0.5f;
    public bool fadeInOnStart = false;

    private SpriteRenderer sr;
    private float timer = 0f;
    private bool fadingIn = false;
    private bool fadingOut = false;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>(); // 자식에 있을 수도 있음
        if (sr == null) sr = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (fadeInOnStart) StartFadeIn();
    }

    public void StartFadeIn()
    {
        fadingIn = true;
        fadingOut = false;
        timer = 0f;
        SetAlpha(0f);
    }

    public void StartFadeOutAndDestroy()
    {
        fadingOut = true;
        fadingIn = false;
        timer = 0f;
    }

    void Update()
    {
        if (fadingIn)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / fadeDuration);
            SetAlpha(t);
            if (t >= 1f) fadingIn = false;
        }
        else if (fadingOut)
        {
            timer += Time.deltaTime;
            float t = 1f - Mathf.Clamp01(timer / fadeDuration);
            SetAlpha(t);
            if (t <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    void SetAlpha(float alpha)
    {
        if (sr != null)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}