using UnityEngine;
using UsefulDataTypes;

[System.Serializable]
public class ConfigUnityEntitiesContainer : ConfigBase
{
    [System.Serializable]
    public class UnityEntityToUnityEntityDataDictionary : SerializableDictionary<TypeReferenceInheritedFrom<UnityEntity_Base>, UnityEntityData> { }

    [field: SerializeField]
    public UnityEntityToUnityEntityDataDictionary UnityEntityDatas { get; private set; }

    public bool TryGetUnityEntityData<T>(out UnityEntityData unityEntityData) where T : UnityEntity_Base
    {
        if (!UnityEntityDatas.TryGetValue(typeof(T), out unityEntityData))
            return false;


        return true;
    }

    public bool TryGetUnityEntityData(System.Type entityType, out UnityEntityData unityEntityData)
    {
        if (!UnityEntityDatas.TryGetValue(entityType, out unityEntityData))
            return false;


        return true;
    }
}
