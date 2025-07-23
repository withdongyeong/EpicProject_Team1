using System;
using Unity.VisualScripting;
using UnityEngine;
using Steamworks;


/// <summary>
/// 정식 출시할 때 미리 저장된 데이터를 초기화하는 프로세스입니다.
/// 작동 방식 :
/// SaveManager에 SaveKey 중 하나라도 존재하면 모든 데이터를 싹다 초기화 시키고 새로운 세이브 키 1.0.0 을 저장합니다.
/// 재 접속 시 1.0.0이 있으면 초기화 프로세스는 작동하지 않습니다.
/// SaveVersion과 게임의 Version은 다를 수 있습니다.
/// </summary>
public static class SaveDataResetProcess
{
    private const string SaveVersionKey = "Save_Version";
    private const string SaveVersion = "1.0.0";
    
    
    private static readonly string[] _saveKeys = 
    {
        SaveKeys.FirstStart,
        SaveKeys.UnlockLevel,
        SaveKeys.IsTutorialCompleted,
        SaveKeys.IsFullScreen,
        SaveKeys.Resolution,
        SaveKeys.MasterVolume,
        SaveKeys.BgmVolume,
        SaveKeys.SfxVolume,
        SaveKeys.GameModeLevel,
        SaveKeys.ShownUnlockLevel,
        SaveKeys.LanguageIndex,
        SaveKeys.DataAgreement
    };

    public static void DataResetProcess()
    {
        if (PlayerPrefs.GetString(SaveVersionKey, "NULL") == SaveVersion)
        {
            Debug.Log("현재 저장된 버전이 최신 버전입니다. 초기화 프로세스는 실행되지 않습니다.");
            return;
        }
        
        
        // 모든 세이브 키 중 하나라도 존재하는지 확인
        foreach (string key in _saveKeys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                // 하나라도 존재하면 초기화 프로세스 실행
                ResetSaveData();
                return;
            }
        }

        PlayerPrefs.SetString(SaveVersionKey, SaveVersion);
        PlayerPrefs.Save();
    }
    
    private static void ResetSaveData()
    {
        Debug.Log("Save_Version_1.0.0 : 초기화 프로세스가 시작되었습니다.");
        SaveManager.DeleteAllSaves();
        // 새로운 세이브 키 1.0.0 저장
        PlayerPrefs.SetString(SaveVersionKey, SaveVersion);
        PlayerPrefs.Save();
        SteamUserStats.ResetAllStats(true);
        SteamUserStats.StoreStats();
        Debug.Log("Save_Version_1.0.0 : 모든 세이브 데이터가 초기화되었습니다.");
    }
}
