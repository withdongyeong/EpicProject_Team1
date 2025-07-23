using System;
using UnityEngine;

public class dataAgreePanel : MonoBehaviour
{
    public GameObject panel;
    
    // 개인정보 처리 방침 페이지 URL
    [SerializeField] private string privacyPolicyURL = "https://store.steampowered.com/news/app/3822990/view/520845543638828964";

    private void Awake()
    {
        if (SaveManager.FirstStart == 0)
        {
            panel.gameObject.SetActive(true);
        }
        else
        {
            panel.gameObject.SetActive(false);
        }
    }
    
    public void OnAgree()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");
        panel.gameObject.SetActive(false);
        SaveManager.SaveDataAgreement(true);
        SaveManager.SaveFirstStart();
        AnalyticsManager.Instance.CollectStart();
    }
    
    public void OnDisagree()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");
        panel.gameObject.SetActive(false);
        SaveManager.SaveDataAgreement(false);
        SaveManager.SaveFirstStart();
    }
    
    public void OnShow()
    {
        SoundManager.Instance.UISoundClip("ButtonActivate");
        
        // 우선순위: 스팀 클라이언트 → 스팀 오버레이 → 기본 브라우저
        OpenPrivacyPolicy();
    }
    
    private void OpenPrivacyPolicy()
    {
        // 1순위: 스팀 클라이언트에서 열기 시도
        if (TryOpenInSteamClient())
        {
            Debug.Log("스팀 클라이언트에서 개인정보 처리 방침 열기 성공");
            return;
        }
        
        // 2순위: 스팀 오버레이에서 열기 시도
        if (TryOpenInSteamOverlay())
        {
            Debug.Log("스팀 오버레이에서 개인정보 처리 방침 열기 성공");
            return;
        }
        
        // 3순위: 기본 브라우저에서 열기 (최종 폴백)
        Debug.Log("스팀 접근 실패, 기본 브라우저에서 개인정보 처리 방침 열기");
        Application.OpenURL(privacyPolicyURL);
    }
    
    private bool TryOpenInSteamClient()
    {
        try
        {
            // 스팀 브라우저에서 열기
            string steamBrowserURL = $"steam://openurl/{privacyPolicyURL}";
            Application.OpenURL(steamBrowserURL);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"스팀 클라이언트 열기 실패: {e.Message}");
            return false;
        }
    }
    
    private bool TryOpenInSteamOverlay()
    {
        try
        {
            // 스팀 오버레이 브라우저 사용
            string overlayURL = $"steam://overlay/http/{privacyPolicyURL.Replace("https://", "")}";
            Application.OpenURL(overlayURL);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"스팀 오버레이 열기 실패: {e.Message}");
            return false;
        }
    }
}