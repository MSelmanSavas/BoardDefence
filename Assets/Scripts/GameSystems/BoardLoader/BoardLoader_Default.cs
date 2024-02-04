using UnityEngine;

public class BoardLoader_Default : GameSystem_Base
{
    [Sirenix.OdinInspector.ShowInInspector]
    IGridManager _gridManager;

    [Sirenix.OdinInspector.ShowInInspector]
    ConfigUnityEntitiesContainer _entityContainer;


    public override bool TryInitialize(GameSystems gameSystems)
    {
        if (!base.TryInitialize(gameSystems))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _gridManager))
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
        Vector2Int gridSize = new Vector2Int(4, 8);

        for (int y = 0; y < gridSize.y; y++)
            for (int x = 0; x < gridSize.x; x++)
            {
                Vector2Int index = new(x, y);

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
