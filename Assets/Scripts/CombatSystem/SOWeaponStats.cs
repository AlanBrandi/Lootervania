using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats_", menuName = "Weapon/WeaponStats", order = 1)]
public class SOWeaponStats : ScriptableObject
{
    public int damage;
    public float reloadTime;
    public float fireRate_BPS;
    public int maxAmmoAmount;
    public Vector3 bulletSize;
    public int amountShotsPerTrigger;

    [Space]
    [Header("Perks")]
    public bool isLessAmmoMorePower;
    public float ammoPowerMultiplier;
    
    //Atirar as vezes nn conta munição (pensar em um nome melhor que meu amigo)
    public bool isAmmoRandomCount;
    public float ammoRandomCountPercentage;
}
