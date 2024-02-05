using UnityEngine;
using System.Collections.Generic;
using UsefulDataTypes;
using UsefulDataTypes.Utils;

[System.Serializable]
public class EntityData_CheckRangeAndAttack : EntityComponent_Base
{
    [SerializeField]
    List<Direction> _directionsToAttack = new();

    [SerializeField]
    int _attackRange;

    [SerializeField]
    float _attackCooldown;

    [SerializeField]
    float _attackDamage;

    [SerializeField]
    bool _multipleDamage = false;

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

        TryAttack();

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
                checkIndex = ownIndex + (directionToOffset * i);

                Debug.LogError(checkIndex);

                if (!_entityManager.ConnectedEntityManager.TryGetEntity(checkIndex, out IEntity entity))
                    continue;

                if (entity is not EnemyBase enemyBase)
                    continue;

                if (!entity.TryGetEntityComponent(out EntityData_EnemyState entityData_EnemyState))
                    continue;

                if (!entityData_EnemyState.CanBeDamaged)
                    continue;

                return true;
            }
        }

        return false;
    }

    bool TryAttack()
    {
        Vector2Int ownIndex = _gridIndex.GetIndex();
        Vector2Int checkIndex;

        bool hasAlreadyDamagedAnyEnemy = false;

        foreach (var direction in _directionsToAttack)
        {
            for (int i = 1; i <= _attackRange; i++)
            {
                Vector2Int directionToOffset = DirectionUtils.GetVector2IntFromDirection(direction);
                checkIndex = ownIndex + (directionToOffset * i);

                if (!_entityManager.ConnectedEntityManager.TryGetEntity(checkIndex, out IEntity entity))
                    continue;

                if (!entity.TryGetEntityComponent(out EntityData_EnemyHealth entityData_EnemyHealth))
                    continue;

                entityData_EnemyHealth.ChangeHealth(-_attackDamage);

                hasAlreadyDamagedAnyEnemy = true;

                Debug.Log($"Attacked to : {entity} with damage : {_attackDamage}");
                break;
            }

            if (!_multipleDamage && hasAlreadyDamagedAnyEnemy)
                break;
        }

        return true;
    }
}
