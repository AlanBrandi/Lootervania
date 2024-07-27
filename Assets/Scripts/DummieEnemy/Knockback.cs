using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    [SerializeField] private float _knockBackTime = .25f;
    [SerializeField] private float _knockBackForce = 50f;

    private Rigidbody2D _rb;
    private FlyingEnemyBehaviour _enemyBehaviour; 

    private bool _isKnockBacking;
    private float _timer;

    private void Awake() 
    {
        _rb = GetComponent<Rigidbody2D>();
        _enemyBehaviour = GetComponent<FlyingEnemyBehaviour>(); 
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
                if(_enemyBehaviour != null)
                   _enemyBehaviour.ResumeMovement(); 
            }
        }
    }

    public void StartKnockBack(Vector2 direction)
    {
        _isKnockBacking = true;
        _timer = 0f;
        if(_enemyBehaviour != null)
            _enemyBehaviour.StopMovement(); 
        _rb.AddForce(direction * _knockBackForce, ForceMode2D.Impulse);
    }
}
