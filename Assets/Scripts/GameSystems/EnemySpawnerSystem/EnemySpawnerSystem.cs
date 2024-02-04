using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UsefulExtensions.List;

public class EnemySpawnerSystem : GameSystem_Base
{
    [Sirenix.OdinInspector.ShowInInspector]
    IEntityManager _entityManager;

    [Sirenix.OdinInspector.ShowInInspector]
    IGridManager _gridManager;

    [Sirenix.OdinInspector.ShowInInspector]
    ILevelDataProvider _levelDataProvider;

    [Sirenix.OdinInspector.ShowInInspector]
    List<Vector2Int> _spawnIndicesOriginalReference = new();

    [Sirenix.OdinInspector.ShowInInspector]
    List<Vector2Int> _spawnIndicesRuntime = new();

    [Sirenix.OdinInspector.ShowInInspector]
    List<EnemyToBeSpawnData> _enemyToBeSpawnDatas = new();

    [Sirenix.OdinInspector.ShowInInspector]
    ConfigEnemySpawner _configEnemySpawner;

    [Sirenix.OdinInspector.ShowInInspector]
    ConfigUnityEntitiesContainer _configEntityContainer;

    [Sirenix.OdinInspector.ShowInInspector]
    float _currentWaitTime;

    class EnemyToBeSpawnData
    {
        public System.Type EnemyTypeToSpawn;
        public int EnemyAmountLeftToSpawn;
    }

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

        if (!RefBook.TryGet(out Configurer configurer))
        {
            Logger.LogErrorWithTag(LogCategory.LevelData, $"Cannot find {nameof(Configurer)} from service provider! Cannot initialize {nameof(EnemySpawnerSystem)}!");
            return false;
        }

        if (!configurer.TryGetConfig(out _configEnemySpawner))
        {
            Logger.LogErrorWithTag(LogCategory.LevelData, $"Cannot find {nameof(ConfigEnemySpawner)} from {nameof(Configurer)}! Cannot initialize {nameof(EnemySpawnerSystem)}!");
            return false;
        }


        if (!configurer.TryGetConfig(out _configEntityContainer))
        {
            Logger.LogErrorWithTag(LogCategory.LevelData, $"Cannot find {nameof(ConfigUnityEntitiesContainer)} from {nameof(Configurer)}! Cannot initialize {nameof(EnemySpawnerSystem)}!");
            return false;
        }

        LoadSpawnIndices(_gridManager);
        LoadEnemySpawnDatas(_levelDataProvider);
        _currentWaitTime = _configEnemySpawner.SpawnCooldown;

        return true;
    }

    void LoadSpawnIndices(IGridManager gridManager)
    {
        _spawnIndicesOriginalReference.Clear();
        _spawnIndicesRuntime.Clear();

        Vector2Int boardSize = gridManager.GetGridSize();
        int yIndex = boardSize.y - 1;

        for (int x = 0; x < boardSize.x; x++)
        {
            _spawnIndicesOriginalReference.Add(new Vector2Int(x, yIndex));
            _spawnIndicesRuntime.Add(new Vector2Int(x, yIndex));
        }
    }

    void LoadEnemySpawnDatas(ILevelDataProvider levelDataProvider)
    {
        LevelData levelData = levelDataProvider.GetCurrentLevelData();
        EnemySpawnData enemySpawnData = levelData.EnemySpawnData;

        _enemyToBeSpawnDatas.Clear();

        foreach (var (enemyType, spawnCount) in enemySpawnData.EnemySpawnLimits)
        {
            _enemyToBeSpawnDatas.Add(new()
            {
                EnemyTypeToSpawn = enemyType,
                EnemyAmountLeftToSpawn = spawnCount,
            });
        }
    }

    public override void Update(RuntimeGameSystemContext gameSystemContext)
    {
        base.Update(gameSystemContext);

        if (!TryCheckCoolDown())
            return;

        if (!TryFindIndexToSpawn(_entityManager, out Vector2Int indexToSpawn))
            return;

        if (!TryFindEnemyToSpawn(_entityManager, indexToSpawn, _configEntityContainer))
            return;
    }

    bool TryCheckCoolDown()
    {
        if (_currentWaitTime > 0f)
        {
            _currentWaitTime -= Time.deltaTime;
            return false;
        }

        _currentWaitTime = _configEnemySpawner.SpawnCooldown;
        return true;
    }

    bool TryFindIndexToSpawn(IEntityManager entityManager, out Vector2Int indexToSpawn)
    {
        indexToSpawn = Vector2Int.left + Vector2Int.down;

        _spawnIndicesRuntime.ShuffleList();

        for (int i = 0; i < _spawnIndicesRuntime.Count; i++)
        {
            Vector2Int indexToCheck = _spawnIndicesRuntime[i];

            if (entityManager.TryGetEntity(indexToCheck, out IEntity entity))
                continue;

            indexToSpawn = indexToCheck;
            return true;
        }

        return false;
    }

    bool TryFindEnemyToSpawn(IEntityManager entityManager, Vector2Int indexToSpawn, ConfigUnityEntitiesContainer entitiesContainer)
    {
        _enemyToBeSpawnDatas.ShuffleList();

        EnemyToBeSpawnData checkSpawnData;

        for (int i = 0; i < _enemyToBeSpawnDatas.Count; i++)
        {
            checkSpawnData = _enemyToBeSpawnDatas[i];

            if (checkSpawnData.EnemyAmountLeftToSpawn <= 0)
                continue;

            if (!entitiesContainer.TryGetUnityEntityData(checkSpawnData.EnemyTypeToSpawn, out UnityEntityData entityData))
                continue;

            checkSpawnData.EnemyAmountLeftToSpawn--;


            if (entityData.Prefab == null)
            {
                Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Cannot find prefab from data for enemy type : {checkSpawnData.EnemyTypeToSpawn}");
                continue;
            }

            var enemyObj = GameObject.Instantiate(entityData.Prefab);

            if (!enemyObj.TryGetComponent(out IEntity entity))
            {
                Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Cannot find {nameof(IEntity)} on obj {enemyObj}!");
                GameObject.Destroy(enemyObj);
                continue;
            }

            if (!entityManager.TryAddEntity(indexToSpawn, entity))
            {
                Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Cannot add {entity} on {entityManager} with index : {indexToSpawn}");
                GameObject.Destroy(enemyObj);
                continue;
            }

            return true;
        }

        return false;
    }
}
