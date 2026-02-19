using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerToolTraversal : MonoBehaviour
{
    [Header("Ladder")]
    [SerializeField] private float ladderClimbSpeed = 4.5f;
    [SerializeField] private float ladderHorizontalAssist = 1.5f;

    private Rigidbody2D _rb;
    private PlayerMovement _movement;
    private InputControllerBase _input;

    private PortableLadder _currentLadder;
    private bool _isClimbing;

    private PortableZipline _activeZipline;
    private float _ziplineProgress;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _movement = GetComponent<PlayerMovement>();
        _input = GetComponent<InputControllerBase>();
    }

    private void Update()
    {
        HandleZiplineRide();
        HandleLadderClimb();
    }

    public bool TryAttachZipline(PortableZipline zipline)
    {
        if (zipline == null || _activeZipline != null)
            return false;

        if (!zipline.TryReserve())
            return false;

        _activeZipline = zipline;
        _ziplineProgress = 0f;

        if (_movement != null)
            _movement.EnableExternalToolControl();

        _rb.gravityScale = 0f;
        _rb.linearVelocity = Vector2.zero;
        _rb.position = zipline.StartPoint;
        return true;
    }

    private void HandleZiplineRide()
    {
        if (_activeZipline == null)
            return;

        if (_activeZipline.Length <= 0.001f)
        {
            StopZiplineRide();
            return;
        }

        _ziplineProgress += (_activeZipline.TravelSpeed / _activeZipline.Length) * Time.deltaTime;
        Vector2 point = _activeZipline.GetPointByProgress(_ziplineProgress);
        _rb.position = point;
        _rb.linearVelocity = Vector2.zero;

        if (_ziplineProgress >= 1f)
        {
            StopZiplineRide();
        }
    }

    private void StopZiplineRide()
    {
        if (_activeZipline == null)
            return;

        _activeZipline.Release();
        _activeZipline = null;
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, -1.5f);

        if (_movement != null)
            _movement.DisableExternalToolControl();
    }

    private void HandleLadderClimb()
    {
        if (_activeZipline != null)
            return;

        float vertical = _input != null ? _input.Vertical : 0f;
        bool wantsClimb = _currentLadder != null && Mathf.Abs(vertical) > 0.1f;

        if (wantsClimb && !_isClimbing)
        {
            _isClimbing = true;
            if (_movement != null)
                _movement.EnableExternalToolControl();
        }

        if (!_isClimbing)
            return;

        if (_currentLadder == null)
        {
            StopClimbing();
            return;
        }

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StopClimbing();
            return;
        }

        _rb.gravityScale = 0f;
        float horizontal = _input != null ? _input.Horizontal : 0f;
        Vector2 ladderVelocity = new Vector2(horizontal * ladderHorizontalAssist, vertical * ladderClimbSpeed);
        _rb.linearVelocity = ladderVelocity;

        if (Mathf.Abs(vertical) <= 0.05f && Mathf.Abs(horizontal) <= 0.05f)
        {
            _rb.linearVelocity = Vector2.zero;
        }
    }

    private void StopClimbing()
    {
        _isClimbing = false;
        _rb.linearVelocity = Vector2.zero;

        if (_movement != null)
            _movement.DisableExternalToolControl();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PortableLadder ladder = other.GetComponent<PortableLadder>();
        if (ladder != null)
        {
            _currentLadder = ladder;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PortableLadder ladder = other.GetComponent<PortableLadder>();
        if (ladder != null && ladder == _currentLadder)
        {
            _currentLadder = null;
            if (_isClimbing)
                StopClimbing();
        }
    }
}
