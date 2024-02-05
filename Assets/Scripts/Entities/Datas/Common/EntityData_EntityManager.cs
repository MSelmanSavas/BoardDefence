using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityData_EntityManager : EntityComponent_Base
{
#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ShowInInspector]
#endif
    public IEntityManager ConnectedEntityManager;
}
