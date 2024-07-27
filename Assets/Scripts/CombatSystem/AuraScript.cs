using UnityEngine;

public class AuraScript : MonoBehaviour
{
    private float auraSize;
    private int auraDamage;
    private float auraDamageInterval;
    private float nextDamageTime;
    [SerializeField] private SOBulletStats bulletStats;

    private void Start()
    {
        auraDamage = bulletStats.auraDamage;
        auraDamageInterval = bulletStats.auraDamageInterval;
         
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (Time.time >= nextDamageTime)
            {
                other.GetComponent<HealthController>().ReduceHealth((int)auraDamage, transform.right);
                nextDamageTime = Time.time + auraDamageInterval;
            }
        }
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
