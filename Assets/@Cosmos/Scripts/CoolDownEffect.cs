using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoolDownEffect : MonoBehaviour
{
    [SerializeField]
    private float coolDownPoint = 0;
    private TileObject tileObject;
    private SpriteRenderer sr;

    [SerializeField]
    private Color yellowColor;
    
    [Header("발동 이펙트")]
    private GameObject activateEffectPrefab;
    private float effectDuration = 1f;
    
    public void Awake()
    {
        tileObject = transform.parent.GetComponent<Cell>().GetCombineCell().GetTileObject();
        sr = GetComponent<SpriteRenderer>();
        string hexColor = "#404040";
        // string hexColor = "#7F8263";
        ColorUtility.TryParseHtmlString(hexColor, out Color color);
        color.a = 0.5f;
        yellowColor = color;
        sr.color = yellowColor;
        EventBus.SubscribeGameStart(SetPosition);
        EventBus.SubscribeSceneLoaded(Init);
        
        // 발동 프리팹 할당
        activateEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ActivateEffect");
    }

    
    //초기화
    public void Init(Scene scene = default , LoadSceneMode mode = default)
    {
        StopAllCoroutines();
        sr.size = new Vector2(1, 0);
    }


    public void CompleteEffect()
    {
        StopAllCoroutines();
        StartCoroutine(CompleteEffectCoroutine());
    }

    private IEnumerator CompleteEffectCoroutine()
    {
        //노란색이 됐다가 빠르게 흰색으로 변함
        float duration = 0.3f;
        Color originalColor = yellowColor;
        Color targetColor = new Color(0.984f, 0.949f, 0.212f, 0.4f);
        float elapsedTime = 0f;
        
        // 준비 완료 이펙트
        StartCoroutine(SpawnEffect());
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            sr.color = Color.Lerp(originalColor, targetColor, t);
            yield return null;
        }

        duration = 0.3f;
        originalColor = new Color(1f, 1f, 0f, 0.1f);
        targetColor = new Color(1f, 1f, 0f, 0.0f);
        elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            sr.color = Color.Lerp(originalColor, targetColor, t);
            yield return null;
        }
    }

    /// <summary>
    /// 발동 이펙트
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnEffect()
    {
        if (activateEffectPrefab != null)
        {
            // 부모의 CombineCell을 찾아서 중심점 구하기
            CombineCell combineCell = GetComponentInParent<CombineCell>();
            Vector3 effectPosition = transform.position; // 기본값은 현재 위치
            
            if (combineCell != null)
            {
                // SkillBase와 동일한 방식으로 타일의 중심점 찾기
                Transform spriteTransform = combineCell.GetSprite().transform;
                if (spriteTransform != null)
                {
                    effectPosition = spriteTransform.position;
                }
            }
            
            GameObject effect = Instantiate(activateEffectPrefab, effectPosition, Quaternion.identity);
            Destroy(effect, effectDuration);
        }
        yield return null;
    }
    
    public void StartCoolDown(float coolDownTime)
    {
        sr.size = new Vector2(1, 0);
        coolDownPoint = 0;
        StartCoroutine(CoolDownCoroutine(coolDownTime));
    }
    private IEnumerator CoolDownCoroutine(float coolDownTime)
    {
        sr.color = yellowColor;
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
        Debug.Log("CoolDownEffect");
        if (!SceneLoader.IsInStage() && !SceneLoader.IsInBuilding()) return;
        StopAllCoroutines();
        sr.size = new Vector2(1, 0);
        Debug.Log("before :: " + tileObject.transform.rotation.eulerAngles + " " + transform.localRotation.eulerAngles);
        if (tileObject.transform.rotation.eulerAngles.z == 0)
        {
            transform.localPosition = new Vector3(0, -0.5f, 0);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (tileObject.transform.rotation.eulerAngles.z == 90)
        {
            transform.localPosition = new Vector3(-0.5f, 0, 0);
            transform.localRotation = Quaternion.Euler(0, 0, -90);
        }
        else if (tileObject.transform.rotation.eulerAngles.z == 180)
        {
            transform.localPosition = new Vector3(0, 0.5f, 0);
            transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
        else if (tileObject.transform.rotation.eulerAngles.z == 270)
        {
            transform.localPosition = new Vector3(0.5f, 0, 0);
            transform.localRotation = Quaternion.Euler(0, 0, 90);
        }
        Debug.Log("after :: " + transform.rotation.eulerAngles + " " + transform.localRotation.eulerAngles);
        
        
    }

    public void OnDestroy()
    {
        EventBus.UnsubscribeGameStart(SetPosition);
        EventBus.UnsubscribeSceneLoaded(Init);
    }
}