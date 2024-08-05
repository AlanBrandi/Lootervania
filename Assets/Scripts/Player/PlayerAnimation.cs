using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    private PlayerMovement _playerMovement;
    private Rigidbody2D _RB;
    
    private bool isDashing = false;
    private bool isJumping = false;
    private bool isWallJumping = false;

    public ParticleSystem runDust;  

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

        //Debug.Log(_RB.velocity.magnitude);
        if (_RB.velocity.magnitude > 0.2f && _playerMovement.CanJump())
        {
            Debug.Log("j");
            CreateRunDust();
        }
        else
        {
            Debug.Log("n");
            StopRunDust();
        }
        //Vector3 newVelocity = runDust.velocityOverLifetime;
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

    #region EFFECTS
    private void CreateDust(GameObject dust)
    {
        Vector3 dustPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 1.261f, gameObject.transform.position.z);
        Instantiate(dust, dustPosition, Quaternion.identity);
    }
    private void CreateRunDust()
    {
        runDust.Play();
    }

    private void StopRunDust()
    {
        runDust.Stop();
    }
    #endregion
}
