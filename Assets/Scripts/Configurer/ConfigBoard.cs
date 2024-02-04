using UnityEngine;

[System.Serializable]
public class ConfigBoard : ConfigBase
{
    [field: SerializeField]
    public Vector2 BoardOffset { get; private set; }

    [field: SerializeField]
    public Vector2 BoardCenter { get; private set; }

    [field: SerializeField]
    public Vector2 BoardCellSize { get; private set; }
}
