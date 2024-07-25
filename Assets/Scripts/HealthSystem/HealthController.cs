using System;
using Cinemachine;
using UnityEngine;

public class HealthController : HealthModel
{
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] ParticleSystem deathEffect;
    [SerializeField] Camera mainCamera; 
    [SerializeField] CameraShake cameraShake;
    [SerializeField] Knockback knockback;
    private void Start() 
    {
        knockback = GetComponent<Knockback>();
    }

    public void SetupMaxHealth(int maxHealthValue)
    {
        MaxHealth = maxHealthValue;
    }

    public void IncreaseHealth(int value)
    {
        value = Math.Abs(value);
        CurrentHealth += value;
        onHeal?.Invoke(this, value);
    }

    public void ReduceHealth(int value, Vector2 attackDiretion)
    {
        value = Math.Abs(value);
        CurrentHealth -= value;
        

        if (value > 0)
        {
            onDamageTaken?.Invoke(this, value);

            if (NumberPrefab)
            {
                NumberPrefab.Spawn(value, transform.position + damageNumberOffSetPosition, gameObject);
            }
        }

        if (CurrentHealth <= 0)
        {
            cameraShake.Shake(attackDiretion, 1);
            HandleDeath();
        }
        else
        {
            knockback.StartKnockBack(attackDiretion);
            cameraShake.Shake(attackDiretion, .1f);
            Quaternion spawnRotation = Quaternion.FromToRotation(Vector2.right, attackDiretion);
            Instantiate(hitEffect, transform.position, spawnRotation);
        }
    }

    private void HandleDeath()
    {
        onDeath?.Invoke();

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        

        Destroy(gameObject);
    }

    private void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }
}
