using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIElement_LoadLevel : UIElement_Base
{
    [SerializeField]
    Button _loadLevelButton;

    GameLoader _gameLoader;

    public override void Initialize()
    {
        base.Initialize();

        if (_loadLevelButton == null)
        {
            Logger.LogErrorWithTag(LogCategory.UI, $"Cannot initialize {nameof(UIElement_LoadLevel)}! Button reference is not assigned!");
            return;
        }

        if (!RefBook.TryGet(out _gameLoader))
        {
            Logger.LogErrorWithTag(LogCategory.UI, $"Cannot initialize {nameof(UIElement_LoadLevel)}! No {nameof(GameLoader)} found!");
            return;
        }

        _loadLevelButton.onClick.AddListener(LoadLevel);
    }

    public override void DeInitialize()
    {
        base.DeInitialize();
        _loadLevelButton.onClick.RemoveListener(LoadLevel);
    }

    protected override void OnDisableInternal()
    {
        base.OnDisableInternal();
        DeInitialize();
    }

    void LoadLevel()
    {
        _gameLoader?.DeInitializeGameLoop();
        _gameLoader?.InitializeGameLoop();
    }
}
