using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Weapon : MonoBehaviour
{
    public abstract void Fire();

    public float damage;

    public float reloadTime;

    public float fireRate;

    public int maxAmmoAmount;
    
    public int currentAmmoAmount;

    public bool canShoot;
}
