public interface IUpdatableEntity
{
    System.Action GetOnUpdateEvent();
    void UpdateEntity();
}
