using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProgressionManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject explosionFX;
    [SerializeField] private CinemachineVirtualCamera doorCamera;
    private CinemachineVirtualCamera mainCamera;

    private void Start()
    {
        CinemachineBrain cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        if (cinemachineBrain != null)
        {
            mainCamera = cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        }
    }
    private void Update()
    {
        enemies.RemoveAll(item => item == null);

        if (enemies.Count == 0)
        {
            StartCoroutine(DoorDestroyAnimation());
            enabled = false;
        }
    }

    IEnumerator DoorDestroyAnimation()
    {
        doorCamera.Priority = mainCamera.Priority + 1;
        yield return new WaitForSeconds(1f);

        if (explosionFX != null)
        {
            if (door != null)
            {
                Destroy(door);
            }
            Instantiate(explosionFX, door.transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(1f);

        doorCamera.Priority = mainCamera.Priority - 1;
    }
}
