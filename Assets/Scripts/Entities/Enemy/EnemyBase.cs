using System;

public abstract class EnemyBase : UnityEntity_Base, IUpdatableEntity
{
    Action _onUpdate;
    public Action GetOnUpdateEvent() =>_onUpdate;

    void IUpdatableEntity.UpdateEntity()
    {
        _onUpdate?.Invoke();
    }
}
