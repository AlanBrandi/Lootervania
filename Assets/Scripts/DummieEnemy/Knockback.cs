using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] private float _knockBackTime = .25f;
    [SerializeField] private float _knockBackForce = 50f;

    private Rigidbody2D _rb;

    private bool _isKnockBacking;
    private float _timer;

    private void Awake() 
    {
        _rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        if(_isKnockBacking)
        {
            _timer += Time.deltaTime;

            if(_timer >= _knockBackTime)
            {
                _rb.velocity = new Vector2(0f, 0f);
                _rb.angularVelocity = 0f;
                _isKnockBacking = false;
            }
        }
    }

    public void StartKnockBack(Vector2 direction)
    {
        _isKnockBacking = true;
        _timer = 0f;
        _rb.AddForce(direction * _knockBackForce, ForceMode2D.Impulse);
    }
}
