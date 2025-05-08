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
    [SerializeField] public GameObject stickyGameObject;
    public GameObject StickyGameObject
    {
        get { return stickyGameObject; }
        set { stickyGameObject = value; }
    }
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
    [SerializeField] public Material auraMaterial;
    public Material AuraMaterial
    {
        get { return auraMaterial; }
        set { auraMaterial = value; }
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

    [Space]
    public bool isPullShot;
    [SerializeField] public float pullShotChance;

    [SerializeField] public GameObject pullGameObject;
    public GameObject PullGameObject
    {
        get { return pullGameObject; }
        set { pullGameObject = value; }
    }
    [SerializeField] public GameObject pullFX;
    public GameObject PullFX
    {
        get { return pullFX; }
        set { pullFX = value; }
    }
    public float PullShotChance
    {
        get { return pullShotChance; }
        set { pullShotChance = value; }
    }
    [SerializeField] public float pullStrength;
    public float PullStrength
    {
        get { return pullStrength; }
        set { pullStrength = value; }
    }
    [SerializeField] public float maxPullDistance;
    public float MaxPullDistance
    {
        get { return maxPullDistance; }
        set { maxPullDistance = value; }
    }
    [SerializeField] public float maxPullTime;
    public float MaxPullTime
    {
        get { return maxPullTime; }
        set { maxPullTime = value; }
    }
    [SerializeField] public float pullDamage;
    public float PullDamage
    {
        get { return pullDamage; }
        set { pullDamage = value; }
    }
    [SerializeField] public float pullDamageInterval;
    public float PullDamageInterval
    {
        get { return pullDamageInterval; }
        set { pullDamageInterval = value; }
    }
    [Space]
    [Tooltip("Doesn't work. Don't activate.")]
    public bool isBouncyShot;
    [SerializeField] public PhysicsMaterial2D bouncyMaterial;
    public PhysicsMaterial2D BouncyMaterial
    {
        get { return bouncyMaterial; }
        set { bouncyMaterial = value; }
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
        if (selectedPerks.Contains("ExplosiveShot"))
        {
            isExplosive = true;
        }
        if (selectedPerks.Contains("PullShot"))
        {
            isPullShot = true;
        }
        /*if(selectedPerks.Contains("BouncyShot"))
        {
            isBouncyShot = true;
        }*/
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
        isExplosive = false;
        isPullShot = false;
        //isBouncyShot = false;
    }

    [Space]
    [Header("Explosion Settings")]
    public bool isExplosive;
    public float explosionRadius;
    public int explosionDamage;
    public LayerMask damageableLayers;
}
