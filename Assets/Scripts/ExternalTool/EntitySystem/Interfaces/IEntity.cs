using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEntity
{
    public virtual bool OnLoad() { return true; }
    public virtual bool OnAfterLoad() { return true; }
    public virtual bool OnSpawned() { return true; }
    public bool TryGetEntityComponent<T>(out T Component) where T : EntityComponent_Base, new();
    public bool TryAddEntityComponent<T>(T Component) where T : EntityComponent_Base, new();
    public bool TryGetOrAddEntityComponent<T>(out T Component) where T : EntityComponent_Base, new();
    public bool TryRemoveEntityComponent<T>() where T : EntityComponent_Base, new();
    public bool TryRemoveEntityComponent<T>(T component) where T : EntityComponent_Base, new();
}
