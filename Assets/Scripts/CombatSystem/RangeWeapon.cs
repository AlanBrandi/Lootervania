using UnityEngine;
using Utilities.Pool.Core;

public class RangeWeapon : Weapon
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private SOWeaponStats weaponStats;
    [SerializeField] private Bullet bullet;

    private bool isReloading;
    private float timeSinceLastFire; // Tempo decorrido desde o último disparo

    private void Start()
    {
        InitializeWeapon();
    }

    private void Update()
    {
        if (isReloading) return;

        // Atualiza o tempo decorrido desde o último disparo
        timeSinceLastFire += Time.deltaTime;
    }

    public override void Fire()
    {
        if (!canShoot) return;

        // Verifica se pode atirar com base no fireRate
        if (timeSinceLastFire >= 1f / fireRate)
        {
            ShootBullet();
        }
    }

    private void ShootBullet()
    {
        if (currentAmmoAmount > 0)
        {
            var bulletTmp = PoolManager.SpawnObject(bullet.gameObject, spawnPoint.position, spawnPoint.rotation);
            bulletTmp.GetComponent<Bullet>().Initialize(damage);
            currentAmmoAmount--;
            timeSinceLastFire = 0f; // Reinicia o tempo decorrido
        }
        else
        {
            canShoot = false;
            isReloading = true;
            Invoke(nameof(WeaponReload), reloadTime);   
        }
    }

    private void InitializeWeapon()
    {
        damage = weaponStats.damage;
        reloadTime = weaponStats.reloadTime;
        fireRate = weaponStats.fireRate_BPS;
        maxAmmoAmount = weaponStats.maxAmmoAmount;
        currentAmmoAmount = maxAmmoAmount;
        canShoot = true;
        
        isReloading = false; 
        timeSinceLastFire = 0;
    }

    private void WeaponReload()
    {
        canShoot = true;
        isReloading = false;
        currentAmmoAmount = maxAmmoAmount;
    }
}
