using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public abstract void Initialize(float damage);
    public abstract void Act();
    public abstract void OnBulletCollide(Collider2D other);
    public abstract void OnShoot();
    public abstract void OnBulletDestroy();
}
