using System;

public abstract class EnemyBase : UnityEntity_Base, IUpdatableEntity
{
    public Action OnUpdate { get; set; }

    void IUpdatableEntity.UpdateEntity()
    {
        OnUpdate?.Invoke();
    }
}
