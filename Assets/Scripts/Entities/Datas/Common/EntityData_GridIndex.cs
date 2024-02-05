using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityData_GridIndex : EntityComponent_Base
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    List<Vector2Int> _indices = new();

    public List<Vector2Int> GetIndices() => _indices;
    public Vector2Int GetIndex() => _indices != null && _indices.Count > 0 ? _indices[0] : Vector2Int.left + Vector2Int.down;

    public void AddIndices(ICollection<Vector2Int> indices)
    {
        foreach (var index in indices)
            _indices.Add(index);
    }

    public void AddIndex(Vector2Int index)
    {
        _indices.Add(index);
    }

    public void RemoveIndices(ICollection<Vector2Int> indices)
    {
        foreach (var index in indices)
            _indices.Remove(index);
    }

    public void RemoveIndex(Vector2Int index)
    {
        _indices.Remove(index);
    }

    public void SetIndices(ICollection<Vector2Int> indices)
    {
        _indices.Clear();
        AddIndices(indices);
    }
}
