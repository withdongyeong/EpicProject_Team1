using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoolDownEffect : MonoBehaviour
{
    private int coolDownPoint = 0;
    private SpriteRenderer sr;
    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        EventBus.SubscribeSceneLoaded(SetPosition);
    }

    
    
    /// <summary>
    /// 씬이 로드될 때 호출되어 타일 회전에 따른 CoolDownEffect의 위치를 설정합니다.
    /// </summary>
    public void SetPosition(Scene scene , LoadSceneMode mode)
    {
        if (!SceneLoader.IsInStage()) return;
        if (transform.rotation.eulerAngles.z == 0)
        {
            transform.localPosition = new Vector3(0, -0.5f, 0);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        if (transform.rotation.eulerAngles.z == 90)
        {
            transform.localPosition = new Vector3(-0.5f, 0, 0);
            transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        if (transform.rotation.eulerAngles.z == 180)
        {
            transform.localPosition = new Vector3(0, 0.5f, 0);
            transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
        if (transform.rotation.eulerAngles.z == 270)
        {
            transform.localPosition = new Vector3(0.5f, 0, 0);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        
    }

    public void OnDestroy()
    {
        EventBus.UnsubscribeSceneLoaded(SetPosition);
    }
}
