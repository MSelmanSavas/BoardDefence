public abstract class GameSystem_Base
{
    public bool IsMarkedForRemoval = false;

    [Sirenix.OdinInspector.ShowInInspector]
    public GameSystems GameSystems { get; protected set; }

    public virtual bool TryInitialize(GameSystems gameSystems)
    {
        GameSystems = gameSystems;
        return true;
    }

    public virtual bool TryDeInitialize(GameSystems gameSystems)
    {
        return true;
    }

    public virtual void Update(RuntimeGameSystemContext gameSystemContext) { }
}
