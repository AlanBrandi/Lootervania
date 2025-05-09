using System;
using Cinemachine;
using UnityEngine;

public class HealthController : HealthModel
{
    [SerializeField] ParticleSystem hitEffect;
    [SerializeField] ParticleSystem spikesHitEffect;
    [SerializeField] ParticleSystem deathEffect;
    [SerializeField] CameraShake cameraShake;
    [SerializeField] private float offsetDistance = 1.0f;
    private Knockback knockback;
    private Animator anim;
    private FlashDamage flashDamage;

    public GameObject dieFX;
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        flashDamage = GetComponentInChildren<FlashDamage>();
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
    public void PlayAnimHit()
    {
        anim.SetTrigger("Hit");
    }

    public void ReduceHealth(int value, Vector2 attackDirection)
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
            cameraShake.Shake(attackDirection, 1);
            HandleDeath();
            return;
        }
        
        flashDamage.Flash();
        anim.SetTrigger("Hit");
        
        if(knockback)
            knockback.StartKnockBack(attackDirection);
        
        if(cameraShake)
            cameraShake.Shake(attackDirection, .1f);

        Quaternion spawnRotation = Quaternion.FromToRotation(Vector2.right, attackDirection);
        Quaternion spikesSpawnRotation = Quaternion.FromToRotation(Vector2.right, -attackDirection);
        Vector2 spawnOffset = attackDirection.normalized * offsetDistance;
        Vector3 spawnPosition = transform.position + (Vector3)spawnOffset;

        Instantiate(hitEffect, spawnPosition, spawnRotation);
        Instantiate(spikesHitEffect, transform.position, spikesSpawnRotation);
    }

    public void ReduceHealthNoKnockback(int value)
    {
        value = Math.Abs(value);
        CurrentHealth -= value;

        if (value > 0)
        {
            onDamageTaken?.Invoke(this, value);

            if (NumberPrefab != null)
            {
                NumberPrefab.Spawn(value, transform.position + damageNumberOffSetPosition, gameObject);
            }
        }

        if (CurrentHealth <= 0)
        {
            HandleDeath();
            return;
        }
        flashDamage.Flash();
        anim.SetTrigger("Hit");
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

    public void DeathFX()
    {
         Instantiate(dieFX, transform.position, Quaternion.identity);
    }
}
