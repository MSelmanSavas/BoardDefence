#if UNITY_EDITOR
using System.Reflection;
#endif
using UnityEngine;
using UsefulDataTypes;

[System.Serializable]
[CreateAssetMenu(fileName = "NewConfigurer", menuName = "BoardDefence/Create/Configurer")]
public class Configurer : ScriptableObject
{
    [System.Serializable]
    public class ConfigReferencesDictionary : SerializableReferenceDictionary<TypeReferenceInheritedFrom<ConfigBase>, ConfigBase> { }

    [field: SerializeField]
    public ConfigReferencesDictionary Configs { get; private set; }

    ConfigBase _lastAccessedConfig;

    [SerializeField]
    ConfigUnityEntitiesContainer _configUnityEntitiesContainer = new();

    [SerializeField]
    ConfigBoard _configBoard = new();

    [SerializeField]
    ConfigLevelDataContainer _configLevelDataContainer;

    public bool TryGetConfig<T>(out T config) where T : ConfigBase
    {
        config = null;

        if (!Configs.TryGetValue(typeof(T), out _lastAccessedConfig))
        {
            return false;
        }

        if (_lastAccessedConfig is not T foundConfig)
            return false;

        config = foundConfig;

        return true;
    }

#if UNITY_EDITOR

    private void OnValidate()
    {
        CollectConfigReferences();
    }

    [Sirenix.OdinInspector.FoldoutGroup("Methods")]
    [Sirenix.OdinInspector.Button]
    public void CollectConfigReferences()
    {
        Configs.Clear();

        var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.GetValue(this) == null)
                continue;

            if (!typeof(ConfigBase).IsAssignableFrom(field.GetValue(this).GetType()))
                continue;

            if (field.GetValue(this) is not ConfigBase economyDataBase)
                continue;

            if (!Configs.ContainsKey(economyDataBase.GetType()))
                Configs.Add(economyDataBase.GetType(), economyDataBase);
        }

        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (!typeof(ConfigBase).IsAssignableFrom(property.GetValue(this).GetType()))
                continue;

            if (property.GetValue(this) is not ConfigBase economyDataBase)
                continue;

            if (!Configs.ContainsKey(economyDataBase.GetType()))
                Configs.Add(economyDataBase.GetType(), economyDataBase);
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssetIfDirty(this);
    }
#endif
}
