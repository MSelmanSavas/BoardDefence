using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    List<DefenceItemToBeSpawnedData> _defenceItemsToBeSpawnDatas = new();

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    DefenceItemToBeSpawnedData _currentlySelectedDefenceItemData;

    public System.Action<DefenceItemSpawnData> OnDefenceItemSpawned;
    public System.Action<DefenceItemChangeData> OnDefenceItemSelectionChange;

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

        if (_currentlySelectedDefenceItemData == null)
            return;

        if (!TryCheckIndexSuitability(out Vector2Int suitableIndex))
            return;

        if (!TrySpawnSelectedDefenceItemAtIndex(_currentlySelectedDefenceItemData, suitableIndex))
            return;
    }

    bool TryCheckIndexSuitability(out Vector2Int suitableIndex)
    {
        suitableIndex = Vector2Int.left + Vector2Int.down;

        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return false;

        Vector2 mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int index = _positionToIndexProvider.GetIndex(mousePosition);

        if (!_gridManager.TryGetGrid(index, out GridBase foundGrid))
            return false;

        if (foundGrid is not GridBuildable gridBuildable)
            return false;

        if (_entityManager.TryGetEntity(index, out IEntity foundEntity))
            return false;

        suitableIndex = index;

        return true;
    }

    bool TrySpawnSelectedDefenceItemAtIndex(DefenceItemToBeSpawnedData defenceItemToBeSpawnedData, Vector2Int indexToSpawnOn)
    {
        if (defenceItemToBeSpawnedData.ToSpawnAmount <= 0)
            return false;

        if (!_entitesContainer.TryGetUnityEntityData(defenceItemToBeSpawnedData.TypeToSpawn, out UnityEntityData data))
            return false;

        if (data.Prefab == null)
            return false;

        var defenceItemGO = GameObject.Instantiate(data.Prefab);

        if (!defenceItemGO.TryGetComponent(out IEntity defenceItemEntity))
        {
            GameObject.Destroy(defenceItemGO);
            return false;
        }

        if (!_entityManager.TryAddEntity(indexToSpawnOn, defenceItemEntity))
        {
            GameObject.Destroy(defenceItemGO);
            return false;
        }

        if (defenceItemEntity.TryGetEntityComponent(out EntityData_GameObject entityData_GameObject))
        {
            entityData_GameObject.GetGameObject().transform.position = _indexToPositionProvider.GetPosition(indexToSpawnOn);
        }

        int previousCount = defenceItemToBeSpawnedData.ToSpawnAmount;
        defenceItemToBeSpawnedData.ToSpawnAmount--;

        OnDefenceItemSpawned?.Invoke(new DefenceItemSpawnData
        {
            ItemType = defenceItemToBeSpawnedData.TypeToSpawn,
            PreviousCount = previousCount,
            CurrentCount = defenceItemToBeSpawnedData.ToSpawnAmount,
        });

        return true;
    }

    public bool TrySelectDefenceItemToPlaceByType(System.Type type)
    {
        DefenceItemToBeSpawnedData foundData = _defenceItemsToBeSpawnDatas.Where(x => x.TypeToSpawn == type).FirstOrDefault();

        if (foundData == null)
            return false;

        if (foundData.ToSpawnAmount <= 0)
            return false;

        OnDefenceItemSelectionChange?.Invoke(new DefenceItemChangeData
        {
            PreviousType = _currentlySelectedDefenceItemData?.TypeToSpawn,
            CurrentType = foundData.TypeToSpawn,
        });

        _currentlySelectedDefenceItemData = foundData;
        return true;
    }

    public void DeSelectDefenceItemToPlace()
    {
        OnDefenceItemSelectionChange?.Invoke(new DefenceItemChangeData
        {
            PreviousType = _currentlySelectedDefenceItemData?.TypeToSpawn,
            CurrentType = null,
        });

        _currentlySelectedDefenceItemData = null;
    }

    public System.Type GetCurrentSelectedDefenceItemType()
    {
        return _currentlySelectedDefenceItemData != null ? _currentlySelectedDefenceItemData.TypeToSpawn : null;
    }

    public int GetDefenceItemLeftCountByType(System.Type type)
    {
        return _defenceItemsToBeSpawnDatas.Where(x => x.TypeToSpawn == type).FirstOrDefault().ToSpawnAmount;
    }
}
