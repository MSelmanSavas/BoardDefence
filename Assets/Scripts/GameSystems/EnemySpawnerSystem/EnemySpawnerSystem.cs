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
    ILevelDataProvider _levelDataProvider;

    [Sirenix.OdinInspector.ShowInInspector]
    List<Vector2Int> _spawnIndices = new();

    [Sirenix.OdinInspector.ShowInInspector]
    EnemySpawnData.EnemySpawnLimitsDictionary _enemySpawnLimits = new();

    public override bool TryInitialize(GameSystems gameSystems)
    {
        if (!base.TryInitialize(gameSystems))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _entityManager))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _gridManager))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _levelDataProvider))
            return false;

        LoadSpawnIndices(_gridManager);
        LoadEnemySpawnDatas(_levelDataProvider);

        return true;
    }

    void LoadSpawnIndices(IGridManager gridManager)
    {
        _spawnIndices.Clear();

        Vector2Int boardSize = gridManager.GetGridSize();
        int yIndex = boardSize.y - 1;

        for (int x = 0; x < boardSize.x; x++)
        {
            _spawnIndices.Add(new Vector2Int(x, yIndex));
        }
    }

    void LoadEnemySpawnDatas(ILevelDataProvider levelDataProvider)
    {
        LevelData levelData = levelDataProvider.GetCurrentLevelData();
        EnemySpawnData enemySpawnData = levelData.EnemySpawnData;

        _enemySpawnLimits.Clear();

        foreach (var (enemyType, spawnCount) in enemySpawnData.EnemySpawnLimits)
        {
            _enemySpawnLimits.Add(enemyType, spawnCount);
        }
    }

    public override void Update(RuntimeGameSystemContext gameSystemContext)
    {
        base.Update(gameSystemContext);

        
    }
}
