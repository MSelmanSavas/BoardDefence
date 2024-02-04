using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataSystem : GameSystem_Base, ILevelDataProvider
{
    int _currentLevel;
    int _maxLevel;
    ScriptableLevelDataContainer _levelDataContainer;

    public override bool TryInitialize(GameSystems gameSystems)
    {
        if (!base.TryInitialize(gameSystems))
            return false;


        if (!RefBook.TryGet(out Configurer configurer))
        {
            Logger.LogErrorWithTag(LogCategory.LevelData, $"Cannot find {nameof(Configurer)} from service provider! Cannot initialize {nameof(LevelDataSystem)}!");
            return false;
        }

        if (!configurer.TryGetConfig(out ConfigLevelDataContainer levelDataContainerConfig))
        {
            Logger.LogErrorWithTag(LogCategory.LevelData, $"Cannot find {nameof(ConfigLevelDataContainer)} from {nameof(Configurer)}! Cannot initialize {nameof(LevelDataSystem)}!");
            return false;
        }

        _levelDataContainer = levelDataContainerConfig.LevelDataContainer;

        RefBook.AddAs<ILevelDataProvider>(this);

        return true;
    }

    public override bool TryDeInitialize(GameSystems gameSystems)
    {
        if (!base.TryDeInitialize(gameSystems))
            return false;

        RefBook.RemoveAs<ILevelDataProvider>(this);
        return true;
    }

    public void SetCurrentLevel(int levelIndex)
    {
        _currentLevel = Mathf.Clamp(levelIndex, 0, _maxLevel);
    }

    public int GetCurrentLevel() => _currentLevel;

    public LevelData GetCurrentLevelData() => _levelDataContainer.LevelDatas[_currentLevel].LevelData;

    public int GetMaxLevel() => _levelDataContainer.LevelDatas.Count;

    public bool TryGetLevelDataAtLevel(int level, out LevelData levelData)
    {
        levelData = null;

        if (level < 0 || level >= GetMaxLevel())
            return false;

        levelData = _levelDataContainer.LevelDatas[level].LevelData;
        return true;
    }
}
