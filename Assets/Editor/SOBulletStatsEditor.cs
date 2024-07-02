#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SOBulletStats))]
public class SOBulletStatsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        SOBulletStats bulletStats = (SOBulletStats)target;

        // Mostra os campos padrão
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bulletSpeed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("lifeTime"));

        // Mostra IsRecochetShoot e sua propriedade recochetAmount se for true
        SerializedProperty isRecochetShootProp = serializedObject.FindProperty("IsRecochetShoot");
        if (isRecochetShootProp != null)
        {
            EditorGUILayout.PropertyField(isRecochetShootProp);
            if (bulletStats.IsRecochetShoot)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("recochetAmount"));
                EditorGUI.indentLevel--;
            }
        }
     
        // Mostra isPiercingShoot e sua propriedade MaxPiercingShoots se for true
        SerializedProperty isPiercingShootProp = serializedObject.FindProperty("isPiercingShoot");
        if (isPiercingShootProp != null)
        {
            EditorGUILayout.PropertyField(isPiercingShootProp);
            if (bulletStats.isPiercingShoot)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxPiercingShoots"));
                EditorGUI.indentLevel--;
            }
        }

        SerializedProperty isShootGetBigByTime = serializedObject.FindProperty("isShootGetBigByTime");
        if (isShootGetBigByTime != null)
        {
            EditorGUILayout.PropertyField(isShootGetBigByTime);
            if (bulletStats.isShootGetBigByTime)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxBulletSize"));
                EditorGUI.indentLevel--;
            }
        }
        
        SerializedProperty isBoomerangShoot = serializedObject.FindProperty("isBoomerangShoot");
        if (isShootGetBigByTime != null)
        {
            EditorGUILayout.PropertyField(isBoomerangShoot);
            if (bulletStats.isBoomerangShoot)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDistanceBoomerang"));
                EditorGUI.indentLevel--;
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
