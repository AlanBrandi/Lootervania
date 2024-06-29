using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private InputActionReference shootInput;
    [SerializeField] private InputActionReference switchWeapon;
    
    [Header("View")]
    [SerializeField] private GameObject weaponHandle;
    private Vector2 mousePos;

    [Header("Inventory")]
    [SerializeField] private Weapon[] rangeWeapons;
    private int currentWeapon;

    private bool isShooting;

    private void Awake()
    {
        shootInput.action.performed += Shoot;
        shootInput.action.canceled += ShootDisable;
        switchWeapon.action.performed += SwitchWeaponOnperformed;
    }

    private void Shoot(InputAction.CallbackContext obj)
    {
        isShooting = true;
    }

    private void ShootDisable(InputAction.CallbackContext obj)
    {
        isShooting = false;
    }

    private void SwitchWeaponOnperformed(InputAction.CallbackContext obj)
    {
        float value = obj.ReadValue<float>();
        bool isInverted = value < 0;
        int index = isInverted ? currentWeapon - 1 : currentWeapon + 1;
        currentWeapon = Mathf.Clamp(index, 0, rangeWeapons.Length - 1);
    }

    private void Update()
    {
        HandleWeaponRotation();

        if (isShooting)
        {
            rangeWeapons[currentWeapon].Fire();
        }
    }

    private void HandleWeaponRotation()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        float AngleRad = Mathf.Atan2(mousePos.y - weaponHandle.transform.position.y, mousePos.x - weaponHandle.transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;

        weaponHandle.transform.rotation = Quaternion.Euler(0, 0, AngleDeg);
    }
}
