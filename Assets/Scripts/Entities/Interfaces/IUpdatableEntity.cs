public interface IUpdatableEntity
{
    System.Action OnUpdate { get; set; }
    void UpdateEntity();
}
