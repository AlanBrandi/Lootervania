using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletsStats_", menuName = "Weapon/BulletsStats", order = 1)]
public class SOBulletStats : ScriptableObject
{
    [Header("Default stats")]
    public float bulletSpeed;
    public float lifeTime;

    [Space]
    [Header("SimplePerks - Bullet Behaviour")]
    public bool IsRecochetShoot;
    [SerializeField] public int recochetAmount;
    public int RecochetAmount
    {
        get { return recochetAmount; }
        set { recochetAmount = value; }
    }

    [Space]

    public bool IsStickyShot;
    [SerializeField] public float maxStickyShotsTime;
    public float MaxStickyShotsTime
    {
        get { return maxStickyShotsTime; }
        set { maxStickyShotsTime = value; }
    }

    [Space]
    public bool isPiercingShoot;
    [SerializeField] public int maxPiercingShoots;
    public int MaxPiercingShoots
    {
        get { return maxPiercingShoots; }
        set { maxPiercingShoots = value; }
    }

    [Space]
    public bool isShootGetBigByTime;
    [SerializeField] public float maxBulletSize;
    public float MaxBulletSize
    {
        get { return maxBulletSize; }
        set { maxBulletSize = value; }
    }

    [Space]
    public bool isBoomerangShoot;
    [SerializeField] public float maxDistanceBoomerang;
    public float MaxDistanceBoomerang
    {
        get { return maxDistanceBoomerang; }
        set { maxDistanceBoomerang = value; }
    }

    [Space]
    public bool isAuraShot;
    [SerializeField] public GameObject auraGameObject;
    public GameObject AuraGameObject
    {
        get { return auraGameObject; }
        set { auraGameObject = value; }
    }
    [SerializeField] public float sizeAura;
    public float SizeAura
    {
        get { return sizeAura; }
        set { sizeAura = value; }
    }
    [Tooltip("Higher value is slower.")]
    [SerializeField] public float auraSpeedMod;
    public float AuraSpeedMod
    {
        get { return auraSpeedMod; }
        set { auraSpeedMod = value; }
    }
    [SerializeField] public float auraLifetimeMod;
    public float AuraLifetimeMod
    {
        get { return auraLifetimeMod; }
        set { auraLifetimeMod = value; }
    }
    [Tooltip("Higher value is weaker.")]
    [SerializeField] public float auraDamageMod;
    public float AuraDamageMod
    {
        get { return auraDamageMod; }
        set { auraDamageMod = value; }
    }
    public float auraDamage;
    public float AuraDamage
    {
        get { return auraDamage; }
        set { auraDamage = value; }
    }
    [SerializeField] public float auraDamageInterval;
    public float AuraDamageInterval
    {
        get { return auraDamageInterval; }
        set { auraDamageInterval = value; }
    }

    public void ActivatePerks(List<string> selectedPerks)
    {
        if (selectedPerks.Contains("PiercingShoot"))
        {
            isPiercingShoot = true;
        }
        if (selectedPerks.Contains("RecochetShoot"))
        {
            IsRecochetShoot = true;
        }
        if (selectedPerks.Contains("BulletGetBigByTime"))
        {
            isShootGetBigByTime = true;
        }
        if (selectedPerks.Contains("BoomerangShoot"))
        {
            isBoomerangShoot = true;
        }
        if (selectedPerks.Contains("AuraShot"))
        {
            isAuraShot = true;
        }
        if (selectedPerks.Contains("StickyShot"))
        {
            IsStickyShot = true;
        }
    }
//Ignora isso que ta pessimo
    public void DisableAllPerks()
    {
        isPiercingShoot = false;
        IsRecochetShoot = false;
        isShootGetBigByTime = false;
        isBoomerangShoot = false;
        isAuraShot = false;
        IsStickyShot = false;
    }

    [Space]
    [Header("Explosion Settings")]
    public bool isExplosive;
    public float explosionRadius;
    public int explosionDamage;
    public LayerMask damageableLayers;
}
