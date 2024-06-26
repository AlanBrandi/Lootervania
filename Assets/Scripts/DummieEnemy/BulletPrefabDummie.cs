using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Pool.Core;

public class BulletPrefabDummie : MonoBehaviour
{
    [SerializeField] private float damage = 5;

    private void OnBecameInvisible()
    {
        if (!gameObject) return;
        PoolManager.ReleaseObject(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthController>().ReduceHealth((int)damage);
        }
    }
}
