using UnityEngine;

public class BossHitEffect : MonoBehaviour
{
    public AudioClip HitSound;

    private void Start()
    {
       SoundManager.Instance.DamageEffectStart(HitSound);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
