# if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lowscope.HealthPro
{

    // This solution was required because there is no way to properly check if entire gameobject hierarchies are dirty.
    // This will do callbacks for subscribed objects when a property of an object with the same root GameObject has been modified.
    [InitializeOnLoad]
    public class HealthValidator
    {
        static HealthValidator()
        {
            Undo.postprocessModifications += OnPostProcessModifications;
            EditorApplication.hierarchyWindowChanged += OnHierarchyChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            Selection.selectionChanged += LastSelection;
        }

        private static Transform lastRoot;

        private static void LastSelection()
        {
            if (Application.isPlaying)
                return;

            Transform getTransform = Selection.activeTransform;

            if (getTransform != null)
            {
                lastRoot = getTransform.transform.root;
            }
        }

        private static void OnPlayModeChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingPlayMode)
            {
                validateableHealths.Clear();
            }
        }

        // Transform is the root object
        private static Dictionary<Transform, HashSet<Health>> validateableHealths = new Dictionary<Transform, HashSet<Health>>();

        public static void Remove(Health monoBehaviour)
        {
            if (validateableHealths.ContainsKey(monoBehaviour.transform.root))
            {
                if (validateableHealths.Count == 1)
                {
                    validateableHealths.Clear();
                }
                else
                {
                    validateableHealths[monoBehaviour.transform.root].Remove(monoBehaviour);
                }
            }
        }

        public static void Add(Health monoBehaviour)
        {
            if (!validateableHealths.ContainsKey(monoBehaviour.transform.root))
            {
                validateableHealths.Add(monoBehaviour.transform.root, new HashSet<Health>() { monoBehaviour });
            }
            else
            {
                if (!validateableHealths[monoBehaviour.transform.root].Contains(monoBehaviour))
                {
                    validateableHealths[monoBehaviour.transform.root].Add(monoBehaviour);
                }
            }
        }

        // In case anything in the transform hierachy changes we want to check what object has been modified
        private static void OnHierarchyChanged()
        {
            if (Application.isPlaying)
                return;

            if (Selection.activeTransform != null)
            {
                CallValidation(Selection.activeTransform.root);
            }
            else
            {
                if (lastRoot != null && validateableHealths.ContainsKey(lastRoot))
                {
                    CallValidation(lastRoot);
                }
            }
        }

        // What happens here is that when an object gets removed, we check if the root is a Health Component
        // If that is the case, we send a OnValidate notification to ensure all references are in order.
        private static UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] modifications)
        {
            if (Application.isPlaying)
                return modifications;

            for (int i = 0; i < modifications.Length; i++)
            {
                if (modifications[i].currentValue.propertyPath == "m_RootOrder")
                {
                    Component valueTarget = modifications[i].currentValue.target as Component;

                    if (valueTarget != null && valueTarget.gameObject != null)
                    {
                        CallValidation(valueTarget.transform.root);
                    }

                    // Gets called when object is unparented.
                    // We use the lastroot reference to update the health component.
                    if (valueTarget.transform.root == valueTarget.transform)
                    {
                        CallValidation(lastRoot);
                    }
                }
            }

            return modifications;
        }

        // This will check the hierarchy from below to top if there is a gameobject with a health component
        // And it will call CustomOnValidate if it finds one.
        private static void CallValidation(Transform target)
        {
            if (target == null)
                return;

            if (validateableHealths.ContainsKey(target))
            {
                foreach (Health item in validateableHealths[target])
                {
                    if (item != null)
                    {
                        item.OnValidate();

                        /*
                        MethodInfo tMethod = item.GetType().GetMethod("OnValidate");
                        if (tMethod != null)
                        {
                            item.OnValidate();
                            Debug.Log(item.name);

                            tMethod.Invoke(item, null);
                            break;
                        }
                        */
                    }
                }
            }
        }
    }

}

#endif
