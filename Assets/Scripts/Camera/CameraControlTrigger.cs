using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraControlTrigger : MonoBehaviour
{
    public CustomInspectorObjects customInspectorObjects;
    private Collider2D _coll;

    private void Start()
    {
        _coll = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (customInspectorObjects.panCameraOnContact)
            {
                CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            Vector2 exitDirection = (other.transform.position - _coll.bounds.center).normalized;

            if(customInspectorObjects.swapCameras && customInspectorObjects.cameraOnLeft != null && customInspectorObjects.cameraOnRight != null)
            {
                CameraManager.instance.SwapCamera(customInspectorObjects.cameraOnLeft, customInspectorObjects.cameraOnRight, exitDirection);
            }

            if (customInspectorObjects.panCameraOnContact)
            {
                CameraManager.instance.PanCameraOnContact(customInspectorObjects.panDistance, customInspectorObjects.panTime, customInspectorObjects.panDirection, true);
            }
        }
    }
}
[System.Serializable]
public class CustomInspectorObjects
{
    public bool swapCameras = false;
    public bool panCameraOnContact = false;

    [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
    [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

    [HideInInspector] public PanDirection panDirection;
    [HideInInspector] public float panDistance = 3f;
    [HideInInspector] public float panTime = .35f;
}

public enum PanDirection
{
    Up,
    Down,
    Left,
    Right
}

[CustomEditor(typeof(CameraControlTrigger))]
public class MyScriptEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;

    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (cameraControlTrigger.customInspectorObjects.swapCameras)
        {
            cameraControlTrigger.customInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField("Camera on Left", cameraControlTrigger.customInspectorObjects.cameraOnLeft,
            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;

            cameraControlTrigger.customInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField("Camera on Right", cameraControlTrigger.customInspectorObjects.cameraOnRight,
            typeof(CinemachineVirtualCamera), true) as CinemachineVirtualCamera;
        }

        if (cameraControlTrigger.customInspectorObjects.panCameraOnContact)
        {
            cameraControlTrigger.customInspectorObjects.panDirection = (PanDirection)EditorGUILayout.EnumPopup("Camera Pan Direction",
            cameraControlTrigger.customInspectorObjects.panDirection);

            cameraControlTrigger.customInspectorObjects.panDistance = EditorGUILayout.FloatField("Pan Distance", cameraControlTrigger.customInspectorObjects.panDistance);
            cameraControlTrigger.customInspectorObjects.panTime = EditorGUILayout.FloatField("Pan Time", cameraControlTrigger.customInspectorObjects.panTime);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}
