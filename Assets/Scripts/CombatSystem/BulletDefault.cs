using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Pool.Core;

public class BulletDefault : Bullet
{
    private float speed;
    private float lifetime;
    private float elapsedTime = 0;
    private Rigidbody2D rb;
    private float bulletDamage;
    
    [SerializeField] private SOBulletStats bulletStats;
    private void Update()
    {
        rb.velocity = transform.right * speed;
        elapsedTime = Time.deltaTime;
        if (elapsedTime > lifetime)
        {
            OnBulletDestroy();
        }
    }

    public override void Act()
    {
        OnShoot();
    }

    public override void OnBulletCollide(Collider2D other, Vector2 direction)
    {
        OnBulletDestroy();
    }

    public override void OnShoot()
    {
    }

    public override void OnBulletDestroy()
    {
        PoolManager.ReleaseObject(gameObject);
    }

    public override void Initialize(float damage, Vector3 bulletSize)
    {
        rb = GetComponent<Rigidbody2D>();
        speed = bulletStats.bulletSpeed;
        lifetime = bulletStats.lifeTime;
        bulletDamage = damage;
    }

    private void OnBecameInvisible()
    {
        OnBulletDestroy();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnBulletCollide(other, transform.right);
    }
}
