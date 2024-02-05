using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Pool;

public class UIElement_LevelSelectorDropdown : UIElement_Base
{
    [SerializeField]
    TMP_Dropdown _levelSelectionDropdown;

    ConfigLevelDataContainer _configLevelDataContainer;
    public override void Initialize()
    {
        base.Initialize();

        if (_levelSelectionDropdown == null)
        {
            Logger.LogErrorWithTag(LogCategory.UI, $"Cannot initialize {nameof(UIElement_LevelSelectorDropdown)}! Dropdown reference is not assigned!");
            return;
        }

        if (!RefBook.TryGet(out Configurer configurer))
        {
            Logger.LogErrorWithTag(LogCategory.UI, $"Cannot initialize {nameof(UIElement_LevelSelectorDropdown)}! No configurer found to pull level datas!");
            return;
        }

        if (!configurer.TryGetConfig(out _configLevelDataContainer))
        {
            Logger.LogErrorWithTag(LogCategory.UI, $"Cannot initialize {nameof(UIElement_LevelSelectorDropdown)}! No {nameof(ConfigLevelDataContainer)} found in {nameof(Configurer)}!");
            return;
        }

        List<string> levelNames = ListPool<string>.Get();

        _levelSelectionDropdown.ClearOptions();

        foreach (var levelData in _configLevelDataContainer.LevelDataContainer.LevelDatas)
        {
            levelNames.Add(levelData.name);
        }

        _levelSelectionDropdown.AddOptions(levelNames);

        ListPool<string>.Release(levelNames);

        _levelSelectionDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public override void DeInitialize()
    {
        base.DeInitialize();
        _levelSelectionDropdown?.onValueChanged.RemoveListener(OnDropdownValueChanged);
    }

    protected override void OnDisableInternal()
    {
        DeInitialize();
    }

    void OnDropdownValueChanged(int selectionIndex)
    {
        if (PlayerDataSystem.Instance.GetPlayerData(out PlayerData_CurrentLevel playerData_CurrentLevel))
        {
            playerData_CurrentLevel.CurrentLevel = selectionIndex;
        }
    }
}
