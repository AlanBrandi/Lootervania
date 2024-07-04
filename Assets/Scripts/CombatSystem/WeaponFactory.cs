using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private SOWeaponStats pistol;
    [SerializeField] private SOWeaponStats assault;
    [SerializeField] private SOWeaponStats shotgun;
    [SerializeField] private SOWeaponStats bazzuka;

    [SerializeField] private Bullet customBullet;
    [SerializeField] private SOBulletStats bulletStats;
    
    private List<string> perks = new List<string>()
    {
        "LessAmmoMorePower",
        "AmmoRandomCount",
        "PiercingShoot",
        "RecochetShoot",
        "BulletGetBigByTime",
        "BoomerangShoot",
    };

    public List<string> selectedPerks = new List<string>();
    void Start()
    {
        GenerateWeapon(WeaponType.Pistol);
    }

    public void RerollAllPerksFromAllWeapons()
    {
       GenerateWeapon(WeaponType.Pistol); 
       GenerateWeapon(WeaponType.Assault); 
       GenerateWeapon(WeaponType.Shotgun); 
       GenerateWeapon(WeaponType.Bazzuka); 
    }
    public void GenerateWeapon(Enum weapon)
    {
        switch (weapon)
        {
            case WeaponType.Pistol:
                WeaponAndBulletConfiguration(pistol);
                break;
            case WeaponType.Assault:
                WeaponAndBulletConfiguration(assault);
                break;
            case WeaponType.Shotgun:
                WeaponAndBulletConfiguration(shotgun);
                break;
            case WeaponType.Bazzuka:
                WeaponAndBulletConfiguration(bazzuka);
                break;
        }
    }

    private void WeaponAndBulletConfiguration(SOWeaponStats weaponStats)
    {
        var newWeapon = ScriptableObject.CreateInstance<SOWeaponStats>();
        newWeapon = weaponStats;

        var newBullet = ScriptableObject.CreateInstance<SOBulletStats>();
        newBullet = bulletStats;
        
        
        //Set bullet type (por agora só tem uma e para o protótipo ta de boa)
        newWeapon.bullet = customBullet; 
        RandomPerks();
        
        Debug.Log($"Generate weapon, {newWeapon.name}");

        //Aqui ele da um replace no scriptableObject da arma
        newWeapon.DisableAllPerks();
        newWeapon.ActivatePerks(selectedPerks);
        
        newBullet.DisableAllPerks();
        newBullet.ActivatePerks(selectedPerks);
        
        
        //Depois disso a gente tem um scriptableObject pistol completamente novo e um bullet novo tmb
        //Agora só instanciar um rangedWeapon adicionando o newWeapon a bullet nn precisa
    }


    public void RandomPerks()
    {
        selectedPerks.Clear();
        System.Random rand = new System.Random();
        for (int i = 0; i < 2; i++)
        {
            int randomIndex = rand.Next(perks.Count);
            selectedPerks.Add(perks[randomIndex]);
        }
    }
}