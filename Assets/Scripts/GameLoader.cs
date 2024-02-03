using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    [SerializeField]
    Configurer _configurer;

    private void OnEnable()
    {
        if (_configurer != null)
            RefBook.Add(_configurer);

        RefBook.Add(this);
    }

    private void OnDisable()
    {
        if (_configurer != null)
            RefBook.Remove(_configurer);

        RefBook.Remove(this);
    }

    private void Start()
    {
        InitializeGameLoop();
    }

    public void InitializeGameLoop()
    {
        GameObject gameSystemsObj = new GameObject
        {
            name = "GameSystems"
        };

        GameSystems gameSystems = gameSystemsObj.AddComponent<GameSystems>();
        gameSystems.TryAddGameSystemByTypeImmediately<BoardManagerSystem_Default>(autoInitialize: false);
        gameSystems.TryAddGameSystemByTypeImmediately<BoardLoader_Default>(autoInitialize: false);

        gameSystems.Initialize();
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
