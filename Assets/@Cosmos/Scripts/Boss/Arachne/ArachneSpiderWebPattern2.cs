using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArachneSpiderWebPattern2 : MonoBehaviour
{
    public int _spiderWebCount = 8;
    public GameObject _spiderWebPrefeb;

    private PlayerController _playerController;
    private bool Iscircumference;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _playerController = FindAnyObjectByType<PlayerController>();

        // 2초마다 MyFunction을 호출, 시작 지연 시간은 0초
        InvokeRepeating("MyFunction", 1f, 6f);
    }

    void MyFunction()
    {
        List<GameObject> warningTiles = new List<GameObject>();
        Iscircumference = false;

        for (int i = 0; i < _spiderWebCount; i++)
        {
            int X = Random.Range(0, 8);
            int Y = Random.Range(0, 8);

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= -1; y++)
                {
                    int circumferenceX = _playerController.CurrentX + x;
                    int circumferenceY = _playerController.CurrentY + y;

                    if (circumferenceX == X && circumferenceY == Y)
                    {
                        Iscircumference = true;
                    }
                }
            }

            if (GridManager.Instance.IsWithinGrid(new Vector3Int(X, Y, 0)) && !Iscircumference)
            {
                Vector3 pos = GridManager.Instance.GridToWorldPosition(new Vector3Int(X, Y, 0));
                warningTiles.Add(Object.Instantiate(_spiderWebPrefeb, pos, Quaternion.identity));
            }
        }
    }
}
