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
    [SerializeField] public int maxBulletSize;
    
    public int MaxBulletSize
    {
        get { return maxBulletSize; }
        set { maxBulletSize = value; }
    }
    
    [Space]
    
    public bool isDoubleAmmoAmount; 
    [SerializeField] public float percentageRangeToChange;
    
    public float PercentageRangeToChange
    {
        get { return percentageRangeToChange; }
        set { percentageRangeToChange = value; }
    }

}
