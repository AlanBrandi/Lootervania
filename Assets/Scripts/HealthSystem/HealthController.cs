using System;

public class HealthController : HealthModel
{
    public void SetupMaxHealth(int maxHealthValue)
    {
        MaxHealth = maxHealthValue;
    }

    public void IncreaseHealth(int value)
    {
        value = Math.Abs(value);        
        CurrentHealth+=value;
        onHeal?.Invoke(this, value);
    }

    public void ReduceHealth(int value)
    {
        value = Math.Abs(value);
        CurrentHealth-=value;
            
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
                onDeath?.Invoke();
        }
    }
        
    private void OnEnable()
    {
        CurrentHealth = MaxHealth;
    }
}
