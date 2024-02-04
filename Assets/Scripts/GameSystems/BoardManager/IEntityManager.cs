using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityManager
{
    IEnumerator GetEntityIterator();
    bool TryAddEntity(Vector2Int index, IEntity entity);
    bool TryGetEntity(Vector2Int index, out IEntity entity);
    bool TryMoveEntity(Vector2Int fromIndex, Vector2Int toIndex);
    bool TryRemoveEntity(Vector2Int index, out IEntity removedEntity);
}
