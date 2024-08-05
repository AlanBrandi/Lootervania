using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyScript : MonoBehaviour
{
    private Animator anim;
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    public void Explode(float explosionDamage, float explosionRadius, LayerMask damageableLayers, GameObject circlePrefab, float time)
    {
        StartCoroutine(StickyShotCoroutine(explosionDamage, explosionRadius, damageableLayers, circlePrefab, time));
    }

    private void FinalExplosion(float explosionDamage, float explosionRadius, LayerMask damageableLayers, GameObject circlePrefab)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, damageableLayers);
        bool hitEnemy = false;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") && hitCollider.isTrigger)
            {
                Vector2 attackDirection = (hitCollider.transform.position - transform.position).normalized;
                hitCollider.GetComponent<HealthController>().ReduceHealth((int)explosionDamage, attackDirection);
                hitEnemy = true;
            }
        }
        DrawExplosionCircle(transform.position, explosionRadius, hitEnemy ? Color.red : Color.green, circlePrefab);
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

    private IEnumerator StickyShotCoroutine(float explosionDamage, float explosionRadius, LayerMask damageableLayers, GameObject circlePrefab, float time)
    {
        yield return new WaitForSeconds(3f - time);
        anim.SetBool("StartExplosion", true);
        yield return new WaitForSeconds(time);
        FinalExplosion(explosionDamage, explosionRadius, damageableLayers, circlePrefab);
    }
}
