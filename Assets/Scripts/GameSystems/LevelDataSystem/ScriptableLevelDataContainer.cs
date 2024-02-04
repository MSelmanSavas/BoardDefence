using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NewLevelDataContainer", menuName = "BoardDefence/Create/LevelDataContainer")]
public class ScriptableLevelDataContainer : ScriptableObject
{
    [field: SerializeField]
    public List<ScriptableLevelData> LevelDatas { get; private set; }
}
