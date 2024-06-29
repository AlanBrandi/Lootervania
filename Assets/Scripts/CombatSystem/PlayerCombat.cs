using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    //EXISTE UM SUPER B.O
    //O Jeito que o thaigo fez o personagem virar com o scale da algumas merdas, tem q arrumar (anda para esquerda e aponta a arma lá)

    [SerializeField] private InputActionReference shootInput;
    [SerializeField] private InputActionReference switchWeapon;
    
    [Header("View")]
    [SerializeField] private GameObject weaponHandle;
    private Vector2 mousePos;

    [Header("Inventory")]
    [SerializeField] private Weapon[] rangeWeapons;
    private int currentWeapon;

    [SerializeField] private Weapon meleeWeapon;

    private void Awake()
    {
        shootInput.action.performed += Shoot;
        switchWeapon.action.performed += SwitchWeaponOnperformed;
    }

    private void SwitchWeaponOnperformed(InputAction.CallbackContext obj)
    {
        Debug.Log("Switch weapon");
        float value = obj.ReadValue<float>();
        bool isInverted = value < 0;
        int index = isInverted ? currentWeapon - 1 : currentWeapon + 1;
        currentWeapon = Mathf.Clamp(index, 0, rangeWeapons.Length - 1);
    }

    private void Shoot(InputAction.CallbackContext obj)
    {
        Debug.Log("Shoot");
        rangeWeapons[currentWeapon].Fire();
    }

    private void Update()
    {
        HandleWeaponRotation();
    }
    private void HandleWeaponRotation()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        float AngleRad = Mathf.Atan2(mousePos.y - weaponHandle.transform.position.y, mousePos.x - weaponHandle.transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;

        weaponHandle.transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
    }
}
