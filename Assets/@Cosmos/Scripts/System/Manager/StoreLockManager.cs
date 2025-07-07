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
}
