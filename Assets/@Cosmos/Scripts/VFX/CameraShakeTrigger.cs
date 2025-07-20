using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeTrigger : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float intensity = 1f)
    {
        if (impulseSource != null)
            impulseSource.GenerateImpulse(intensity);
    }
}