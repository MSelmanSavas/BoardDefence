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
}
