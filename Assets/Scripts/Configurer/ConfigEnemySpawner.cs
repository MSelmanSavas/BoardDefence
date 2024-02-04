using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConfigEnemySpawner : ConfigBase
{
    [field: SerializeField]
    public float SpawnCooldown { get; private set; } = 2f;
}
