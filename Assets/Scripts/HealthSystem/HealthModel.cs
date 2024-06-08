using UnityEngine;
using UnityEngine.Events;


public class HealthModel : MonoBehaviour
{
    [SerializeField] int currentHealth;
    [SerializeField] int maxHealth;
    [SerializeField] DamageNumberSystem numberPrefab;
    [SerializeField] protected Vector3 damageNumberOffSetPosition;


#region public properties
    public int MaxHealth
    {
    get => maxHealth;

        protected set
        {
            if (maxHealth < 1) 
            {
                maxHealth = 1; 
            }

            maxHealth = value;
        }
    }

    public int CurrentHealth
    {
        get => currentHealth;
        protected set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }

    public DamageNumberSystem NumberPrefab
    {
        get => numberPrefab;
        set => numberPrefab = value;
    }


#endregion

#region Events
        public UnityEvent onDeath;
        public UnityEvent<HealthModel, int> onDamageTaken;
        public UnityEvent<HealthModel, int> onHeal;
#endregion
}
