using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using Utilities.Pool.Core;

public class RangeWeapon : Weapon
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SOWeaponStats weaponStats;
    [SerializeField] private Bullet bullet;
   
    private float nextFireTime = 0f;
    
    private void Start()
    {
        InitializeWeapon();
    }

    public override void Fire()
    {
        if (!canShoot || Time.time < nextFireTime) return;
        
        if (currentAmmoAmount < 0)
        {
            var bulletTmp = PoolManager.SpawnObject(bullet.gameObject, spawnPoint.position, spawnPoint.rotation);
            bulletTmp.GetComponent<Bullet>().Initialize(damage); //Ta errado fazendo isso mas nn tem o que fazer por agora
            currentAmmoAmount--; 
            nextFireTime = Time.time + 1f / fireRate;
        }
        else
        {
            canShoot = false;
            Invoke(nameof(WeaponReload), reloadTime);   
        }
        
    }

    private void InitializeWeapon()
    {
        damage = weaponStats.damage;
        reloadTime = weaponStats.reloadTime;
        fireRate = weaponStats.fireRate;
        maxAmmoAmount = weaponStats.maxAmmoAmount;
        currentAmmoAmount = maxAmmoAmount;
        canShoot = true;
    }

    private void WeaponReload()
    {
        canShoot = true;
        currentAmmoAmount = maxAmmoAmount;
    }
}
