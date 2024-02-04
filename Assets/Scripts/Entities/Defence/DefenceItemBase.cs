using System;

public abstract class DefenceItemBase : UnityEntity_Base, IUpdatableEntity
{
    Action _onUpdate;
    public Action GetOnUpdateEvent() => _onUpdate;

    void IUpdatableEntity.UpdateEntity()
    {
        _onUpdate?.Invoke();
    }
}