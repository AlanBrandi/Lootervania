using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public PlayerData Data;

	#region COMPONENTS
	public Rigidbody2D RB { get; private set; }

	[SerializeField] private GameObject turnableGameObject;

	public static PlayerMovement Instance;
	#endregion

	#region STATE PARAMETERS
	public bool IsFacingRight { get; private set; }
	public bool IsJumping { get; private set; }
	public bool IsWallJumping { get; private set; }
	public bool IsDashing { get; private set; }
	public bool IsSliding { get; private set; }
	public bool ExternalToolControlActive { get; private set; }
	public float LastOnGroundTime { get; private set; }
	public float LastOnWallTime { get; private set; }
	public float LastOnWallRightTime { get; private set; }
	public float LastOnWallLeftTime { get; private set; }
	public float LastOnWallBack { get; private set; }

	private InputControllerBase _inputController = null;


    private CameraFollowObject _cameraFollowObject;
	private float _fallSpeedYDampingChangeThreshold;
	private bool _isWallJumpBackWall;
	private bool _isJumpCut;
	private bool _isJumpFalling;

	private float _wallJumpStartTime;
	private int _lastWallJumpDir;

	private int _dashesLeft;
	private bool _dashRefilling;
	private Vector2 _lastDashDir;
	private bool _isDashAttacking;
	private Knockback _knockback;

	private TrailRenderer _trailRenderer;

	public GameObject landDust;
	public GameObject jumpDust;
	private PlayerAnimation playerAnimation;

	#endregion

	#region INPUT PARAMETERS
	public float LastPressedJumpTime { get; private set; }
	public float LastPressedDashTime { get; private set; }
	#endregion

	#region CHECK PARAMETERS
	[Header("Checks")]
	[SerializeField] private Transform _groundCheckPoint;
	[SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	[Space(5)]
	[SerializeField] private Transform _frontWallCheckPoint;
	[SerializeField] private Transform _backWallCheckPoint;
	[SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
	#endregion

	#region LAYERS & TAGS
	[Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
	#endregion

	#region CAMERAS
	[Header("Cameras")]
	[SerializeField] private GameObject _cameraFollowGO;
	#endregion

	private void Awake()
	{
		if (Instance)
		{
			Destroy(this);
		}
		else
		{
			Instance = this;
		}


		RB = GetComponent<Rigidbody2D>();
		_trailRenderer = GetComponent<TrailRenderer>();
		_inputController = GetComponent<LocalInputController>();
		_inputController.OnJumpDown += OnJumpInput;
		_inputController.OnJumpUp += OnJumpUpInput;
		_inputController.OnDash += OnDashInput;
		playerAnimation = GetComponent<PlayerAnimation>();
	}

	private void Start()
	{
		SetGravityScale(Data.gravityScale);
		IsFacingRight = true;

        _knockback = GetComponent<Knockback>();
		_cameraFollowObject = _cameraFollowGO.GetComponent<CameraFollowObject>();
		_fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
	}

	private void Update()
	{
		#region TIMERS
		LastOnGroundTime -= Time.deltaTime;
		LastOnWallTime -= Time.deltaTime;
		LastOnWallRightTime -= Time.deltaTime;
		LastOnWallLeftTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		LastPressedDashTime -= Time.deltaTime;
		#endregion

		#region INPUT HANDLER

		if (_inputController.Horizontal != 0)
			CheckDirectionToFace(_inputController.Horizontal > 0);

		#endregion

		if (ExternalToolControlActive)
		{
			return;
		}

		#region COLLISION CHECKS
		if (!IsDashing && !IsJumping)
		{
			if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
			{
				if (LastOnGroundTime < -0.1f)
				{
					CreateDust(landDust);
					//landAnimation
				}

				LastOnGroundTime = Data.coyoteTime;
			}

			bool frontWallCheck = Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer);
			bool backWallCheck = Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer);

			if (!IsWallJumping)
			{
				if (frontWallCheck)
				{
					_isWallJumpBackWall = false;
					if (IsFacingRight)
						LastOnWallRightTime = Data.coyoteTime;
					else
						LastOnWallLeftTime = Data.coyoteTime;
				}

				if (backWallCheck)
				{
					_isWallJumpBackWall = true;
					if (!IsFacingRight)
						LastOnWallRightTime = Data.coyoteTime;
					else
						LastOnWallLeftTime = Data.coyoteTime;
				}
			}

			LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
		}
		#endregion

		#region JUMP CHECKS
		if (IsJumping && RB.linearVelocity.y < 0)
		{
			IsJumping = false;

			_isJumpFalling = true;
		}

		if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
		{
			IsWallJumping = false;
		}

		if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
		{
			_isJumpCut = false;

			_isJumpFalling = false;
		}

		if (!IsDashing)
		{
			if (CanJump() && LastPressedJumpTime > 0 && !CanWallJump())
			{
				IsJumping = true;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				Jump();
				CreateDust(jumpDust);
				
				//call jump animation
			}
			else if (CanWallJump() && LastPressedJumpTime > 0)
			{
				if (Data.doTurnOnWallJump && !_isWallJumpBackWall) Turn();
				IsWallJumping = true;
				IsJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;

				_wallJumpStartTime = Time.time;
				_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

				WallJump(_lastWallJumpDir);
			}
		}
		#endregion

		#region DASH CHECKS
		if (CanDash() && LastPressedDashTime > 0)
		{
			_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;


			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;
			_isJumpCut = false;

			StartCoroutine(nameof(StartDash), _lastDashDir);
		}
		#endregion

		#region SLIDE CHECKS
		if (CanSlide() && ((LastOnWallLeftTime > 0 && _inputController.Horizontal < 0) || (LastOnWallRightTime > 0 && _inputController.Horizontal > 0)))
			IsSliding = true;
		else
			IsSliding = false;
		#endregion

		#region GRAVITY
		if (!_isDashAttacking)
		{
			if (IsSliding)
			{
				SetGravityScale(0);
			}
			else if (RB.linearVelocity.y < 0 && _inputController.Vertical < 0)
			{
				SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
				RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFastFallSpeed));
			}
			else if (_isJumpCut)
			{
				SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
				RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
			}
			else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
			{
				SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
			}
			else if (RB.linearVelocity.y < 0)
			{
				SetGravityScale(Data.gravityScale * Data.fallGravityMult);
				RB.linearVelocity = new Vector2(RB.linearVelocity.x, Mathf.Max(RB.linearVelocity.y, -Data.maxFallSpeed));
			}
			else
			{
				SetGravityScale(Data.gravityScale);
			}
		}
		else
		{
			SetGravityScale(0);
		}
		#endregion

		#region CAMERA CHECKS
		if (RB.linearVelocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
		{
			CameraManager.instance.LerpYDamping(true);
		}
		if(RB.linearVelocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
		{
			CameraManager.instance.LerpedFromPlayerFalling = false;
			CameraManager.instance.LerpYDamping(false);
		}
		#endregion
	}


	private void FixedUpdate()
	{
		if (ExternalToolControlActive)
			return;

		if (!IsDashing)
		{
			if (!IsWallJumping)
			{
				Run(Data.wallJumpRunLerp);
				return;
			}
			
			if(_knockback == null) return;
			if(!_knockback._isKnockBacking)
				Run(1);
		}
		if(_isDashAttacking)
		{
			Run(Data.dashEndRunLerp);
		}

		if (IsSliding)
			Slide();
	}

	#region INPUT CALLBACKS
	private void OnJumpInput()
	{
		LastPressedJumpTime = Data.jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
	}

	public void OnDashInput()
	{
		LastPressedDashTime = Data.dashInputBufferTime;
	}

	#endregion

	#region GENERAL METHODS
	public void SetGravityScale(float scale)
	{
		RB.gravityScale = scale;
	}

	public void EnableExternalToolControl()
	{
		ExternalToolControlActive = true;
		IsDashing = false;
		IsJumping = false;
		IsWallJumping = false;
		IsSliding = false;
		_isDashAttacking = false;
		_trailRenderer.emitting = false;
		LastPressedDashTime = 0f;
		LastPressedJumpTime = 0f;
	}

	public void DisableExternalToolControl()
	{
		ExternalToolControlActive = false;
		SetGravityScale(Data.gravityScale);
	}
	#endregion

	#region RUN METHODS
	private void Run(float lerpAmount)
	{
		float targetSpeed = _inputController.Horizontal * Data.runMaxSpeed;
		targetSpeed = Mathf.Lerp(RB.linearVelocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.linearVelocity.y) < Data.jumpHangTimeThreshold)
		{
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed *= Data.jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum
		if (Data.doConserveMomentum && Mathf.Abs(RB.linearVelocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.linearVelocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			accelRate = 0;
		}
		#endregion

		float speedDif = targetSpeed - RB.linearVelocity.x;

		float movement = speedDif * accelRate;

		RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}

	private void Turn()
	{
		if (IsWallJumping) return;

		Vector3 rotation = turnableGameObject.transform.rotation.eulerAngles;
		rotation.y += 180f;
		turnableGameObject.transform.rotation = Quaternion.Euler(rotation);
		playerAnimation.ChangeDustDirection();
		IsFacingRight = !IsFacingRight;
		_cameraFollowObject.CallTurn();
	}
	#endregion


	#region JUMP METHODS
	private void Jump()
	{
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		#region Perform Jump
		float force = Data.jumpForce;
		if (RB.linearVelocity.y < 0)
			force -= RB.linearVelocity.y;

		RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}

	private void WallJump(int dir)
	{
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		LastOnWallRightTime = 0;
		LastOnWallLeftTime = 0;

		#region Perform Wall Jump
		Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
		force.x *= dir;

		if (Mathf.Sign(RB.linearVelocity.x) != Mathf.Sign(force.x))
			force.x -= RB.linearVelocity.x;

		if (RB.linearVelocity.y < 0)
			force.y -= RB.linearVelocity.y;

		RB.AddForce(force, ForceMode2D.Impulse);
		#endregion
	}
	#endregion

	#region DASH METHODS
	private IEnumerator StartDash(Vector2 dir)
	{
		LastOnGroundTime = 0;
		LastPressedDashTime = 0;

		float startTime = Time.time;

		_dashesLeft--;
		_isDashAttacking = true;

		SetGravityScale(0);

		_trailRenderer.emitting = true;

		while (Time.time - startTime <= Data.dashAttackTime)
		{
			RB.linearVelocity = dir.normalized * Data.dashSpeed;
			yield return null;
		}

		startTime = Time.time;

		_isDashAttacking = false;

		SetGravityScale(Data.gravityScale);
		RB.linearVelocity = Data.dashEndSpeed * dir.normalized;

		while (Time.time - startTime <= Data.dashEndTime)
		{
			yield return null;
		}

		_trailRenderer.emitting = false;

		IsDashing = false;
	}

	private IEnumerator RefillDash(int amount)
	{
		_dashRefilling = true;
		yield return new WaitForSeconds(Data.dashRefillTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
	}
	#endregion

	#region OTHER MOVEMENT METHODS
	private void Slide()
	{
		if (RB.linearVelocity.y > 0)
		{
			RB.AddForce(-RB.linearVelocity.y * Vector2.up, ForceMode2D.Impulse);
		}

		float speedDif = Data.slideSpeed - RB.linearVelocity.y;
		float movement = speedDif * Data.slideAccel;
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		RB.AddForce(movement * Vector2.up);
	}
	#endregion


	#region CHECK METHODS
	public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

	public bool CanJump()
	{
		return LastOnGroundTime > 0 && !IsJumping;
	}

	private bool CanWallJump()
	{
		return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
			(LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
	}

	private bool IsWallClose()
	{
		return !IsWallJumping || (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) ||
				(LastOnWallLeftTime > 0 && _lastWallJumpDir == -1);
	}

	private bool CanJumpCut()
	{
		return IsJumping && RB.linearVelocity.y > 0;
	}

	private bool CanWallJumpCut()
	{
		return IsWallJumping && RB.linearVelocity.y > 0;
	}

	private bool CanDash()
	{
		if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
		{
			StartCoroutine(nameof(RefillDash), 1);
		}

		return _dashesLeft > 0;
	}


	public bool CanSlide()
	{
		if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
			return true;
		else
			return false;
	}
    #endregion

    #region EFFECTS
	private void CreateDust(GameObject dust)
    {
		Vector3 dustPosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y -1.261f, gameObject.transform.position.z);
		Instantiate(dust, dustPosition, Quaternion.identity);
	}
	#endregion

	#region EDITOR METHODS
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
		Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
	}
	#endregion
}
