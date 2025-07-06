using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoolDownEffect : MonoBehaviour
{
    [SerializeField]
    private float coolDownPoint = 0;
    private SpriteRenderer sr;
    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        EventBus.SubscribeGameStart(SetPosition);
        EventBus.SubscribeBossDeath(Init);
    }

    public void Init()
    {
        StopAllCoroutines();
        sr.size = new Vector2(1, 0);
    }
    public void StartCoolDown(float coolDownTime)
    {
        sr.size = new Vector2(1, 0);
        coolDownPoint = 0;
        StartCoroutine(CoolDownCoroutine(coolDownTime));
    }
    private IEnumerator CoolDownCoroutine(float coolDownTime)
    {
        while (coolDownPoint < 1)
        {
            coolDownPoint += Time.deltaTime / coolDownTime;
            //sr.color = new Color(1, 1, 1, 1 - coolDownPoint); // 알파값을 감소시켜 투명해짐
            sr.size = new Vector2(1, 1 * coolDownPoint);
            yield return null;
        }
        sr.size = new Vector2(1, 1);
    }
    
    /// <summary>
    /// 씬이 로드될 때 호출되어 타일 회전에 따른 CoolDownEffect의 위치를 설정합니다.
    /// </summary>
    public void SetPosition()
    {
        if (!SceneLoader.IsInStage()) return;
        StopAllCoroutines();
        sr.size = new Vector2(1, 0);
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
        EventBus.UnsubscribeGameStart(SetPosition);
        EventBus.UnsubscribeBossDeath(Init);
    }
}
