using System.Collections;
using System.Collections.Generic;
using SingletonMonoBehaviour;
using UnityEngine;

public class PlayerDataSystem : SingletonMonoBehaviour<PlayerDataSystem>
{
    protected override bool dontDestroyOnLoad => true;

    [System.Serializable]
    public class PlayerDataDictionary : UsefulDataTypes.SerializableReferenceDictionary<TypeReference, object> { }

    [Sirenix.OdinInspector.ShowInInspector]
    public PlayerDataDictionary PlayerDatas = new PlayerDataDictionary();

    protected override void AwakeInternal() { }

    protected override void OnValidateInternal() { }

    public bool GetPlayerData<T>(out T PlayerData) where T : class, new()
    {
        if (!PlayerDatas.ContainsKey(typeof(T)))
        {
            PlayerData = new T();
            PlayerDatas.Add(typeof(T), PlayerData);
        }

        PlayerData = PlayerDatas[typeof(T)] as T;
        return true;
    }

    public bool GetPlayerData(System.Type dataType, out object PlayerData)
    {
        if (dataType == null)
        {
            PlayerData = null;
            return false;
        }

        if (!PlayerDatas.ContainsKey(dataType))
        {
            PlayerData = System.Activator.CreateInstance(dataType);

            PlayerDatas.Add(dataType, PlayerData);
        }

        PlayerData = PlayerDatas[dataType];
        return true;
    }

    public bool CheckDataExistance<T>()
    {
        return PlayerDatas.ContainsKey(typeof(T));
    }

    public void ResetAllPlayerData()
    {
        PlayerDatas.Clear();
    }
}
