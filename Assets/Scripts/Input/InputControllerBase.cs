using UnityEngine;
using UnityEngine.Events;

public abstract class InputControllerBase : MonoBehaviour
{
    public UnityAction OnDash;
    public UnityAction OnJumpDown;
    public UnityAction OnJumpUp;
    public abstract float Horizontal { get; }
    public abstract float Vertical { get; }
}