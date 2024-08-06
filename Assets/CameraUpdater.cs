using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraUpdater : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    private Camera thisCamera;

    private void Start()
    {
        thisCamera = GetComponent<Camera>();
    }
    private void Update()
    {
        thisCamera.orthographicSize = mainCamera.orthographicSize;
    }
}
