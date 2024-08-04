using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;


public enum WeaponType
{
    Pistol,
    Assault,
    Shotgun,
    Bazooka
}
public class WeaponFactory : MonoBehaviour
{
    public bool isEnabled;
    [SerializeField] private SOWeaponStats pistol;
    [SerializeField] private SOWeaponStats assault;
    [SerializeField] private SOWeaponStats shotgun;
    [SerializeField] private SOWeaponStats bazooka;

    [SerializeField] private Bullet customBullet;
    [SerializeField] private SOBulletStats bulletStatsPistol;
    [SerializeField] private SOBulletStats bulletStatsAssault;
    [SerializeField] private SOBulletStats bulletStatsShotgun;
    [SerializeField] private SOBulletStats bulletStatsBazooka;

    private bool isBazooka;

    private List<string> perks = new List<string>()
    {
        "LessAmmoMorePower",
        "AmmoRandomCount",
        "PiercingShoot",
        "RecochetShoot",
        "BulletGetBigByTime",
        "BoomerangShoot",
        "AuraShot",
        "ExplosiveShot",
        "StickyShot",
        "PullShot",
    };

    private List<string> availablePerks = new List<string>();

    public List<string> selectedPerks = new List<string>();
    void Start()
    {
        if (isEnabled)
            RerollAllPerksFromAllWeapons();
    }

    public void RerollAllPerksFromAllWeapons()
    {
       GenerateWeapon(WeaponType.Pistol); 
       GenerateWeapon(WeaponType.Assault); 
       GenerateWeapon(WeaponType.Shotgun); 
       GenerateWeapon(WeaponType.Bazooka); 
    }
    public void GenerateWeapon(Enum weapon)
    {
        isBazooka = false;
        switch (weapon)
        {
            case WeaponType.Pistol:
                WeaponAndBulletConfiguration(pistol, bulletStatsPistol);
                break;
            case WeaponType.Assault:
                WeaponAndBulletConfiguration(assault, bulletStatsAssault);
                break;
            case WeaponType.Shotgun:
                WeaponAndBulletConfiguration(shotgun, bulletStatsShotgun);
                break;
            case WeaponType.Bazooka:
                isBazooka = true;
                WeaponAndBulletConfiguration(bazooka, bulletStatsBazooka);
                break;
        }
    }

    private void WeaponAndBulletConfiguration(SOWeaponStats weaponStats, SOBulletStats bulletStats)
    {
        var newWeapon = ScriptableObject.CreateInstance<SOWeaponStats>();
        newWeapon = weaponStats;

        var newBullet = ScriptableObject.CreateInstance<SOBulletStats>();
        newBullet = bulletStats;

        //Set bullet type (por agora só tem uma e para o protótipo ta de boa)
        newWeapon.bullet = weaponStats.bullet;
        
        Debug.Log($"Generate weapon, {newWeapon.name}");

        RandomPerks();

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
        //Reset available perks list
        availablePerks.Clear();
        availablePerks.AddRange(perks);

        selectedPerks.Clear();

        if (isBazooka)
        {
            selectedPerks.Add("ExplosiveShot");
            availablePerks.Remove("ExplosiveShot");
        }

        System.Random rand = new System.Random();
        for (int i = 0; i < 2; i++)
        {
            int randomIndex = rand.Next(availablePerks.Count);
            selectedPerks.Add(availablePerks[randomIndex]);
            if (availablePerks[randomIndex] == "ExplosiveShot")
            {
                availablePerks.Remove("StickyShot");
            }
            else if (availablePerks[randomIndex] == "StickyShot")
            {
                selectedPerks.Add("ExplosiveShot");
                availablePerks.Remove("ExplosiveShot");
            }
            availablePerks.RemoveAt(randomIndex);
        }
    }
}