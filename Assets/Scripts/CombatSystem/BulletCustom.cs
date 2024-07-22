using UnityEngine;
using Utilities.Pool.Core;

public class BulletCustom : Bullet
{
    private float bulletDamage;
    private float speed;
    private float lifetime;
    private float elapsedTime;

    private Rigidbody2D rb;
    private Vector2 direction;

    private bool isPiercingShoot;
    private int MaxPiercingShoots;

    private bool isRecochetShoot;
    private int recochetAmount;

    private bool isBulletGetBigByTime;
    private float maxBulletSize;
    private Vector3 initialSize;

    private bool isBoomerangShoot;
    private Vector2 ongoingDirection;
    private float maxDistance;

    private Transform playerTransform;
    private int hitCountPlayer = 0;

    // Parâmetros de explosão
    private bool isExplosive;
    private float explosionRadius;
    private LayerMask damageableLayers;

    [SerializeField] private SOBulletStats bulletStats;

    private void Awake()
    {
        initialSize = transform.localScale;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Vector2 ongoingDirection = direction;
    }

    private void FixedUpdate()
    {
        if (!gameObject.activeSelf) return;
        Move();
        LifeTimeCount();
    }

    private void Move()
    {
        rb.velocity = direction * speed * Time.deltaTime;
    }

    private void LifeTimeCount()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime)
        {
            OnBulletDestroy();
        }

        if (isBulletGetBigByTime)
        {
            float scalePercentage = elapsedTime / lifetime;
            float currentScale = Mathf.Lerp(1f, maxBulletSize, scalePercentage);

            transform.localScale = new Vector3(currentScale, currentScale, 1f);
            bulletDamage += (int)(0.2f * elapsedTime); // 20% pensar melhor depois
        }

        if (isBoomerangShoot)
        {
            if (elapsedTime >= lifetime / 2)
            {
                Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                direction = directionToPlayer;
            }
        }
    }

    public override void Act()
    {
        OnShoot();
    }

   public override void OnBulletCollide(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<HealthController>().ReduceHealth((int)bulletDamage);
            
            // Piercing
            if (!isPiercingShoot)
            {
                if(isExplosive)
                   Explode();
                else
                   OnBulletDestroy();   
            }
            
            MaxPiercingShoots--;
            if (MaxPiercingShoots < 0)
            {
                OnBulletDestroy();
            }
        }
        
        if (other.CompareTag("Level"))
        {
            if (!isRecochetShoot)
            {
                if(isExplosive)
                   Explode();
                else
                   OnBulletDestroy();   
            }
            
            if (recochetAmount <= 0)
            {
                OnBulletDestroy();
                return;
            }
            
            recochetAmount--;

            ContactPoint2D[] contacts = new ContactPoint2D[10]; 
            int contactCount = other.GetContacts(contacts);

            if (contactCount > 0)
            {
                Vector2 normal = Vector2.zero;
                foreach (ContactPoint2D contact in contacts)
                {
                    normal = contact.normal;
                    break;
                }
                Vector2 newDirection = Vector2.Reflect(rb.velocity.normalized, normal.normalized);
                direction = newDirection;
            }
        }

        if (other.CompareTag("Player"))
        {
            if (hitCountPlayer == 0)
            {
                hitCountPlayer++;
            }
            else
            {
                OnBulletDestroy();
            }
        }
    }

    public override void OnShoot()
    {
    }

    public override void OnBulletDestroy()
    {
        if (!gameObject.activeSelf) return;
        PoolManager.ReleaseObject(gameObject);
    }

    public override void Initialize(float damage, Vector3 bulletSize)
    {
        transform.localScale = bulletSize;
        speed = bulletStats.bulletSpeed;
        lifetime = bulletStats.lifeTime;
        elapsedTime = 0;

        bulletDamage = damage;
        direction = transform.right;

        playerTransform = PlayerMovement.Instance.transform;

        // Inicialize os novos parâmetros
        isExplosive = bulletStats.isExplosive;
        explosionRadius = bulletStats.explosionRadius;
        damageableLayers = bulletStats.damageableLayers;

        // Piercing
        isPiercingShoot = bulletStats.isPiercingShoot;
        MaxPiercingShoots = bulletStats.MaxPiercingShoots;

        // Recochet
        isRecochetShoot = bulletStats.IsRecochetShoot;
        recochetAmount = bulletStats.RecochetAmount;

        // IsShootGetBigByTime
        isBulletGetBigByTime = bulletStats.isShootGetBigByTime;
        maxBulletSize = bulletStats.MaxBulletSize;

        // Boomerang
        isBoomerangShoot = bulletStats.isBoomerangShoot;
        maxDistance = bulletStats.maxDistanceBoomerang;

        hitCountPlayer = 0;

        // Quando for fazer o código final melhorar isso
        if (isBoomerangShoot)
        {
            bulletStats.isBoomerangShoot = true;
            isPiercingShoot = true;
        }
    }

    private void OnBecameInvisible()
    {
        if (isBoomerangShoot) return;
        OnBulletDestroy();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnBulletCollide(other);
    }

    private void Explode()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageableLayers);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                hitCollider.GetComponent<HealthController>().ReduceHealth((int)bulletDamage);
            }
        }

        Debug.DrawLine(transform.position, transform.position + Vector3.up * explosionRadius, Color.red, 2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.down * explosionRadius, Color.red, 2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.left * explosionRadius, Color.red, 2f);
        Debug.DrawLine(transform.position, transform.position + Vector3.right * explosionRadius, Color.red, 2f);

        OnBulletDestroy();
    }
}
