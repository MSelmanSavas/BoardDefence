using System;

public abstract class DefenceItemBase : UnityEntity_Base, IUpdatableEntity
{
    public Action OnUpdate { get; set; }
    void IUpdatableEntity.UpdateEntity()
    {
        OnUpdate?.Invoke();
    }
}