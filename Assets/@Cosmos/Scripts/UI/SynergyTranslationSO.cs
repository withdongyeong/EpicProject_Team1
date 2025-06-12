using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SynergyTranslation
{
    public string key;
    public string korean;
    public string japanese;
    public string english;
    public string chinese;
}



[CreateAssetMenu(fileName = "SynergyKeywordData", menuName = "Localization/SynergyKeywordData")]
public class SynergyTranslationSO : ScriptableObject
{
    public List<SynergyTranslation> translations;
}
