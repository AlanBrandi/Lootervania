using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PullScript : MonoBehaviour
{
    private float maxPullDistance;
    private float pullStrength;
    private float maxPullTime;
    private float timer = 0;
    private float pullDamage;
    private float pullDamageInterval;
    private float nextDamageTime;
    private GameObject[] enemies;
    [SerializeField] private GameObject pullFX;

    private List<Collider2D> collidersInTrigger = new List<Collider2D>();

    //private Animator anim;

    [SerializeField] private SOBulletStats bulletStats;

    private void Start()
    {
        //anim = GetComponent<Animator>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        maxPullDistance = bulletStats.maxPullDistance;
        pullStrength = bulletStats.pullStrength;
        maxPullTime = bulletStats.maxPullTime;
        pullDamage = bulletStats.pullDamage;
        pullDamageInterval = bulletStats.pullDamageInterval;
        GameObject pullFXTemp = Instantiate(pullFX, transform);
        //pullFXTemp.transform.localScale = transform.localScale;
        Destroy(gameObject, maxPullTime+1f);
    }

    private void AnimEnd()
    {
        Destroy(gameObject);
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

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if(timer > maxPullTime)
        {
            //anim.SetBool("CanGoOff", true);
            timer = 0f; 
        }

        if (Time.time >= nextDamageTime)
        {
            foreach (var collider in collidersInTrigger)
            {
                collider.GetComponent<HealthController>().ReduceHealthNoKnockback((int)(pullDamage));
            }
            nextDamageTime = Time.time + pullDamageInterval;
        }

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= maxPullDistance)
            {
                Vector3 direction = (transform.position - enemy.transform.position).normalized;

                Rigidbody2D enemyRigidbody = enemy.GetComponent<Rigidbody2D>();
                if (enemyRigidbody != null)
                {
                    float strength = pullStrength * (1 - (distance / maxPullDistance));
                    enemyRigidbody.AddForce(direction * strength, ForceMode2D.Force);
                }
            }
        }
    }
}
