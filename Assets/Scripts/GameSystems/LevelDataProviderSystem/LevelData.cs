using System.Collections.Generic;
using UnityEngine;
using UsefulDataTypes;

[System.Serializable]
public class LevelData
{
    public Vector2Int BoardSize;
    public List<Vector2Int> BuildableIndices = new();
    public EnemySpawnData EnemySpawnData;
    public DefenceItemData DefenceItemData;
}

[System.Serializable]
public class EnemySpawnData
{
    [System.Serializable]
    public class EnemySpawnLimitsDictionary : SerializableDictionary<TypeReferenceInheritedFrom<EnemyBase>, int> { }

    [field: SerializeField]
    public EnemySpawnLimitsDictionary EnemySpawnLimits { get; private set; } = new();
}

[System.Serializable]
public class DefenceItemData
{
    [System.Serializable]
    public class DefenceItemLimitsDictionary : SerializableDictionary<TypeReferenceInheritedFrom<DefenceItemBase>, int> { }

    [field: SerializeField]
    public DefenceItemLimitsDictionary DefenceItemLimits { get; private set; } = new();
}
