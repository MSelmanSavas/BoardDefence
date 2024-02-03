using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    private void OnEnable()
    {
        RefBook.Add(this);
    }

    private void OnDisable()
    {
        RefBook.Remove(this);
    }

    public void InitializeGameLoop()
    {
        GameObject gameSystemsObj = new GameObject
        {
            name = "GameSystems"
        };

        GameSystems gameSystems = gameSystemsObj.AddComponent<GameSystems>();
    }

    public void DeInitializeGameLoop()
    {
        if (RefBook.TryGet(out GameSystems gameSystems))
        {
            gameSystems.DeInitialize();
            Destroy(gameSystems.gameObject);
        }
    }
}
