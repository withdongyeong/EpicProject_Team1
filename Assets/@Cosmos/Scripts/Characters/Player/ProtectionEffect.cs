using UnityEngine;
using System.Collections.Generic;

public class ProtectionEffect : MonoBehaviour
{
    [SerializeField] private GameObject protectionImagePrefab; // 보호막 이미지 프리팹
    [SerializeField] private float moveDuration = 1.0f; // 객체 이동 시간
    [SerializeField] private float minCurveHeight = 0.5f; // 최소 곡선 높이
    [SerializeField] private float maxCurveHeight = 2.0f; // 최대 곡선 높이
    [SerializeField] private float minCurveOffset = -0.5f; // 곡선 좌우 오프셋 최소값
    [SerializeField] private float maxCurveOffset = 0.5f; // 곡선 좌우 오프셋 최대값
    [SerializeField] private float minScale = 0.5f; // 최소 스케일
    [SerializeField] private float maxScale = 1.5f; // 최대 스케일
    [SerializeField] private int defaultObjectCount = 3; // 초기 생성 객체 수
    [SerializeField] private float spawnInterval = 0.2f; // 새 객체 생성 간격 (초)

    private Dictionary<int, (List<(GameObject instance, Coroutine coroutine)> objects, GameObject startObject)> activeEffects = new Dictionary<int, (List<(GameObject, Coroutine)>, GameObject)>();
    private Dictionary<int, Coroutine> spawnCoroutines = new Dictionary<int, Coroutine>();
    private int effectIdCounter = 0;

    // 효과 시작 함수 (이펙트 ID 반환)
    public int StartProtectionEffect(GameObject startObject, Vector3 targetPos)
    {
        if (protectionImagePrefab == null || startObject == null) return -1;

        int effectId = effectIdCounter++;
        List<(GameObject instance, Coroutine coroutine)> effectObjects = new List<(GameObject, Coroutine)>();

        // 초기 객체 생성
        for (int i = 0; i < defaultObjectCount; i++)
        {
            GameObject protectionInstance = CreateProtectionObject(startObject.transform.position);
            Coroutine coroutine = StartCoroutine(MoveProtection(effectId, protectionInstance, startObject.transform.position, targetPos));
            effectObjects.Add((protectionInstance, coroutine));
        }

        // 활성 이펙트 목록에 추가
        activeEffects[effectId] = (effectObjects, startObject);
        // 주기적 객체 생성 코루틴 시작
        spawnCoroutines[effectId] = StartCoroutine(SpawnProtectionObjects(effectId, startObject, targetPos));

        return effectId;
    }

    // 보호막 객체 생성
    private GameObject CreateProtectionObject(Vector3 startPos)
    {
        GameObject protectionInstance = Instantiate(protectionImagePrefab, startPos, Quaternion.identity);
        float randomScale = Random.Range(minScale, maxScale);
        protectionInstance.transform.localScale = new Vector3(randomScale, randomScale, 1f);
        return protectionInstance;
    }

    // 특정 이펙트 중단 함수
    public void StopProtectionEffect(int effectId)
    {
        // 주기적 생성 코루틴 중단
        if (spawnCoroutines.ContainsKey(effectId))
        {
            if (spawnCoroutines[effectId] != null)
            {
                StopCoroutine(spawnCoroutines[effectId]);
            }
            spawnCoroutines.Remove(effectId);
        }

        // 활성 객체 및 코루틴 중단
        if (activeEffects.ContainsKey(effectId))
        {
            foreach (var effect in activeEffects[effectId].objects)
            {
                if (effect.coroutine != null)
                {
                    StopCoroutine(effect.coroutine);
                }
                if (effect.instance != null)
                {
                    Destroy(effect.instance);
                }
            }
            activeEffects.Remove(effectId);
        }
    }

    // 모든 이펙트 중단 함수
    public void StopAllProtectionEffects()
    {
        // 모든 주기적 생성 코루틴 중단
        foreach (var coroutine in spawnCoroutines.Values)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
        spawnCoroutines.Clear();

        // 모든 활성 객체 및 코루틴 중단
        foreach (var effect in activeEffects.Values)
        {
            foreach (var obj in effect.objects)
            {
                if (obj.coroutine != null)
                {
                    StopCoroutine(obj.coroutine);
                }
                if (obj.instance != null)
                {
                    Destroy(obj.instance);
                }
            }
        }
        activeEffects.Clear();
    }

    // 주기적으로 객체 생성
    private System.Collections.IEnumerator SpawnProtectionObjects(int effectId, GameObject startObject, Vector3 targetPos)
    {
        while (true)
        {
            if (activeEffects.ContainsKey(effectId) && startObject != null)
            {
                GameObject protectionInstance = CreateProtectionObject(startObject.transform.position);
                Coroutine coroutine = StartCoroutine(MoveProtection(effectId, protectionInstance, startObject.transform.position, targetPos));
                activeEffects[effectId].objects.Add((protectionInstance, coroutine));
            }
            else
            {
                yield break; // 이펙트가 중단된 경우 코루틴 종료
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private System.Collections.IEnumerator MoveProtection(int effectId, GameObject protectionInstance, Vector3 startPos, Vector3 targetPos)
    {
        float elapsedTime = 0f;
        // 랜덤한 곡선 높이와 오프셋 생성
        float curveHeight = Random.Range(minCurveHeight, maxCurveHeight);
        float curveOffset = Random.Range(minCurveOffset, maxCurveOffset);

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;

            // 베지어 곡선을 사용한 부드러운 이동
            Vector3 linearPos = Vector3.Lerp(startPos, targetPos, t);
            Vector3 controlPoint = (startPos + targetPos) / 2 + Vector3.up * curveHeight + Vector3.right * curveOffset;
            Vector3 newPos = CalculateQuadraticBezierPoint(startPos, controlPoint, targetPos, t);

            if (protectionInstance != null)
            {
                protectionInstance.transform.position = newPos;
            }
            else
            {
                yield break; // 이펙트가 중단된 경우 코루틴 종료
            }
            yield return null;
        }

        // 목표 위치 도달 후 객체 제거
        if (protectionInstance != null)
        {
            protectionInstance.transform.position = targetPos;
            Destroy(protectionInstance, 0.1f);
        }

        // 이펙트 리스트에서 객체 제거
        if (activeEffects.ContainsKey(effectId))
        {
            activeEffects[effectId].objects.RemoveAll(effect => effect.instance == protectionInstance);
            // 이펙트 내 객체가 없고 생성 코루틴도 없으면 이펙트 제거
            if (activeEffects[effectId].objects.Count == 0 && !spawnCoroutines.ContainsKey(effectId))
            {
                activeEffects.Remove(effectId);
            }
        }
    }

    // 이차 베지어 곡선 계산
    private Vector3 CalculateQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 point = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return point;
    }
}