using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    private PlayerMovement _playerMovement;
    private Rigidbody2D _RB;
    
    private bool isDashing = false;
    private bool isJumping = false;
    private bool isWallJumping = false;

    void Start()
    {
        _RB = GetComponent<Rigidbody2D>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void FixedUpdate() 
    {
        UpdateAnimationParameters();
        CheckAndSetAnimationTriggers();
    }

    private void UpdateAnimationParameters()
    {
        _anim.SetFloat("Velocity", _RB.velocity.magnitude);
        _anim.SetBool("CanJump", _playerMovement.CanJump());
        _anim.SetBool("IsDashing", _playerMovement.IsDashing);
        _anim.SetBool("IsJumping", _playerMovement.IsJumping);
        _anim.SetBool("IsWallJumping", _playerMovement.IsWallJumping);
    }

    private void CheckAndSetAnimationTriggers()
    {
        UpdateAnimationTrigger(ref isDashing, _playerMovement.IsDashing, "Dash");
        UpdateAnimationTrigger(ref isJumping, !_playerMovement.CanJump(), "Jump");
        UpdateAnimationTrigger(ref isWallJumping, _playerMovement.IsWallJumping, "WallJump");
    }

    private void UpdateAnimationTrigger(ref bool currentState, bool newState, string triggerName)
    {
        if (currentState != newState)
        {
            currentState = newState;
            _anim.SetTrigger(triggerName);
        }
    }
}
