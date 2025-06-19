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
        globalSetting = Resources.Load<GameObject>("GlobalSetting");
        //Instantiate the managers
        Instantiate(gameManager);
        Instantiate(soundManager);
        Instantiate(dragManager);
        Instantiate(gridManager);
        Instantiate(gameStateManager);
        Instantiate(timeScaleManager);
        Instantiate(goldManager);
        Instantiate(globalSetting);
    }

    private void Start()
    {
        SceneLoader.LoadSceneWithName("BuildingScene_KYH");
    }
}
