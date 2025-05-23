using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities.Pool.Core;
using Random = UnityEngine.Random;

public class RangeWeapon : Weapon
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SOWeaponStats weaponStats;

    [SerializeField] private GameObject AmmoPanel;
    [SerializeField] private GameObject ReloadPanel;
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private Slider sliderReload;

    [SerializeField] private InputActionReference reloadInput;

    private bool isReloading;
    private float timeSinceLastFire; 
    
    private Bullet bullet;

    public ParticleSystem shootFX;
    
    //Perks
    [Space]
    [Header("Perks")]
    [SerializeField] private bool isLessAmmoMorePower;
    [SerializeField] private float ammoPowerMultiplier;
    
    [SerializeField] private bool isAmmoRandomCount;
    [SerializeField] private float ammoRandomCountPercentage;

    private void Start()
    {
        reloadInput.action.performed += Reload;
        InitializeWeapon();
    }

    private void Update()
    {
        if (isReloading) return;
        

        timeSinceLastFire += Time.deltaTime;
    }

    private void Reload(InputAction.CallbackContext obj)
    {
        if (isReloading || currentAmmoAmount == maxAmmoAmount) return;
        OnStartReload();
        canShoot = false;
        Invoke(nameof(WeaponReload), reloadTime);
    }

    private void AddPerks()
    {
    }

    public override void Fire()
    {
        if (!canShoot) return;

        if (timeSinceLastFire >= 1f / fireRate)
        {
            ShootBullet(weaponStats.amountShotsPerTrigger);
        }
    }

    private void ShootBullet(int amountShotsPerTrigger)
    {
        if (currentAmmoAmount > 0)
        {
            int currentAmountShotsPerTrigger = amountShotsPerTrigger;
            damage = weaponStats.damage;
            Vector3 bulletSize = weaponStats.bulletSize;
            shootFX.Play();
            if (isLessAmmoMorePower)
            {
                float ammoRatio = (float)currentAmmoAmount / maxAmmoAmount;
                damage *= (int)(1 + ammoPowerMultiplier * (1 - ammoRatio));
            }
            float spreadAngle = 15f;
        float baseAngle = spawnPoint.rotation.eulerAngles.z;
        float startAngle = baseAngle - spreadAngle / 2f;
        float angleStep = spreadAngle / (currentAmountShotsPerTrigger - 1);

        while(currentAmountShotsPerTrigger > 0)
        {
            for (int i = 0; i < currentAmountShotsPerTrigger; i++)
            {
                var bulletTmp = PoolManager.SpawnObject(bullet.gameObject, spawnPoint.position, spawnPoint.rotation);
                if(bullet == null) continue;
                float angle = currentAmountShotsPerTrigger > 1 ? startAngle + i * angleStep : baseAngle;
                bulletTmp.transform.rotation = Quaternion.Euler(0, 0, angle);
                bulletTmp.GetComponent<Bullet>().Initialize(damage, bulletSize);
            }
            currentAmountShotsPerTrigger = 0;
        }
            
            if (!isAmmoRandomCount)
            {
                currentAmmoAmount--;
            }
            else
            {
                float percentage = ammoRandomCountPercentage / 100;
                if (Random.value < percentage)
                {
                    
                }
                else
                {
                    currentAmmoAmount--;
                }
            }
            timeSinceLastFire = 0f; 
            ammoText.text = $"{currentAmmoAmount}/{maxAmmoAmount}";
        }
        else
        {
            OnStartReload();
            canShoot = false;
            Invoke(nameof(WeaponReload), reloadTime);   
        }
    }

    private void InitializeWeapon()
    {
        bullet = weaponStats.bullet;
        damage = weaponStats.damage;    
        
        reloadTime = weaponStats.reloadTime;
        fireRate = weaponStats.fireRate_BPS;
        maxAmmoAmount = weaponStats.maxAmmoAmount;
        currentAmmoAmount = maxAmmoAmount;
        canShoot = true;
        
        isReloading = false; 
        timeSinceLastFire = 0;


        isLessAmmoMorePower = weaponStats.isLessAmmoMorePower;
        ammoPowerMultiplier = weaponStats.ammoPowerMultiplier;

        isAmmoRandomCount = weaponStats.isAmmoRandomCount;
        ammoRandomCountPercentage = weaponStats.ammoRandomCountPercentage;

        ammoText.text = $"{currentAmmoAmount}/{maxAmmoAmount}";
    }

    private void WeaponReload()
    {
        canShoot = true;
        currentAmmoAmount = maxAmmoAmount;
        OnReloadFinished();
    }
    
    private void OnStartReload()
    {
        if(gameObject.activeSelf)
            StartCoroutine(ReloadRoutine());
    }
    private void OnReloadFinished()
    {
        StopCoroutine(ReloadRoutine());
        ammoText.text = new string($"{currentAmmoAmount}/{maxAmmoAmount}");
    }
    
    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        ReloadPanel.SetActive(true);
        AmmoPanel.SetActive(false);

        float reloadProgress = 0f;
        while (reloadProgress < reloadTime)
        {
            reloadProgress += Time.deltaTime;
            sliderReload.value = reloadProgress / reloadTime;
            yield return null;
        }

        isReloading = false;
        ReloadPanel.SetActive(false);
        AmmoPanel.SetActive(true);
    }
}
