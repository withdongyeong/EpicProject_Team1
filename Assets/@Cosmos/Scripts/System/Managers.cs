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
        globalSetting = Resources.Load<GameObject>("GlobalSetting");
        storeLockManager = Resources.Load<GameObject>("StoreLockManager");
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
        MakeManager(globalSetting);
        MakeManager(storeLockManager);
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
