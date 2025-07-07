using System;
using UnityEngine;

public class LogoSkipSystem : MonoBehaviour
{
    private float skipTime = 0.5f; // 스킵 가능한 시간
    private SkipBarImage _skipBarImage;
    [SerializeField]
    private float timer = 0f; // 타이머
    
    private void Awake()
    {
        // 초기화
        timer = 0f;
    }

    private void Start()
    {
        _skipBarImage = FindAnyObjectByType<SkipBarImage>();
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0))
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer -= Time.deltaTime;
            if (timer < 0f)
            {
                timer = 0f; // 타이머가 음수로 내려가지 않도록 초기화
            }
        }
        _skipBarImage.SetFillAmount(timer / skipTime);
    }
    
    
}
