using System;
using UnityEngine;

public abstract class LivingEntity : MonoBehaviour, IDamageable
{
    public float StartHealth;

    public event Action OnDeath;

    protected float Health;
    protected bool Dead;

    protected virtual void Start()
    {
        Health = StartHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    [ContextMenu("Self Destruct")]
    protected virtual void Die()
    {
        Dead = true;

        if (OnDeath != null)
        {
            OnDeath();
        }

        GameObject.Destroy(gameObject);
    }

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0 && !Dead)
        {
            Die();
        }
    }
}
