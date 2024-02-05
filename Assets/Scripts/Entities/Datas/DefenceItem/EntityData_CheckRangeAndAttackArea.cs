using UnityEngine;
using System.Collections.Generic;
using UsefulDataTypes;
using UsefulDataTypes.Utils;

[System.Serializable]
public class EntityData_CheckRangeAndAttackArea : EntityComponent_Base
{

    [SerializeField]
    int _attackRange;

    [SerializeField]
    float _attackCooldown;

    [SerializeField]
    float _attackDamage;

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

        Vector2Int minIndex = ownIndex + DirectionUtils.GetVector2IntFromDirection(Direction.DownLeft) * _attackRange;
        Vector2Int maxIndex = ownIndex + DirectionUtils.GetVector2IntFromDirection(Direction.UpRight) * _attackRange;

        for (int x = minIndex.x; x <= maxIndex.x; x++)
        {
            for (int y = minIndex.y; y <= maxIndex.y; y++)
            {
                checkIndex = new Vector2Int(x, y);

                if (checkIndex == ownIndex)
                    continue;

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

        Vector2Int minIndex = ownIndex + DirectionUtils.GetVector2IntFromDirection(Direction.DownLeft) * _attackRange;
        Vector2Int maxIndex = ownIndex + DirectionUtils.GetVector2IntFromDirection(Direction.UpRight) * _attackRange;

        for (int x = minIndex.x; x <= maxIndex.x; x++)
        {
            for (int y = minIndex.y; y <= maxIndex.y; y++)
            {
                checkIndex = new Vector2Int(x, y);

                if (checkIndex == ownIndex)
                    continue;

                if (!_entityManager.ConnectedEntityManager.TryGetEntity(checkIndex, out IEntity entity))
                    continue;

                if (!entity.TryGetEntityComponent(out EntityData_EnemyHealth entityData_EnemyHealth))
                    continue;

                entityData_EnemyHealth.ChangeHealth(-_attackDamage);
                Debug.Log($"Attacked to : {entity} with damage : {_attackDamage}");
            }
        }

        return true;
    }
}
