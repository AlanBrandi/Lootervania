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
    private int bulletDamage;
    
    [SerializeField] private SOBulletStats bulletStats;
    private void Update()
    {
        rb.velocity = transform.up * speed;
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

    public override void OnBulletCollide()
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

    public override void Initialize(int damage)
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        OnBulletCollide();
    }
}
