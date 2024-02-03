using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    public virtual bool OnLoad() { return true; }
    public virtual bool OnAfterLoad() { return true; }
    public virtual bool OnSpawned() { return true; }
    public bool TryGetComponent<T>(out T Component) where T : EntityComponent_Base, new();
    public bool TryAddComponent<T>(T Component) where T : EntityComponent_Base, new();
    public bool TryGetOrAddComponent<T>(out T Component) where T : EntityComponent_Base, new();
    public bool TryRemoveComponent<T>() where T : EntityComponent_Base, new();
    public bool TryRemoveComponent<T>(T component) where T : EntityComponent_Base, new();
}
