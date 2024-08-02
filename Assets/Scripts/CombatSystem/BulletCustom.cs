using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
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

    private bool isAuraShot;
    private GameObject auraGameObject;
    private GameObject auraTmp;
    private float auraDamage;
    private float auraSpeedMod;
    private float auraLifetimeMod;

    private bool isStickyShot;
    private float MaxStickyShotsTime;

    private bool isPullShot;
    private GameObject pullGameObject;
    private float pullShotChance;
    private float pullStrength;
    private float maxPullDistance;
    private float maxPullTime;

    private Transform playerTransform;
    private int hitCountPlayer = 0;

    // Parâmetros de explosão
    private bool isExplosive;
    private float explosionRadius;
    private int explosionDamage;
    private LayerMask damageableLayers;

    [SerializeField] private SOBulletStats bulletStats;
    [SerializeField] private GameObject circlePrefab;

    private bool shouldMove = true; // Flag to control movement

    private void Awake()
    {
        initialSize = transform.localScale;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (!gameObject.activeSelf) return;

        if (shouldMove)
        {
            Move();
        }

        LifeTimeCount();
    }

    private void Move()
    {
        rb.velocity = direction * speed * Time.deltaTime;
    }

    private void LifeTimeCount()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime >= lifetime + MaxStickyShotsTime)
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

        if (isAuraShot)
        {
            if (elapsedTime >= lifetime * auraLifetimeMod)
            {
                Destroy(auraTmp);
            }
        }
    }

    public override void Act()
    {
        OnShoot();
    }

    public override void OnBulletCollide(Collider2D other, Vector2 attackDiretion)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<HealthController>().ReduceHealth((int)bulletDamage, attackDiretion);
            
            // Piercing
            if (!isPiercingShoot)
            {
                if (isExplosive)
                {
                    if(!isStickyShot)
                    {
                        Explode();
                    }
                }
                else
                    OnBulletDestroy();
            }

            MaxPiercingShoots--;
            if (MaxPiercingShoots < 0)
            {
                OnBulletDestroy();
            }

            if (isPullShot)
            {
                if (Random.Range(0f, 100f) <= pullShotChance)
                {
                    Instantiate(pullGameObject, transform.position, Quaternion.identity);
                }
            }
        }

        if (other.CompareTag("Level"))
        {
            if (isStickyShot)
            {
                rb.velocity = Vector2.zero;
                shouldMove = false;
                StartCoroutine(StickyShotCoroutine());
            }
            else if (!isRecochetShoot)
            {
                if (isExplosive)
                {
                    if(isStickyShot)
                    {
                        rb.velocity = Vector2.zero;
                        shouldMove = false;
                        StartCoroutine(StickyShotCoroutine());
                    }
                    else
                    {
                        Explode();
                    }
                }
                    
                else
                    OnBulletDestroy();
            }

            if (!isStickyShot)
            {
                if (recochetAmount <= 0)
                {
                    OnBulletDestroy();
                    return;
                }

                recochetAmount--;

                Debug.Log("enter");
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction);
                if (hit.collider != null)
                {
                    Vector2 normal = hit.normal;

                    Vector2 newDirection = Vector2.Reflect(direction, normal);
                    direction = newDirection;
                    Debug.DrawRay(hit.point, direction);
                }
            }

            if (isPullShot)
            {
                if (Random.Range(0f, 100f) <= pullShotChance)
                {
                    Instantiate(pullGameObject, transform.position, Quaternion.identity);
                }
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
        if (gameObject != null)
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
        explosionDamage = bulletStats.explosionDamage;
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

        // Aura
        isAuraShot = bulletStats.isAuraShot;
        auraGameObject = bulletStats.auraGameObject;
        auraSpeedMod = bulletStats.auraSpeedMod;
        auraLifetimeMod = bulletStats.auraLifetimeMod;
        bulletStats.auraDamage = bulletDamage / bulletStats.auraDamageMod;

        //Sticky
        isStickyShot = bulletStats.IsStickyShot;
        MaxStickyShotsTime = bulletStats.maxStickyShotsTime;

        //Pull
        isPullShot = bulletStats.isPullShot;
        pullGameObject = bulletStats.pullGameObject;
        pullShotChance = bulletStats.pullShotChance;

        hitCountPlayer = 0;

        shouldMove = true; // Reset the movement flag

        // Quando for fazer o código final melhorar isso
        if (isBoomerangShoot)
        {
            bulletStats.isBoomerangShoot = true;
            isPiercingShoot = true;
        }

        if (isAuraShot)
        {
            lifetime = lifetime * auraLifetimeMod;
            speed = speed / auraSpeedMod;
            GameObject aura = Instantiate(auraGameObject, transform);
            auraTmp = aura;
        }
    }

    public void ActivatePerks(List<string> selectedPerks)
    {
        if (selectedPerks.Contains("PiercingShoot"))
        {
            isPiercingShoot = true;
        }
        if (selectedPerks.Contains("RecochetShoot"))
        {
            isRecochetShoot = true;
        }
        if (selectedPerks.Contains("StickyShot"))
        {
            isStickyShot = true;
        }
        // ... adicione mais condições conforme necessário
    }

    private void OnBecameInvisible()
    {
        if (isBoomerangShoot) return;
        OnBulletDestroy();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnBulletCollide(other, transform.right);
    }

    private IEnumerator StickyShotCoroutine()
    {
        yield return new WaitForSeconds(MaxStickyShotsTime);
        Explode();
    }

    private void Explode()
{
    Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageableLayers);
    bool hitEnemy = false;

    foreach (var hitCollider in hitColliders)
    {
        if (hitCollider.CompareTag("Enemy"))
        {
            Vector2 attackDirection = (hitCollider.transform.position - transform.position).normalized;
            hitCollider.GetComponent<HealthController>().ReduceHealth((int)explosionDamage, attackDirection);
            hitEnemy = true;
        }
    }

    DrawExplosionCircle(transform.position, explosionRadius, hitEnemy ? Color.red : Color.green);

    OnBulletDestroy();
}

    private void DrawExplosionCircle(Vector3 position, float radius, Color color)
    {
        GameObject circle = Instantiate(circlePrefab, position, Quaternion.identity);
        LineRenderer lineRenderer = circle.GetComponent<LineRenderer>();

        int segments = 50;
        lineRenderer.positionCount = segments + 1;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        float angle = 0f;
        for (int i = 0; i <= segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0) + position);
            angle += 360f / segments;
        }

        Destroy(circle, 0.5f);
    }
}
