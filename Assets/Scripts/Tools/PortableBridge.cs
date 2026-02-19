using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PortableBridge : ToolPlacedObject
{
    public float Length { get; private set; }

    public void Configure(float length)
    {
        Length = Mathf.Max(0.5f, length);

        BoxCollider2D collider2D = GetComponent<BoxCollider2D>();
        collider2D.isTrigger = false;
        collider2D.size = new Vector2(Length, 0.35f);
        collider2D.offset = Vector2.zero;
    }
}
