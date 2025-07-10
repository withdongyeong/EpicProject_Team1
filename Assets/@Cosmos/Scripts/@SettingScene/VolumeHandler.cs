using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class VolumeHandler : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    private Slider _masterSlider; // 마스터 음량 슬라이더
    private Slider _bgmSlider; // BGM 음량 슬라이더
    private Slider _sfxSlider; // SFX 음량 슬라이더

    private void Awake()
    {
        _masterSlider = transform.GetChild(0).GetComponent<Slider>();
        _bgmSlider = transform.GetChild(1).GetComponent<Slider>();
        _sfxSlider = transform.GetChild(2).GetComponent<Slider>();
    }

    private void Start()
    {
        float masterVolume = SaveManager.MasterVolume;
        float bgmVolume = SaveManager.BgmVolume;
        float sfxVolume = SaveManager.SfxVolume;
        
        
        if(_masterSlider != null)
        {
            _masterSlider.value = masterVolume;
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(masterVolume, 0.0001f, 1f)) * 20);
            _masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }
        
        if(_bgmSlider != null)
        {
            _bgmSlider.value = bgmVolume;
            audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(bgmVolume, 0.0001f, 1f)) * 20);
            _bgmSlider.onValueChanged.AddListener(SetBgmVolume);
        }
        
        if(_sfxSlider != null)
        {
            _sfxSlider.value = sfxVolume;
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(sfxVolume, 0.0001f, 1f)) * 20);
            _sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }
    }

    
    private void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        SaveManager.SaveMasterVolume(volume);
    }
    
    private void SetBgmVolume(float volume)
    {
        audioMixer.SetFloat("BGMVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        SaveManager.SaveBgmVolume(volume);
    }

    private void SetSfxVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        SaveManager.SaveSfxVolume(volume);
    }
    
    private void OnDestroy()
    {
        if (_masterSlider != null)
            _masterSlider.onValueChanged.RemoveListener(SetMasterVolume);
        
        if (_bgmSlider != null)
            _bgmSlider.onValueChanged.RemoveListener(SetBgmVolume);
        
        if (_sfxSlider != null)
            _sfxSlider.onValueChanged.RemoveListener(SetSfxVolume);
    }
}
