using UnityEngine;

public class BossHitEffect : MonoBehaviour
{
    public AudioClip HitSound;

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
