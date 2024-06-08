using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Pool.Core;

public class EnemyDummieBehaviour : MonoBehaviour
{
    [SerializeField] bool IsEnemyAgressive;
    [SerializeField] bool IsMoving = false;
    [SerializeField] bool IsEnemyMelee = false; //implement later

    public void RespawnDummie()
    {
        PoolManager.SpawnObject(this.gameObject, transform.position, transform.rotation);
    }


    //MoveRight and left
}
