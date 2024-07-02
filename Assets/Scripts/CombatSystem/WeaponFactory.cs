using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Pistol,
    Assault,
    Shotgun,
    Bazzuka
}
public class WeaponFactory : MonoBehaviour
{
     public Dictionary<WeaponType, ScriptableObject> weaponDataDefault = new Dictionary<WeaponType, ScriptableObject>();

     [SerializeField] private ScriptableObject[] weaponDefaults;

    

    public void GenerateWeapon(Enum weapon)
    {
        switch (weapon)
        {
            case WeaponType.Pistol:
                
                break;
            case WeaponType.Assault:
                
                break;
            case WeaponType.Shotgun:
                
                break;
            case WeaponType.Bazzuka:
                
                break;
        }
    }
}
