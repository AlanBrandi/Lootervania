using System.Collections;
using UnityEngine;

public class FlyingEnemyBehaviour : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float returnDelay = 2f; 

    private Transform player;
    private Vector2 startPosition;
    private bool isChasing = false;
    private bool isReturning = false;
    private bool isKnockback = false; 
    private Rigidbody2D rb;
    private Knockback knockbackScript; 

    private void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        knockbackScript = GetComponent<Knockback>(); 
    }

    private void Update()
    {
        if (isKnockback) return;

        if (isChasing)
        {
            ChasePlayer();
        }
        else if (isReturning)
        {
            ReturnToStart();
        }
        else
        {
            Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("PlayerLayer"));
            if (playerCollider != null)
            {
                player = playerCollider.transform;
                isChasing = true;
            }
        }
    }

    private void ChasePlayer()
    {
        if (player == null)
        {
            isChasing = false;
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;

        if (Vector2.Distance(transform.position, player.position) > detectionRadius)
        {
            StartCoroutine(ReturnToStartCoroutine());
        }
    }

    private void ReturnToStart()
    {
        Vector2 direction = (startPosition - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;

        if (Vector2.Distance(transform.position, startPosition) < 0.1f)
        {
            isReturning = false;
            player = null;
            rb.velocity = Vector2.zero; 
        }
    }

    private IEnumerator ReturnToStartCoroutine()
    {
        isChasing = false;
        isReturning = true;

        yield return new WaitForSeconds(returnDelay);

        isReturning = true;
    }

    public void StopMovement()
    {
        isKnockback = true;
        rb.velocity = Vector2.zero;
    }

    public void ResumeMovement()
    {
        isKnockback = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
