using UnityEngine;

public class BoardLoader_Default : GameSystem_Base
{
    [Sirenix.OdinInspector.ShowInInspector]
    IGridManager _gridManager;

    [Sirenix.OdinInspector.ShowInInspector]
    ConfigUnityEntitiesContainer _entityContainer;

    [Sirenix.OdinInspector.ShowInInspector]
    ILevelDataProvider _levelDataProvider;

    public override bool TryInitialize(GameSystems gameSystems)
    {
        if (!base.TryInitialize(gameSystems))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _gridManager))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _levelDataProvider))
            return false;

        if (!RefBook.TryGet(out Configurer configurer))
            return false;

        if (!configurer.TryGetConfig(out _entityContainer))
            return false;

        try
        {
            LoadGameplayGrids(_gridManager, _entityContainer);
        }
        catch (System.Exception e)
        {
            Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Error while loading level data to gamefield! Error : {e}");
            return false;
        }

        return true;
    }

    void LoadGameplayGrids(IGridManager gridManager, ConfigUnityEntitiesContainer entityContainer)
    {
        LevelData levelData = _levelDataProvider.GetCurrentLevelData();

        Vector2Int gridSize = levelData.BoardSize;
        gridManager.TrySetGridSize(gridSize);

        for (int i = 0; i < levelData.BuildableIndices.Count; i++)
        {
            Vector2Int index = levelData.BuildableIndices[i];

            if (!entityContainer.TryGetUnityEntityData(typeof(GridBuildable), out UnityEntityData data))
            {
                Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Cannot find data. Skipping grid at index : {index}");
                continue;
            }

            if (data.Prefab == null)
            {
                Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Cannot find prefab from data. Skipping grid at index : {index}");
                continue;
            }

            var gridObj = GameObject.Instantiate(data.Prefab);

            if (!gridObj.TryGetComponent(out GridBase gridBase))
            {
                Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Cannot find {nameof(GridBase)} on obj {gridObj}!. Skipping grid at index : {index}");
                GameObject.Destroy(gridObj);
                continue;
            }

            if (!gridManager.TryAddGrid(index, gridBase))
            {
                Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Cannot add {gridBase} on {gridManager}!. Skipping grid at index : {index}");
                GameObject.Destroy(gridObj);
                continue;
            }
        }

        for (int y = 0; y < gridSize.y; y++)
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2Int index = new(x, y);

                if (gridManager.TryGetGrid(index, out GridBase foundGrid) && foundGrid != null)
                    continue;

                if (!entityContainer.TryGetUnityEntityData(typeof(GridDefault), out UnityEntityData data))
                {
                    Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Cannot find data. Skipping grid at index : {index}");
                    continue;
                }

                if (data.Prefab == null)
                {
                    Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Cannot find prefab from data. Skipping grid at index : {index}");
                    continue;
                }

                var gridObj = GameObject.Instantiate(data.Prefab);

                if (!gridObj.TryGetComponent(out GridBase gridBase))
                {
                    Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Cannot find {nameof(GridBase)} on obj {gridObj}!. Skipping grid at index : {index}");
                    GameObject.Destroy(gridObj);
                    continue;
                }

                if (!gridManager.TryAddGrid(index, gridBase))
                {
                    Logger.LogErrorWithTag(LogCategory.BoardLoader, $"Cannot add {gridBase} on {gridManager}!. Skipping grid at index : {index}");
                    GameObject.Destroy(gridObj);
                    continue;
                }
            }
    }
}
