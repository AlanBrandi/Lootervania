using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private InputActionReference shootInput;
    [SerializeField] private InputActionReference switchWeapon;
    [SerializeField] private InputActionReference lookInput;

    [Header("View")]
    [SerializeField] private GameObject weaponHandle;
    private Vector2 lookDirection;

    [Header("Inventory")]
    [SerializeField] private Weapon[] rangeWeapons;
    private int currentWeapon;

    private bool isShooting;
    private bool useMouse;
    private Vector2 lastMousePosition;

    private void Awake()
    {
        shootInput.action.performed += Shoot;
        shootInput.action.canceled += ShootDisable;
        switchWeapon.action.performed += SwitchWeaponOnperformed;
        lookInput.action.performed += LookPerformed;
        
        lastMousePosition = Mouse.current.position.ReadValue();
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

    private void LookPerformed(InputAction.CallbackContext obj)
    {
        lookDirection = obj.ReadValue<Vector2>();
        useMouse = false;  // Use controller input
    }

    private void Update()
    {
        if (Mouse.current.position.ReadValue() != lastMousePosition)
        {
            lastMousePosition = Mouse.current.position.ReadValue();
            useMouse = true;
        }
        else
        {
            useMouse = false;
        }

        
        HandleWeaponRotation();

        if (isShooting)
        {
            rangeWeapons[currentWeapon].Fire();
        }
    }

    private void HandleWeaponRotation()
    {
        if (useMouse)
        {
            // Handle rotation using mouse
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            Vector2 direction = mousePos - (Vector2)weaponHandle.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            weaponHandle.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Handle rotation using controller
            if (lookDirection != Vector2.zero)
            {
                float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
                weaponHandle.transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}
