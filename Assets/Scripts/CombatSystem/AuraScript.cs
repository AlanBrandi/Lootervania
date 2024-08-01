using System.Collections.Generic;
using UnityEngine;

public class AuraScript : MonoBehaviour
{
    private float sizeAura;
    private float auraDamage;
    private float auraDamageInterval;
    private float nextDamageTime;
    private List<Collider2D> collidersInTrigger = new List<Collider2D>();
    [SerializeField] private SOBulletStats bulletStats;

    private void Start()
    {
        auraDamage = bulletStats.auraDamage;
        auraDamageInterval = bulletStats.auraDamageInterval;
        sizeAura = bulletStats.sizeAura;
        transform.localScale = transform.localScale * sizeAura;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            collidersInTrigger.Add(other);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            collidersInTrigger.Remove(other);
        }
    }

    /*public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (Time.time >= nextDamageTime)
            {
                other.GetComponent<HealthController>().ReduceHealth((int)auraDamage, transform.right);
                nextDamageTime = Time.time + auraDamageInterval;
            }
        }
    }*/

    private void FixedUpdate()
    {
        if (Time.time >= nextDamageTime)
        {
            foreach (var collider in collidersInTrigger)
            {
                collider.GetComponent<HealthController>().ReduceHealthNoKnockback((int)auraDamage);
            }
            nextDamageTime = Time.time + auraDamageInterval;
        }
    }
}
