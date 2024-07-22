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
    [Header("Explosion Settings")]
    public bool isExplosive;
    public float explosionRadius;
    public LayerMask damageableLayers;
}
