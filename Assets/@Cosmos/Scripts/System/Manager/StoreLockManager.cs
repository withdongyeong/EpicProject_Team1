using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class StoreLockManager : Singleton<StoreLockManager>
{
    private GameObject[] storeLocks = new GameObject[5];

    public GameObject GetStoreLocks(int i)
    {
        if (storeLocks[i] != null)
        {
            return storeLocks[i];
        }
        else
            return null;
    }

    public void AssignStoreLock(int i, GameObject prefab)
    {
        storeLocks[i] = prefab;
    }

    public void RemoveStoreLock(int i)
    {
        if (storeLocks[i] != null)
        {
            storeLocks[i] = null;
        }
    }

    public int GetStoreLockInt()
    {
        int result = 0;
        foreach(GameObject _object in storeLocks)
        {
            if(_object != null)
            {
                result++;
            }
        }
        return result;
    }
}
