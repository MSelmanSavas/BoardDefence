using UnityEngine;

[System.Serializable]
public class UnityEntityData
{
    [field: SerializeField]
    public GameObject Prefab { get; private set; }
}
