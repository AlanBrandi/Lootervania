using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;
    public void Shake(Vector2 diretion, float shakeForce)
    {
        cinemachineImpulseSource.GenerateImpulseWithVelocity(-diretion * shakeForce);
    }
}
