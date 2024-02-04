using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelDataProvider
{
    public void SetCurrentLevel(int levelIndex);
    public int GetCurrentLevel();
    public int GetMaxLevel();
    public bool TryGetLevelDataAtLevel(int level, out LevelData levelData);
    public LevelData GetCurrentLevelData();
}
