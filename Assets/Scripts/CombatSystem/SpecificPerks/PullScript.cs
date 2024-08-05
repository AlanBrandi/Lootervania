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
    private GameObject[] enemies;
    [SerializeField] private GameObject pullFX;

    //private Animator anim;

    [SerializeField] private SOBulletStats bulletStats;

    private void Start()
    {
        //anim = GetComponent<Animator>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        maxPullDistance = bulletStats.maxPullDistance;
        pullStrength = bulletStats.pullStrength;
        maxPullTime = bulletStats.maxPullTime;
        Instantiate(pullFX, transform);
        Destroy(gameObject, maxPullTime+1f);
    }

    private void AnimEnd()
    {
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if(timer > maxPullTime)
        {
            //anim.SetBool("CanGoOff", true);
            timer = 0f; 
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
