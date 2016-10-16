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

    public void TakeHit(float damage, RaycastHit hit)
    {
        TakeDamage(damage);
    }

    private void Die()
    {
        Dead = true;

        if (OnDeath != null)
        {
            OnDeath();
        }

        GameObject.Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;

        if (Health <= 0 && !Dead)
        {
            Die();
        }
    }
}
