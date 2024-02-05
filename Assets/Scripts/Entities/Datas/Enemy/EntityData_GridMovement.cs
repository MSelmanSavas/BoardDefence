using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class EntityData_GridMovement : EntityComponent_Base
{
    IIndexToPositionProvider _indexToPositionProvider;
    EntityData_EntityManager _entityManager;
    EntityData_GridIndex _entityGridIndex;
    EntityData_GameObject _entityGameObject;
    EntityData_EnemyState _entityEnemyState;

    [SerializeField]
    float _movementCooldown = 2f;
    [SerializeField]
    int _gridMoveDistance = 1;

    float _currentCooldown = 2f;

    public override bool TryInitialize(IEntity entity)
    {
        if (!RefBook.TryGet(out _indexToPositionProvider))
            return false;

        if (!entity.TryGetEntityComponent(out _entityManager))
            return false;

        if (!entity.TryGetEntityComponent(out _entityGridIndex))
            return false;

        if (!entity.TryGetEntityComponent(out _entityGameObject))
            return false;

        if (!entity.TryGetEntityComponent(out _entityEnemyState))
            return false;

        if (entity is not IUpdatableEntity updatableEntity)
            return false;

        updatableEntity.OnUpdate += OnEntityUpdate;

        return base.TryInitialize(entity);
    }

    void OnEntityUpdate()
    {
        if (CheckIsMoving())
            return;

        if (!TryCheckCoolDown())
            return;

        if (!TryMoveGrid())
            return;

        ResetCooldown();
    }

    bool CheckIsMoving()
    {
        return _entityEnemyState.IsMoving;
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
        _currentCooldown = _movementCooldown;
    }

    bool TryMoveGrid()
    {
        int checkDistance = _gridMoveDistance;

        while (checkDistance > 0)
        {
            Vector2Int currentIndex = _entityGridIndex.GetIndex();
            Vector2Int moveToIndex = currentIndex;
            moveToIndex.y += -checkDistance;

            if (!_entityManager.ConnectedEntityManager.TryMoveEntity(currentIndex, moveToIndex))
            {
                checkDistance--;
                continue;
            }

            _entityEnemyState.IsMoving = true;
            _entityEnemyState.CanBeDamaged = false;

            Vector2 moveToPosition = _indexToPositionProvider.GetPosition(moveToIndex);

            _entityGameObject.GetGameObject().transform.DOMove(moveToPosition, 1.5f).OnComplete(() =>
            {
                _entityEnemyState.IsMoving = false;
                _entityEnemyState.CanBeDamaged = true;

            });

            return true;
        }

        return false;
    }
}
