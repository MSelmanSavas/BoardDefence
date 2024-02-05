using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UsefulExtensions.List;

public class DefenceItemPlacementSystem : GameSystem_Base
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    ConfigUnityEntitiesContainer _entitesContainer;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    ILevelDataProvider _levelDataProvider;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    IGridManager _gridManager;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    IEntityManager _entityManager;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    IIndexToPositionProvider _indexToPositionProvider;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    IPositionToIndexProvider _positionToIndexProvider;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    Camera _mainCamera;

    [Sirenix.OdinInspector.ShowInInspector]
    List<DefenceItemToBeSpawnedData> _defenceItemsToBeSpawnDatas = new();

    class DefenceItemToBeSpawnedData
    {
        public System.Type TypeToSpawn;
        public int ToSpawnAmount;
    }

    public override bool TryInitialize(GameSystems gameSystems)
    {
        if (!base.TryInitialize(gameSystems))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _levelDataProvider))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _gridManager))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _entityManager))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _indexToPositionProvider))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _positionToIndexProvider))
            return false;

        if (!RefBook.TryGet(out Configurer configurer))
            return false;

        if (!configurer.TryGetConfig(out _entitesContainer))
            return false;

        RefBook.Add(this);

        LoadTowerSpawnDatas(_levelDataProvider);

        _mainCamera = Camera.main;

        return true;
    }

    public override bool TryDeInitialize(GameSystems gameSystems)
    {
        if (!base.TryDeInitialize(gameSystems))
            return false;

        RefBook.Remove(this);

        return true;
    }

    void LoadTowerSpawnDatas(ILevelDataProvider levelDataProvider)
    {
        LevelData levelData = levelDataProvider.GetCurrentLevelData();
        DefenceItemData defenceItemData = levelData.DefenceItemData;

        _defenceItemsToBeSpawnDatas.Clear();

        foreach (var (type, spawnCount) in defenceItemData.DefenceItemLimits)
        {
            _defenceItemsToBeSpawnDatas.Add(new()
            {
                TypeToSpawn = type,
                ToSpawnAmount = spawnCount,
            });
        }
    }

    public override void Update(RuntimeGameSystemContext gameSystemContext)
    {
        base.Update(gameSystemContext);

        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return;

        Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int index = _positionToIndexProvider.GetIndex(mousePosition);

        if (!_gridManager.TryGetGrid(index, out GridBase foundGrid))
            return;

        if (foundGrid is not GridBuildable gridBuildable)
            return;

        if (_entityManager.TryGetEntity(index, out IEntity foundEntity))
            return;

        _defenceItemsToBeSpawnDatas.ShuffleList();

        for (int i = 0; i < _defenceItemsToBeSpawnDatas.Count; i++)
        {
            DefenceItemToBeSpawnedData randomData = _defenceItemsToBeSpawnDatas[i];

            if (randomData.ToSpawnAmount <= 0)
                return;

            if (!_entitesContainer.TryGetUnityEntityData(randomData.TypeToSpawn, out UnityEntityData data))
                return;

            if (data.Prefab == null)
                return;

            var defenceItemGO = GameObject.Instantiate(data.Prefab);

            if (!defenceItemGO.TryGetComponent(out IEntity defenceItemEntity))
            {
                GameObject.Destroy(defenceItemGO);
                return;
            }

            if (!_entityManager.TryAddEntity(index, defenceItemEntity))
            {
                GameObject.Destroy(defenceItemGO);
                return;
            }

            if (defenceItemEntity.TryGetEntityComponent(out EntityData_GameObject entityData_GameObject))
            {
                entityData_GameObject.GetGameObject().transform.position = _indexToPositionProvider.GetPosition(index);
            }

            randomData.ToSpawnAmount--;
        }
    }
}
