using UnityEngine;

public interface IPositionToIndexProvider
{
    Vector2Int GetIndex(Vector2 position);
}
