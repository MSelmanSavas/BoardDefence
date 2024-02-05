using System;
using UnityEngine;

public abstract class EnemyBase : UnityEntity_Base, IUpdatableEntity
{
    [SerializeField]
    ParticleSystem _onGetDamagedParticleSystem;

    public Action OnUpdate { get; set; }

    void IUpdatableEntity.UpdateEntity()
    {
        OnUpdate?.Invoke();
    }

    private void OnEnable()
    {
        if (TryGetEntityComponent(out EntityData_EnemyHealth enemyHealth))
        {
            enemyHealth.OnHealthChange += OnHealthChange;
            enemyHealth.OnHealthDeplete += OnHealthDeplete;
        }
    }

    private void OnDisable()
    {
        if (TryGetEntityComponent(out EntityData_EnemyHealth enemyHealth))
        {
            enemyHealth.OnHealthChange -= OnHealthChange;
            enemyHealth.OnHealthDeplete -= OnHealthDeplete;
        }
    }

    void OnHealthDeplete()
    {
        if (!TryGetEntityComponent(out EntityData_EntityManager entityManager))
            return;

        if (!TryGetEntityComponent(out EntityData_GridIndex indexData))
            return;

        entityManager.ConnectedEntityManager.TryRemoveEntity(indexData.GetIndices()[0], out IEntity removedEntity);
        Destroy(gameObject);
    }

    void OnHealthChange(float previousHealth, float currentHealth)
    {
        _onGetDamagedParticleSystem?.Play();
    }
}
