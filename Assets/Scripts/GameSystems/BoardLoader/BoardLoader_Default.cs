public class BoardLoader_Default : GameSystem_Base
{
    [Sirenix.OdinInspector.ShowInInspector]
    IGridManager _gridManager;

    public override bool TryInitialize(GameSystems gameSystems)
    {
        if (!base.TryInitialize(gameSystems))
            return false;

        if (!gameSystems.TryGetGameSystemByTypeWithoutConstraint(out _gridManager))
            return false;

        return true;
    }
}
