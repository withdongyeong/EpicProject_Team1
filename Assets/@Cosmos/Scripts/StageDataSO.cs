using UnityEngine;


[CreateAssetMenu(fileName = "New Stage", menuName = "NewStage/StageData")]
public class StageDataSO : ScriptableObject
{
    public GameObject enemyPrefab;
    public Sprite backgroundSprite;
    public AudioClip bgmClip;
    public float bgmVolume;
}
