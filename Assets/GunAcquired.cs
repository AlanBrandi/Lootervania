using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAcquired : MonoBehaviour
{
    [SerializeField] private GameObject gun;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
        }
    }
}
