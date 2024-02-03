[System.Serializable]
public abstract class EntityComponent_Base
{
    /// <summary>
    /// Used for initializing component before any transformation is done on it.
    /// </summary>
    /// <returns></returns> <summary>
    /// Is initialization successful?
    /// </summary>
    /// <returns></returns>
    public virtual bool TryInitialize(IEntity entity) { return true; }

    /// <summary>
    /// Used for resetting component if necessary. Can be used for pooling.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual bool TryReset(IEntity entity) { return true; }
}
