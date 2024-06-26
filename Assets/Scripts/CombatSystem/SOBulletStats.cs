using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletsStats_", menuName = "Weapon/BulletsStats", order = 1)]
public class SOBulletStats : ScriptableObject
{
    public float bulletSpeed;
    public float lifeTime;
}
