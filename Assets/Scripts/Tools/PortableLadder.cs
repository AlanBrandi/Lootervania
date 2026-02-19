using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PortableLadder : ToolPlacedObject
{
    [SerializeField] private float climbHeight = 3.5f;

    public float ClimbHeight => climbHeight;
    public Vector2Int SurfaceKey { get; private set; }

    public void Configure(float height, Vector2Int key)
    {
        climbHeight = height;
        SurfaceKey = key;

        BoxCollider2D collider2D = GetComponent<BoxCollider2D>();
        collider2D.isTrigger = true;
        collider2D.size = new Vector2(0.8f, climbHeight);
        collider2D.offset = Vector2.zero;
    }
}
