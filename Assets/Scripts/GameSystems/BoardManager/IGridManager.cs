using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridManager
{
    bool TrySetGridSize(Vector2Int gridSize);
    bool TryAddGrid(Vector2Int index, GridBase Grid);
    bool TryGetGrid(Vector2Int index, out GridBase Grid);
    bool TryRemoveGrid(Vector2Int index, out GridBase removedEntity);
}
