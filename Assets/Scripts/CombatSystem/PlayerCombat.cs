using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerCombat : MonoBehaviour
{
    //EXISTE UM SUPER B.O
    //O Jeito que o thaigo fez o personagem virar com o scale da algumas merdas, tem q arrumar (anda para esquerda e aponta a arma lá)
    
    
    
    [Header("View")]
    [SerializeField] private GameObject weaponHandle;
    private Vector3 mousePos;

    [Header("Inventory")]
    [SerializeField] private Weapon[] rangeWeapons;
    private int currentWeapon;

    [SerializeField]private Weapon meleeWeapon;

    private void Update()
    {
        HandleWeaponRotation();

        if (Input.GetKey(KeyCode.Mouse0)) //Ele não funciona aqui pq nn ta no inputSystem novo, se quiser trocar da uma olhada no código do thaigo. (Vou fazer isso durante a semana)
        {
            rangeWeapons[currentWeapon].Fire();
        }

        // Por agora ele não dá loop na arma.
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            var index = currentWeapon - 1;
            currentWeapon = Mathf.Clamp(index, 0, rangeWeapons.Length);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            var index = currentWeapon + 1;
            currentWeapon = Mathf.Clamp(index, 0, rangeWeapons.Length);
        }
    }

    private void HandleWeaponRotation()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float AngleRad = Mathf.Atan2(mousePos.y - weaponHandle.transform.position.y, mousePos.x - weaponHandle.transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;

        weaponHandle.transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
    }
}
