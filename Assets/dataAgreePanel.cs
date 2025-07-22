using System;
using UnityEngine;

public class dataAgreePanel : MonoBehaviour
{
    public GameObject panel;

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
        SaveManager.SaveFirstStart();
    }
}
