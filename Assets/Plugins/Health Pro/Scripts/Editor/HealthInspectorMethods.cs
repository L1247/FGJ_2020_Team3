using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Lowscope.HealthPro
{
    public class HealthInspectorMethods : Editor
    {
        public static void DrawCachedListenerComponents(SerializedProperty cachedListenerComponents, ref bool hideInterfaces)
        {
            SerializedProperty arraySizeProp = cachedListenerComponents.FindPropertyRelative("Array.size");

            EditorGUI.BeginChangeCheck();

            hideInterfaces = EditorGUILayout.Foldout(hideInterfaces, string.Format("Cached Listener Components ({0})", arraySizeProp.intValue));

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool("HealthPro_HideInterfaces", hideInterfaces);
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (!hideInterfaces)
            {
                GUI.enabled = false;
                for (int i = 0; i < arraySizeProp.intValue; i++)
                {
                    EditorGUILayout.PropertyField(cachedListenerComponents.GetArrayElementAtIndex(i), new GUIContent());

                    if (i > 20)
                    {
                        EditorGUILayout.TextField("Limiting view to 20 for performance reasons");
                        break;
                    }
                }
                GUI.enabled = true;

            }

            EditorGUILayout.EndVertical();
        }

    }
}