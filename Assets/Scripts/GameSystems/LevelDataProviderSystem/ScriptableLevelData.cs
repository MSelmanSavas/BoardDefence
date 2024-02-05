using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "NewLevelData", menuName = "BoardDefence/Create/LevelData")]
public class ScriptableLevelData : ScriptableObject
{
    [field: SerializeField]
    public LevelData LevelData { get; private set; }
}
