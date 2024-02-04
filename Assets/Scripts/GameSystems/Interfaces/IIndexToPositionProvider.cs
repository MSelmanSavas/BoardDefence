using UnityEngine;

public interface IIndexToPositionProvider
{
    Vector2 GetPosition(Vector2Int index);
}
