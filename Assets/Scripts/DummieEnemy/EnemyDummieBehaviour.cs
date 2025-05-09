using System;
using UnityEngine;
using System.Collections;
using Utilities.Pool.Core;

public class EnemyDummieBehaviour : MonoBehaviour
{
    
    //Colocar o inimigo apontando a arma para o player
    [Header("Dummie behaviour")]
    [SerializeField] private bool IsEnemyAgressive;
    [SerializeField] private bool canMove;
    [SerializeField] private bool IsMoving = false;

    [Header("Movement configuration")]
    [SerializeField] private float maxDistance;
    [SerializeField] private float speed;

    [Header("Shoot configuration")] 
    [SerializeField] private GameObject prefabBullet;
    [SerializeField] private float timeToShoot;
    [SerializeField] private float bulletSpeed;

    private Vector2 startPosition;
    private Vector2 direction;
    private Rigidbody2D rb;
    private Transform target;
    private Coroutine fireCoroutine;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = GetComponentInParent<Rigidbody2D>();
        startPosition = transform.position;
        direction = Vector2.right;
    }

    private void Update()
    {
        if (IsEnemyAgressive && target)
        {
            if (fireCoroutine == null)
            {
                fireCoroutine = StartCoroutine(FireRoutine());
            }
        }
        else
        {
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
                fireCoroutine = null;
            }
        }
    }

    private void FixedUpdate()
    {
        if(!canMove) return;

        if (!IsMoving)
        {
            rb.velocity = Vector2.zero;
            return;
        } 
        
        float distanceFromStart = transform.position.x - startPosition.x;
        if (distanceFromStart > maxDistance)
        {
            transform.position = new Vector2(startPosition.x + maxDistance, transform.position.y);
            direction = Vector2.left;
        }
        else if (distanceFromStart < -maxDistance)
        {
            transform.position = new Vector2(startPosition.x - maxDistance, transform.position.y);
            direction = Vector2.right;
        }
        rb.velocity = direction * speed;
    }

    public void RespawnDummie()
    {
        PoolManager.SpawnObject(this.gameObject, transform.position, transform.rotation);
    }

    IEnumerator FireRoutine()
    {
        while (true)
        {
            if (target && prefabBullet)
            {
                ShootTarget();
            }
            yield return new WaitForSeconds(timeToShoot);
        }
    }

    private void ShootTarget()
    {
        var bullet = PoolManager.SpawnObject(prefabBullet, transform.position, Quaternion.identity);
        if(bullet == null) return;
        
        Vector2 shootDirection = (target.position - transform.position).normalized;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        Rigidbody2D rb2d = bullet.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.velocity = shootDirection * bulletSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IsMoving = false;
            target = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IsMoving = true;
            target = null;
        }
    }

}
