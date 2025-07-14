using System;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenToggle : MonoBehaviour
{
    private Toggle _fullScreen;
    
    private void Start()
    {
        _fullScreen = GetComponent<Toggle>();
        _fullScreen.isOn = SaveManager.IsFullScreen;
        _fullScreen.onValueChanged.AddListener(OnFullScreenToggle);
    }

    private void OnFullScreenToggle(bool isOn)
    {
        Screen.fullScreen = isOn;
        SaveManager.SaveIsFullScreen(isOn); // 설정 저장
    }
}
