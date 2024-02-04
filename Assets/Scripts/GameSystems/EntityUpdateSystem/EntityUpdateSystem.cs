using System.Collections.Generic;
using UnityEngine;

public class EntityUpdateSystem : GameSystem_Base
{
    [Sirenix.OdinInspector.ShowInInspector]
    IEntityManager _entityManager;

    public override bool TryInitialize(GameSystems gameSystems)
    {
        if (!base.TryInitialize(gameSystems))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _entityManager))
            return false;

        return true;
    }

    public override void Update(RuntimeGameSystemContext gameSystemContext)
    {
        base.Update(gameSystemContext);

        var entityIterator = _entityManager.GetEntityIterator();

        while (entityIterator.MoveNext())
        {
            KeyValuePair<Vector2Int, IEntity> entityKV = (KeyValuePair<Vector2Int, IEntity>)entityIterator.Current;

            (Vector2Int index, IEntity entity) = entityKV;

            if (entity == null)
                continue;

            if (entity is not IUpdatableEntity updatableEntity)
                continue;

            updatableEntity.UpdateEntity();
        }

        entityIterator.Reset();
    }
}
