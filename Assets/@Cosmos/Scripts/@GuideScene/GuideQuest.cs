using UnityEngine;

public abstract class GuideQuest : MonoBehaviour
{
    public string instructionText;

    public abstract bool IsCompleted();

    public virtual void OnStart() { }
    public virtual void OnComplete() { }
}
