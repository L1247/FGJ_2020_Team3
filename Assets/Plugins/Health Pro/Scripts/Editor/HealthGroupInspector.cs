using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lowscope.HealthPro
{
    [CustomEditor(typeof(HealthGroup))]
    public class HealthGroupInspector : Editor
    {
        private bool hideInterfaces;
        private bool hideConfig;
        private bool hideListenerConfig;
        private bool hideEvents;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            hideConfig = EditorGUILayout.Foldout(hideConfig, "Config");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (!hideConfig)
            {
                GUI.enabled = false;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("totalHealth"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("currentHealth"), true);
                GUI.enabled = true;

                EditorGUILayout.PropertyField(serializedObject.FindProperty("clampHealth"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("healthComponents"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("obtainFromChildren"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("obtainFromVolume"), true);
            }

            EditorGUILayout.EndVertical();

            hideListenerConfig = EditorGUILayout.Foldout(hideListenerConfig, "Interface Scan Configuration");

            if (!hideListenerConfig)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("scanChildren"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("scanSelf"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("externalListeners"), true);

                EditorGUILayout.EndVertical();
            }

            hideEvents = EditorGUILayout.Foldout(hideEvents, "Events");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (!hideEvents)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onGroupAliveCountChange"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onGroupTotalCountChange"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onGroupEmpty"), true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("onGroupEntered"), true);
                

                EditorGUILayout.HelpBox("In case you require more events, this component reacts to health listeners. Such as the Health Death Listener.", MessageType.Info);
            }

            EditorGUILayout.EndVertical();

            HealthInspectorMethods.DrawCachedListenerComponents(serializedObject.FindProperty("cachedListenerComponents"), ref hideInterfaces);

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }

            if (GUILayout.Button("Reload Interface References"))
            {
                (target as Health).OnValidate();
            }
        }
    }
}
