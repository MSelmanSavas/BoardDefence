using UnityEngine;
using System.Collections.Generic;
using UsefulDataTypes;
using UsefulDataTypes.Utils;

[System.Serializable]
public class EntityData_CheckRangeAndAttack : EntityComponent_Base
{
    [SerializeField]
    GameObject _attackProjectile;

    [SerializeField]
    List<Direction> _directionsToAttack = new();

    [SerializeField]
    int _attackRange;

    [SerializeField]
    float _attackCooldown;

    float _currentCooldown;

    EntityData_GridIndex _gridIndex;
    IIndexToPositionProvider _indexToPositionProvider;
    EntityData_EntityManager _entityManager;

    public override bool TryInitialize(IEntity entity)
    {
        if (!RefBook.TryGet(out _indexToPositionProvider))
            return false;

        if (!entity.TryGetEntityComponent(out _entityManager))
            return false;

        if (!entity.TryGetEntityComponent(out _gridIndex))
            return false;

        if (entity is not IUpdatableEntity updatableEntity)
            return false;

        updatableEntity.OnUpdate += OnEntityUpdate;

        return base.TryInitialize(entity);
    }

    void OnEntityUpdate()
    {
        if (!TryCheckCoolDown())
            return;

        if (!TryCheckEnemiesInRange())
            return;

        if (!TryAttack())
            return;

        ResetCooldown();
    }

    bool TryCheckCoolDown()
    {
        if (_currentCooldown > 0f)
        {
            _currentCooldown -= Time.deltaTime;
            return false;
        }

        return true;
    }

    void ResetCooldown()
    {
        _currentCooldown = _attackCooldown;
    }

    bool TryCheckEnemiesInRange()
    {
        Vector2Int ownIndex = _gridIndex.GetIndex();
        Vector2Int checkIndex;

        foreach (var direction in _directionsToAttack)
        {
            for (int i = 1; i <= _attackRange; i++)
            {
                Vector2Int directionToOffset = DirectionUtils.GetVector2IntFromDirection(direction);
                checkIndex = ownIndex + (directionToOffset * _attackRange);

                if (!_entityManager.ConnectedEntityManager.TryGetEntity(checkIndex, out IEntity entity))
                    continue;

                if (entity is not EnemyBase enemyBase)
                    continue;

                return true;
            }
        }

        return false;
    }

    bool TryAttack()
    {
        return true;
    }
}
