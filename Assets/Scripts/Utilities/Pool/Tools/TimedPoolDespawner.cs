using UnityEngine;
using Utilities.Pool.Core;

namespace Utilities.Pool.Tools
{
    public class TimedPoolDespawner : MonoBehaviour
    {
        [SerializeField] private float delay = 1;
    
        private void OnEnable()
        {
            Invoke(nameof(Despawn), delay);
        }

        private void Despawn()
        {
            PoolManager.ReleaseObject(gameObject);
        }
    }
}
