using System;
using UnityEngine;

public abstract class GuideQuest : MonoBehaviour
{
    public string instructionText;
    public string titleText;
    public string subTitleText;
    public string contentText;
    public string goalText;

    public virtual void  Awake()
    {
        SetTexts();
    }

    public abstract void SetTexts();
    public abstract bool IsCompleted();

    public virtual void OnStart() { }
    public virtual void OnComplete() { }
}
