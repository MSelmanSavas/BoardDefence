using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerSystem : GameSystem_Base
{
    [Sirenix.OdinInspector.ShowInInspector]
    IEntityManager _entityManager;

    [Sirenix.OdinInspector.ShowInInspector]
    IGridManager _gridManager;

    [Sirenix.OdinInspector.ShowInInspector]
    List<Vector2Int> _spawnIndices = new();

    public override bool TryInitialize(GameSystems gameSystems)
    {
        if (!base.TryInitialize(gameSystems))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _entityManager))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _gridManager))
            return false;

        _spawnIndices.Clear();

        Vector2Int boardSize = _gridManager.GetGridSize();
        int yIndex = boardSize.y - 1;

        for (int x = 0; x < boardSize.x; x++)
        {
            _spawnIndices.Add(new Vector2Int(x, yIndex));
        }

        return true;
    }
}
