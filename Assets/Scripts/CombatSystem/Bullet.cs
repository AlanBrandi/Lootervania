using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public abstract void Initialize(int damage);
    public abstract void Act();
    public abstract void OnBulletCollide();
    public abstract void OnShoot();
    public abstract void OnBulletDestroy();
}
