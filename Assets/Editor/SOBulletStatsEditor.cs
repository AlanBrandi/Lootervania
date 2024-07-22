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

        // Mostra isShootGetBigByTime e sua propriedade maxBulletSize se for true
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

        // Mostra isBoomerangShoot e sua propriedade maxDistanceBoomerang se for true
        SerializedProperty isBoomerangShoot = serializedObject.FindProperty("isBoomerangShoot");
        if (isBoomerangShoot != null)
        {
            EditorGUILayout.PropertyField(isBoomerangShoot);
            if (bulletStats.isBoomerangShoot)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxDistanceBoomerang"));
                EditorGUI.indentLevel--;
            }
        }

        // Mostra isExplosive e suas propriedades explosionRadius e damageableLayers se for true
        SerializedProperty isExplosiveProp = serializedObject.FindProperty("isExplosive");
        if (isExplosiveProp != null)
        {
            EditorGUILayout.PropertyField(isExplosiveProp);
            if (bulletStats.isExplosive)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("explosionRadius"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("damageableLayers"));
                EditorGUI.indentLevel--;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
