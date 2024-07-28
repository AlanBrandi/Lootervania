using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = .5f;

    private int _turnTweenId;

    private PlayerMovement _player;

    private bool _isFacingRight;

    private void Awake() 
    {
        _player = _playerTransform.gameObject.GetComponentInParent<PlayerMovement>();
        _isFacingRight = _player.IsFacingRight;
    }

    private void Update() 
    {
        transform.position = _playerTransform.position;
    }

    public void CallTurn()
    {
        LeanTween.cancel(_turnTweenId);

        _turnTweenId = LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYRotationTime)
            .setEaseInOutSine()
            .id;
    }

    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;

        if (_isFacingRight)
        {
            return 180f;
        }
        else
        {
            return 0f;
        }
    }
}
