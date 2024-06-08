using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LocalInputController : InputControllerBase
{
    private PlayerInput _playerInput;

    public override float Horizontal => enabled ? _playerInput.actions["Move"].ReadValue<Vector2>().x : 0f;
    
    public override float Vertical => enabled ? _playerInput.actions["Move"].ReadValue<Vector2>().y : 0f;
    
    private void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
    }
    
    private void Update()
    {
        if (!enabled)
            return;
        
        if(GetDashInput())
            OnDash?.Invoke();
        
        GetJumpInput();
    }
    

    private bool GetDashInput()
    {
        if (enabled)
            return _playerInput.actions["Dash"].triggered;

        return false;
    }
    
    private void GetJumpInput()
    {
        if (enabled)
        {
            if (_playerInput.actions["JumpDown"].triggered)
                OnJumpDown?.Invoke();

            if (_playerInput.actions["Jump"].ReadValue<float>() <= 0.1f)
                OnJumpUp?.Invoke();
        }
    }
}