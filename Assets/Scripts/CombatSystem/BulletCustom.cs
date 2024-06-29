using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Pool.Core;

public class BulletCustom : Bullet
{
    private int bulletDamage;
    
    private float speed;
    private float lifetime;
    private float elapsedTime;

    private Rigidbody2D rb;


    private Vector2 direction;

    private bool isPiercingShoot;
    private int  MaxPiercingShoots;

    private bool isRecochetShoot;
    private int recochetAmount;
    
    private bool isBulletGetBigByTime;
    private float maxBulletSize;
    private Vector3 initialSize;
    
    [SerializeField] private SOBulletStats bulletStats;

    private void Awake()
    {
        initialSize = transform.localScale;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(!gameObject.activeSelf) return;
        Move();
        LifeTimeCount();
    }

    private void Move()
    {
        rb.velocity = direction * speed * Time.deltaTime;
    }

    private void LifeTimeCount()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= lifetime)
        {
            OnBulletDestroy();
        }

        if (isBulletGetBigByTime)
        {
            float scalePercentage = elapsedTime / lifetime;
            float currentScale = Mathf.Lerp(1f, maxBulletSize, scalePercentage);

            transform.localScale = new Vector3(currentScale, currentScale, 1f);
        }
    }

    public override void Act()
    {
        OnShoot();
    }

    public override void OnBulletCollide(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<HealthController>().ReduceHealth(bulletDamage);
            
            //Piercing
            if(!isPiercingShoot) OnBulletDestroy();
            
            MaxPiercingShoots--;
            if (MaxPiercingShoots < 0)
            {
                OnBulletDestroy();
            }

        }

        
        
        if (other.CompareTag("Level"))
        {
            
            if (!isRecochetShoot) return;
            
            if (recochetAmount <= 0)
            {
                OnBulletDestroy();
                return;
            }
            
            recochetAmount--;

            ContactPoint2D[] contacts = new ContactPoint2D[10]; 
            int contactCount = other.GetContacts(contacts);

            if (contactCount > 0)
            {

                Vector2 normal = Vector2.zero;
                foreach (ContactPoint2D contact in contacts)
                {
                    normal = contact.normal;
                    break;
                }
                Vector2 newDirection = Vector2.Reflect(rb.velocity.normalized, normal.normalized);
                direction = newDirection;
            }
        }
    }

    public override void OnShoot()
    {
    }

    public override void OnBulletDestroy()
    {
        if(!gameObject.activeSelf) return;
        PoolManager.ReleaseObject(gameObject);
    }

    public override void Initialize(int damage)
    {

        transform.localScale = initialSize;
        
        speed = bulletStats.bulletSpeed;
        lifetime = bulletStats.lifeTime;
        elapsedTime = 0;
        
        
        bulletDamage = damage;

        direction = transform.right;
        
        
        //Piercing
        isPiercingShoot = bulletStats.isPiercingShoot;
        MaxPiercingShoots = bulletStats.MaxPiercingShoots;
        
        //Recochet
        isRecochetShoot = bulletStats.IsRecochetShoot;
        recochetAmount = bulletStats.RecochetAmount;
        
        //IsShootGetBigByTime
        isBulletGetBigByTime = bulletStats.isShootGetBigByTime;
        maxBulletSize = bulletStats.MaxBulletSize;
    }

    private void OnBecameInvisible()
    {
        OnBulletDestroy();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnBulletCollide(other);
    }
}
