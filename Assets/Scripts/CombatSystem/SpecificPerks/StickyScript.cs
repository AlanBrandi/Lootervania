using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyScript : MonoBehaviour
{
    private Animator anim;
    public bool isBig= false;
    private float originalLocalScale;
    public SOBulletStats bulletStats;
    private float elapsedTime;
    private float lifetime;
    private float damage;
    private float originalDamage;
    [SerializeField] private GameObject explosionFX;
    private void Start()
    {
        anim = GetComponent<Animator>();
        originalLocalScale = transform.localScale.x;
        elapsedTime = 0;
    }

    public void UpdateSize()
    {
        isBig = true;
    }

    private void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;
        float scalePercentage = (elapsedTime / lifetime) * .2f;
        float currentScale = Mathf.Lerp(originalLocalScale, bulletStats.maxBulletSize, scalePercentage);

        //float sizeRatio = currentScale / bulletStats.maxBulletSize;
        //damage = originalDamage * sizeRatio;
        damage += (int)(0.2 * elapsedTime);

        transform.localScale = new Vector3(currentScale, currentScale, 1f);
    }
    public void Explode(float explosionDamage, float explosionRadius, LayerMask damageableLayers, GameObject circlePrefab, float time)
    {
        lifetime = time;
        originalDamage = explosionDamage;
        damage = originalDamage;
        StartCoroutine(StickyShotCoroutine(explosionRadius, damageableLayers, circlePrefab, time));
    }

    private void FinalExplosion(float explosionRadius, LayerMask damageableLayers, GameObject circlePrefab)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, ((explosionRadius)+(explosionRadius*transform.localScale.x)), damageableLayers);
        bool hitEnemy = false;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") && hitCollider.isTrigger)
            {
                Vector2 attackDirection = (hitCollider.transform.position - transform.position).normalized;
                hitCollider.GetComponent<HealthController>().ReduceHealth((int)damage, attackDirection);
                hitEnemy = true;
            }
        }
        GameObject tmpExplosion = Instantiate(explosionFX, transform.position, Quaternion.identity);
        tmpExplosion.transform.localScale = transform.localScale;
        //DrawExplosionCircle(transform.position, explosionRadius, hitEnemy ? Color.red : Color.green, circlePrefab);
        Destroy(gameObject);
    }

    private void DrawExplosionCircle(Vector3 position, float radius, Color color, GameObject circlePrefab)
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

        FindObjectOfType<CameraShake>().Shake(Vector2.right, .1f);
        Destroy(circle, 0.5f);
    }

    private IEnumerator StickyShotCoroutine(float explosionRadius, LayerMask damageableLayers, GameObject circlePrefab, float time)
    {
        yield return new WaitForSeconds(2f - time);
        anim.SetBool("StartExplosion", true);
        yield return new WaitForSeconds(time);
        FinalExplosion(explosionRadius, damageableLayers, circlePrefab);
    }
}
