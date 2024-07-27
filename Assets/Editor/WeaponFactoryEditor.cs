#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponFactory))]
public class WeaponFactoryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        WeaponFactory weaponFactory = (WeaponFactory)target;
        SerializedProperty isEnabled = serializedObject.FindProperty("isEnabled");

        if (isEnabled != null)
        {
            EditorGUILayout.PropertyField(isEnabled);
            if (weaponFactory.isEnabled)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("pistol"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("assault"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("shotgun"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bazooka"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("customBullet"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletStats"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("selectedPerks"));
                EditorGUI.indentLevel--;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif