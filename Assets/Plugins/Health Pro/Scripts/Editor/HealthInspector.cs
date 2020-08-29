using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;


namespace Lowscope.HealthPro
{
    [CustomEditor(typeof(Health), true)]
    public class HealthInspector : Editor
    {
        private Health targetComponent;

        private bool hideInterfaces;

        private bool hideDebugInfo = true;

        // Cached fields
        private float totalHealth;
        private float startingHealth;
        private bool clampHealth;

        private SerializedProperty listenerComponentsProperty;

        private static readonly string[] _dontIncludeMe = new string[] { "m_Script" };


        private void OnEnable()
        {
            hideInterfaces = EditorPrefs.GetBool("HealthPro_HideInterfaces");
            hideDebugInfo = EditorPrefs.GetBool("HealthPro_HideDebugMethods");

            totalHealth = serializedObject.FindProperty("totalHealth").floatValue;
            startingHealth = serializedObject.FindProperty("startingHealth").floatValue;
            clampHealth = serializedObject.FindProperty("clampHealth").boolValue;

            listenerComponentsProperty = serializedObject.FindProperty("cachedListenerComponents");

            targetComponent = target as Health;
        }

        public override void OnInspectorGUI()
        {
            var serializedObject = new SerializedObject(targetComponent);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            bool modifiedProperty = false;

            // Draw starting health and check for changes made in the float field.
            EditorGUI.BeginChangeCheck();

            totalHealth = EditorGUILayout.FloatField("Total Health", totalHealth);

            if (EditorGUI.EndChangeCheck())
            {
                startingHealth = totalHealth;

                startingHealth = Mathf.Max(0, startingHealth);

                modifiedProperty = true;
            }

            // Draw starting health

            EditorGUI.BeginChangeCheck();

            startingHealth = EditorGUILayout.FloatField("Starting Health", startingHealth);

            if (EditorGUI.EndChangeCheck())
            {
                if (clampHealth == true)
                {
                    startingHealth = Mathf.Clamp(startingHealth, 0, totalHealth);
                }

                modifiedProperty = true;
            }

            // Draw current health
            GUI.enabled = false;
            EditorGUILayout.FloatField("Current Health", 
                (Application.isPlaying)? serializedObject.FindProperty("currentHealth").floatValue : 0);
            GUI.enabled = true;

            // Draw the default inspector stuff
            EditorGUI.BeginChangeCheck();

            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

            if (EditorGUI.EndChangeCheck())
                modifiedProperty = true;

            // If any properties are modified, lets apply the changes
            if (modifiedProperty)
            {
                clampHealth = serializedObject.FindProperty("clampHealth").boolValue;
                serializedObject.FindProperty("startingHealth").floatValue = startingHealth;

                if (clampHealth == true)
                {
                    startingHealth = Mathf.Clamp(startingHealth, 0, totalHealth);
                }

                serializedObject.FindProperty("totalHealth").floatValue = totalHealth;
                serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.BeginChangeCheck();

            hideDebugInfo = EditorGUILayout.Toggle("Show Debug Methods", hideDebugInfo);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("HealthPro_HideDebugMethods", hideDebugInfo);
            }

            EditorGUILayout.EndVertical();

            if (hideDebugInfo)
            {
                GUI.enabled = Application.isPlaying;

                if (GUILayout.Button("Damage (1)"))
                {
                    targetComponent.Damage(1, targetComponent.transform.position, null);
                }

                if (GUILayout.Button("Heal (1)"))
                {
                    targetComponent.Heal(1, targetComponent.transform.position, null);
                }

                if (GUILayout.Button("Revive"))
                {
                    targetComponent.Revive();
                }

                if (GUILayout.Button("Restore"))
                {
                    targetComponent.Heal(1, null);
                }

                if (GUILayout.Button("Kill"))
                {
                    targetComponent.Kill(false, targetComponent.transform.position, null);
                }
            }

            GUI.enabled = true;

            HealthInspectorMethods.DrawCachedListenerComponents(listenerComponentsProperty, ref hideInterfaces);

            if (GUILayout.Button("Reload Interface References"))
            {
                (target as Health).OnValidate();
            }
        }
    }

}