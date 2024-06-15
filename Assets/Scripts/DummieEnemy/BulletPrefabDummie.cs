using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Pool.Core;

public class BulletPrefabDummie : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        PoolManager.ReleaseObject(gameObject);
    }
}
