using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class PortableCrate : ToolPlacedObject
{
    [SerializeField] private float maxSupportedMass = 120f;
    [SerializeField] private float maxPushSpeed = 3f;

    private Rigidbody2D _rb;

    public void Configure(float supportedMass, float pushSpeedLimit)
    {
        maxSupportedMass = supportedMass;
        maxPushSpeed = pushSpeedLimit;

        _rb = GetComponent<Rigidbody2D>();
        _rb.mass = Mathf.Max(1f, supportedMass * 0.02f);
        _rb.linearDamping = 1.25f;
        _rb.angularDamping = 3f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        BoxCollider2D collider2D = GetComponent<BoxCollider2D>();
        collider2D.size = new Vector2(0.95f, 0.95f);
    }

    private void FixedUpdate()
    {
        if (_rb == null)
            return;

        float clampedX = Mathf.Clamp(_rb.linearVelocity.x, -maxPushSpeed, maxPushSpeed);
        _rb.linearVelocity = new Vector2(clampedX, _rb.linearVelocity.y);
    }
}
