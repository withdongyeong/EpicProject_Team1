using System;
using UnityEngine;


/// <summary>
/// 모든 Manager를 관리하는 싱글톤 클래스입니다.
/// 처음에 얘만 넣어주면 실행됩니다.
/// 나중에 초기화용 씬을 따로 둬야하나 생각도 듭니다.
/// </summary>
public class Managers : MonoBehaviour
{
    private GameObject gameManager;
    private GameObject soundManager;
    private GameObject dragManager;
    private GameObject gridManager;
    private GameObject gameStateManager;
    private GameObject timeScaleManager;
    private GameObject goldManager;
    private GameObject StageSelectManager;
    private GameObject FadeManager;
    private GameObject storeLockManager;
    private GameObject globalSetting;
    private GameObject lifeManager;
    private GameObject totalDamageManager;
    private GameObject cursorManager;
    private GameObject steamManager;
    private GameObject journalManager;
    private GameObject steamStatsManager;

    
    



    private void Awake()
    {
        gameManager = Resources.Load<GameObject>("GameManager");
        soundManager = Resources.Load<GameObject>("SoundManager");
        dragManager = Resources.Load<GameObject>("DragManager");
        gridManager = Resources.Load<GameObject>("GridManager");
        gameStateManager = Resources.Load<GameObject>("GameStateManager");
        timeScaleManager = Resources.Load<GameObject>("TimeScaleManager");
        goldManager = Resources.Load<GameObject>("GoldManager");
        StageSelectManager = Resources.Load<GameObject>("StageSelectManager");
        FadeManager = Resources.Load<GameObject>("FadeManager");
        storeLockManager = Resources.Load<GameObject>("StoreLockManager");
        globalSetting = Resources.Load<GameObject>("GlobalSetting");
        lifeManager = Resources.Load<GameObject>("LifeManager");
        totalDamageManager = Resources.Load<GameObject>("TotalDamageManager");
        cursorManager = Resources.Load<GameObject>("CursorManager");
        steamManager = Resources.Load<GameObject>("SteamManager");
        journalManager = Resources.Load<GameObject>("Prefabs/UI/Journal/Journal");
        steamStatsManager = Resources.Load<GameObject>("SteamStatsManager");
        //Instantiate the managers
        MakeManager(gameManager);
        MakeManager(soundManager);
        MakeManager(dragManager);
        MakeManager(gridManager);
        MakeManager(gameStateManager);
        MakeManager(timeScaleManager);
        MakeManager(goldManager);
        MakeManager(StageSelectManager);
        MakeManager(FadeManager);
        MakeManager(storeLockManager);
        MakeManager(globalSetting);
        MakeManager(lifeManager);
        MakeManager(totalDamageManager);
        MakeManager(cursorManager);
        MakeManager(steamManager);
        MakeManager(journalManager);
        MakeManager(steamStatsManager);
    }

    private void MakeManager(GameObject manager)
    {
        GameObject m = Instantiate(manager);
        if (m == null)
        {
            Debug.LogWarning("Managers cannot be instantiated" + manager.name);
        }
    }
    private void Start()
    {
        //SceneLoader.LoadLogo();
    }
}
