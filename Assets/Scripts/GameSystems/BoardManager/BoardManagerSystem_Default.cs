using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManagerSystem_Default : GameSystem_Base, IGridManager, IEntityManager, IIndexToPositionProvider, IPositionToIndexProvider
{
    #region IEntityManager Fields

    [Sirenix.OdinInspector.ShowInInspector]
    public Dictionary<Vector2Int, IEntity> Entities { get; private set; } = new();

    #endregion

    #region IGridManager Fields
    [Sirenix.OdinInspector.ShowInInspector]
    public Dictionary<Vector2Int, GridBase> Grids { get; private set; } = new();

    #endregion

    #region IEntityManager Methods

    public IEnumerator GetEntityIterator() => Entities.GetEnumerator();
    public bool TryAddEntity(Vector2Int index, IEntity entity)
    {
        if (!IsInsideLimits(Vector2Int.zero, BoardSize, index))
            return false;

        if (Entities.ContainsKey(index))
            return false;

        Entities.Add(index, entity);

        if (entity.TryGetEntityComponent(out EntityData_GameObject entityData_GameObject))
        {
            entityData_GameObject.GetGameObject().transform.SetParent(_entityParents);
        }

        if (entity.TryGetEntityComponent(out EntityData_EntityManager entityData_EntityManager))
        {
            entityData_EntityManager.ConnectedEntityManager = this;
        }

        if (entity.TryGetEntityComponent(out EntityData_GridIndex entityData_GridIndex))
        {
            entityData_GridIndex.AddIndex(index);
        }


        Entities.GetEnumerator();

        return true;
    }

    public bool TryMoveEntity(Vector2Int fromIndex, Vector2Int toIndex)
    {
        if (!IsInsideLimits(Vector2Int.zero, BoardSize, fromIndex))
            return false;

        if (!IsInsideLimits(Vector2Int.zero, BoardSize, toIndex))
            return false;

        if (!TryGetEntity(fromIndex, out IEntity fromEntity))
            return false;

        if (TryGetEntity(toIndex, out IEntity toEntitiy))
            return false;

        if (!TryRemoveEntity(fromIndex, out IEntity removedEntity))
            return false;

        if (!TryAddEntity(toIndex, fromEntity))
            return false;

        return true;
    }

    public bool TryGetEntity(Vector2Int index, out IEntity entity)
    {
        entity = null;

        if (!IsInsideLimits(Vector2Int.zero, BoardSize, index))
            return false;

        return Entities.TryGetValue(index, out entity);
    }

    public bool TryRemoveEntity(Vector2Int index, out IEntity removedEntity)
    {
        removedEntity = null;

        if (!IsInsideLimits(Vector2Int.zero, BoardSize, index))
            return false;

        if (!Entities.TryGetValue(index, out removedEntity))
            return false;

        Entities.Remove(index);

        if (removedEntity.TryGetEntityComponent(out EntityData_EntityManager entityData_EntityManager))
        {
            entityData_EntityManager.ConnectedEntityManager = null;
        }

        if (removedEntity.TryGetEntityComponent(out EntityData_GridIndex entityData_GridIndex))
        {
            entityData_GridIndex.RemoveIndex(index);
        }

        return true;
    }

    #endregion


    #region IGridManager Methods

    public bool TryAddGrid(Vector2Int index, GridBase grid)
    {
        if (!IsInsideLimits(Vector2Int.zero, BoardSize, index))
            return false;

        if (Grids.ContainsKey(index))
            return false;

        Grids.Add(index, grid);

        if (grid.TryGetEntityComponent(out EntityData_GameObject entityData_GameObject))
        {
            entityData_GameObject.GetGameObject().transform.SetParent(_gridParents);
        }

        if (grid.TryGetEntityComponent(out EntityData_GridManager entityData_GridManager))
        {
            entityData_GridManager.ConnectedGridManager = this;
        }

        if (grid.TryGetEntityComponent(out EntityData_GridIndex entityData_GridIndex))
        {
            entityData_GridIndex.AddIndex(index);
        }

        return true;
    }

    public bool TryGetGrid(Vector2Int index, out GridBase grid)
    {
        grid = null;

        if (!IsInsideLimits(Vector2Int.zero, BoardSize, index))
            return false;

        return Grids.TryGetValue(index, out grid);
    }

    public bool TryRemoveGrid(Vector2Int index, out GridBase removedGrid)
    {
        removedGrid = null;

        if (!IsInsideLimits(Vector2Int.zero, BoardSize, index))
            return false;

        if (!Grids.TryGetValue(index, out removedGrid))
            return false;

        Grids.Remove(index);

        if (removedGrid.TryGetEntityComponent(out EntityData_GridManager entityData_GridManager))
        {
            entityData_GridManager.ConnectedGridManager = null;
        }

        if (removedGrid.TryGetEntityComponent(out EntityData_GridIndex entityData_GridIndex))
        {
            entityData_GridIndex.RemoveIndex(index);
        }

        return true;
    }

    public bool TrySetGridSize(Vector2Int gridSize)
    {
        BoardSize = gridSize;

        BoardOrigin = new Vector2
        {
            x = (-BoardCellSize.x / 2f) - ((gridSize.x - 1) / 2f * BoardCellSize.x),
            y = (BoardCellSize.y / 2f) - ((gridSize.y - 1) / 2f * BoardCellSize.y)
        };

        BoardOrigin += BoardCenter;
        BoardOrigin += BoardOffset;

        return true;
    }

    public Vector2Int GetGridSize() => BoardSize;

    #endregion

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    public Vector2Int BoardSize { get; private set; }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    public Vector2 BoardOrigin { get; private set; }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    public Vector2 BoardCenter { get; private set; }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    public Vector2 BoardOffset { get; private set; }

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    public Vector2 BoardCellSize { get; private set; }

    Transform _gridParents;
    Transform _entityParents;
    public override bool TryInitialize(GameSystems gameSystems)
    {
        if (!base.TryInitialize(gameSystems))
            return false;

        if (RefBook.TryGet(out Configurer configurer))
            if (configurer.TryGetConfig(out ConfigBoard configBoard))
                SetBoardConfig(configBoard);

        RefBook.AddAs<IGridManager>(this);
        RefBook.AddAs<IEntityManager>(this);
        RefBook.AddAs<IIndexToPositionProvider>(this);
        RefBook.AddAs<IPositionToIndexProvider>(this);

        _gridParents = new GameObject()
        {
            name = "GridParent"
        }.transform;

        _entityParents = new GameObject()
        {
            name = "EntityParent"
        }.transform;

        return true;
    }

    public override bool TryDeInitialize(GameSystems gameSystems)
    {
        if (!base.TryDeInitialize(gameSystems))
            return false;

        RefBook.RemoveAs<IGridManager>(this);
        RefBook.RemoveAs<IEntityManager>(this);
        RefBook.RemoveAs<IIndexToPositionProvider>(this);
        RefBook.RemoveAs<IPositionToIndexProvider>(this);

        foreach (var (key, entity) in Entities)
        {
            if (entity == null)
                continue;

            if (!entity.TryGetEntityComponent(out EntityData_GameObject gameObjectData))
                continue;

            GameObject.Destroy(gameObjectData.GetGameObject());
        }

        foreach (var (key, grid) in Grids)
        {
            if (grid == null)
                continue;

            if (!grid.TryGetEntityComponent(out EntityData_GameObject gameObjectData))
                continue;

            GameObject.Destroy(gameObjectData.GetGameObject());
        }

        Entities.Clear();
        Grids.Clear();

        GameObject.Destroy(_entityParents.gameObject);
        GameObject.Destroy(_gridParents.gameObject);

        return true;
    }

    void SetBoardConfig(ConfigBoard configBoard)
    {
        if (configBoard == null)
            return;

        BoardOrigin = Vector2.zero;
        BoardCellSize = configBoard.BoardCellSize;
        BoardOffset = configBoard.BoardOffset;
        BoardCenter = configBoard.BoardCenter;
    }


    bool IsInsideLimits(Vector2Int minIndex, Vector2Int maxIndex, Vector2Int checkIndex)
    {
        return checkIndex.x >= minIndex.x && checkIndex.y >= minIndex.y && checkIndex.x < maxIndex.x && checkIndex.y < maxIndex.y;
    }


    #region IIndexToPositionProvider Methods

    public Vector2 GetPosition(Vector2Int index)
    {
        return BoardOrigin + new Vector2(index.x * BoardCellSize.x,
                                   index.y * BoardCellSize.y);
    }

    #endregion

    #region IPositionToIndexProvider Methods

    public Vector2Int GetIndex(Vector2 position)
    {
        Vector2 currentPosition = position - BoardOrigin;

        float xIndex = currentPosition.x / BoardCellSize.x;
        float yIndex = currentPosition.y / BoardCellSize.y;

        return new Vector2Int()
        {
            x = Mathf.RoundToInt(xIndex),
            y = Mathf.RoundToInt(yIndex),
        };
    }

    #endregion
}
