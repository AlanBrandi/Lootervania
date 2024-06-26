using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats_", menuName = "Weapon/WeaponStats", order = 1)]
public class SOWeaponStats : ScriptableObject
{
    public int damage;
    public float reloadTime;
    public float fireRate;
    public int maxAmmoAmount;
}
