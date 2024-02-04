using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConfigLevelDataContainer : ConfigBase
{
    [field: SerializeField]
    public ScriptableLevelDataContainer LevelDataContainer { get; private set; }
}
