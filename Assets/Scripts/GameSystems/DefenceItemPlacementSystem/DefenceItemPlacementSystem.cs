using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (_entityManager.TryGetEntity(index, out IEntity entity))
            return;

    }
}
