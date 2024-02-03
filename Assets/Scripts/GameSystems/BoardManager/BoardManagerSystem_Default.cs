using System.Collections.Generic;
using UnityEngine;

public class BoardManagerSystem : GameSystem_Base, IGridManager, IEntityManager
{
    #region IEntityManager Fields

    [Sirenix.OdinInspector.ShowInInspector]
    public Dictionary<Vector2Int, IEntity> Entities { get; private set; }

    #endregion

    #region IGridManager Fields
    [Sirenix.OdinInspector.ShowInInspector]
    public Dictionary<Vector2Int, GridBase> Grids { get; private set; }

    #endregion

    #region IEntityManager Methods

    public bool TryAddEntity(Vector2Int index, IEntity entity)
    {
        if (Entities.ContainsKey(index))
            return false;

        Entities.Add(index, entity);

        return true;
    }

    public bool TryGetEntity(Vector2Int index, out IEntity entity)
    {
        return Entities.TryGetValue(index, out entity);
    }

    public bool TryRemoveEntity(Vector2Int index, out IEntity removedEntity)
    {
        if (!Entities.TryGetValue(index, out removedEntity))
            return false;

        Entities.Remove(index);

        return true;
    }

    #endregion


    #region IGridManager Methods

    public bool TryAddGrid(Vector2Int index, GridBase Grid)
    {

        if (Grids.ContainsKey(index))
            return false;

        Grids.Add(index, Grid);

        return true;
    }

    public bool TryGetGrid(Vector2Int index, out GridBase Grid)
    {
        return Grids.TryGetValue(index, out Grid);
    }

    public bool TryRemoveGrid(Vector2Int index, out GridBase removedGrid)
    {
        if (!Grids.TryGetValue(index, out removedGrid))
            return false;

        Grids.Remove(index);

        return true;
    }

    #endregion

    public override bool TryInitialize(GameSystems gameSystems)
    {
        if (!base.TryInitialize(gameSystems))
            return false;

        RefBook.Add(this as IGridManager);
        RefBook.Add(this as IEntityManager);

        return true;
    }

    public override bool TryDeInitialize(GameSystems gameSystems)
    {
        if (!base.TryDeInitialize(gameSystems))
            return false;

        RefBook.Remove(this as IGridManager);
        RefBook.Remove(this as IEntityManager);

        return true;
    }
}
