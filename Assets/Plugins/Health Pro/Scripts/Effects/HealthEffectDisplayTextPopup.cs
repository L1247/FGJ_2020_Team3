using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.HealthPro.Pooling;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Display Text Popup")]
    public class HealthEffectDisplayTextPopup : MonoBehaviour, IDamageable, IKillable, IHealable, IDamageCallback, IHealCallback
    {
        [SerializeField]
        private GameObject displayTextPrefab = null;

        [SerializeField, Tooltip("Used for obtaining the collider height, only required if you use 3D")]
        private new Collider collider;

        [SerializeField, Tooltip("Used for obtaining the collider height, only required if you use 2D")]
        private new Collider2D collider2D;

        [SerializeField]
        private bool is2DPopup = false;

        [System.Serializable]
        public class TextPopupConfig
        {
            public EObtainHeightMethod ObtainHeightMethod;

            public EEffectCondition Condition;

            public Color Color = Color.white;

            public float HeightOffset;

            [SerializeField, Tooltip("This only happens when the effect condition is " +
                "set to Callback, only executing if the target is set to a specific tag")]
            public bool FilterForObjectTags = false;

            [SerializeField]
            private string[] tags = new string[0];

            public bool DoesMatchTag(Transform target)
            {
                for (int i = 0; i < tags.Length; i++)
                {
                    if (target.CompareTag(tags[i]))
                    {
                        return true;
                    }
                }

                return false;
            }

            public float ObtainHeightOffset(Transform target, Component collider)
            {
                if (ObtainHeightMethod != EObtainHeightMethod.HeightOffset && collider == null)
                {
                    if (Condition != EEffectCondition.OnDamageCallback && Condition != EEffectCondition.OnDamageCallbackKill)
                    {
#if UNITY_EDITOR
                        Debug.LogWarning(string.Format("Display Text Popup Effect: " +
                            "Could not find collider for : {0}. Either change option" +
                            "to use Height Offset only or reference a collider.", target.name));
#endif
                    }

                    return HeightOffset;
                }

                switch (ObtainHeightMethod)
                {
                    case EObtainHeightMethod.ColliderHeightWithHeightOffset:
                        return ((collider as Collider).bounds.extents.y) + HeightOffset;
                    case EObtainHeightMethod.Collider2DHeightWithHightOffset:
                        return ((collider as Collider2D).bounds.extents.y) + HeightOffset;
                    case EObtainHeightMethod.HeightOffset:
                        return HeightOffset;
                    default:
                        return HeightOffset;
                }
            }
        }

        [SerializeField]
        private TextPopupConfig[] textPopupConfigurations;

        // Int == Instance ID
        private static Dictionary<int, float> heightOffsetCache;
        private static int textPopupDisplayerCount;

        private void Reset()
        {
            textPopupConfigurations = new TextPopupConfig[1] { new TextPopupConfig() };
            textPopupConfigurations[0].Color = Color.white;
            collider = GetComponent<Collider>();
        }

        private void Awake()
        {
            textPopupDisplayerCount++;

            if (textPopupDisplayerCount == 1)
            {
                heightOffsetCache = new Dictionary<int, float>();
            }

            if (collider == null)
            {
                collider = this.GetComponent<Collider>();
            }
        }

        private void OnDestroy()
        {
            textPopupDisplayerCount--;

            if (textPopupDisplayerCount < 0)
            {
                heightOffsetCache.Clear();
            }
        }

        public void OnDamaged(HealthInfo info)
        {
            OnEvent(EEffectCondition.OnDamaged, info);
        }

        public void OnDeath(HealthInfo info)
        {
            OnEvent(EEffectCondition.OnDeath, info);
        }

        public void OnHealed(HealthInfo info)
        {
            if (info.IsHealthRestored())
            {
                OnEvent(EEffectCondition.OnRestored, info);
            }
            else
            {
                OnEvent(EEffectCondition.OnHealed, info);
            }
        }

        public void OnHealDone(HealthInfo info)
        {
            OnEvent(EEffectCondition.OnHealCallback, info, info.Owner);
        }

        public void OnDamageDone(HealthInfo info)
        {
            if (info.CurrentHealth > 0)
            {
                OnEvent(EEffectCondition.OnDamageCallback, info, info.Owner);
            }
            else
            {
                OnEvent(EEffectCondition.OnDamageCallbackKill, info, info.Owner);
            }
        }

        private void OnEvent(EEffectCondition condition, HealthInfo info, Health target = null)
        {
            for (int i = 0; i < textPopupConfigurations.Length; i++)
            {
                if (textPopupConfigurations[i].Condition == condition)
                {
                    Vector3 position;
                    float obtainHeightOffset = 0;

                    // In case we have set the damage callback to something local, we use the local member variables
                    if (condition != EEffectCondition.OnDamageCallback
                        && condition != EEffectCondition.OnDamageCallbackKill
                        && condition != EEffectCondition.OnHealCallback)
                    {
                        obtainHeightOffset = textPopupConfigurations[i].ObtainHeightOffset(this.transform, is2DPopup? (Component)collider2D : collider);

                        position = this.transform.position;
                    }
                    else
                    {
                        if (textPopupConfigurations[i].FilterForObjectTags)
                        {
                            if (!textPopupConfigurations[i].DoesMatchTag(target.transform))
                                return;
                        }

                        // In case we want to display text on external objects, we want to cache the height in a static dictionary, for optimization
                        if (heightOffsetCache.ContainsKey(target.GetInstanceID()))
                        {
                            heightOffsetCache.TryGetValue(target.GetInstanceID(), out obtainHeightOffset);
                        }
                        else
                        {
                            obtainHeightOffset = textPopupConfigurations[i].ObtainHeightOffset(target.transform, is2DPopup ? (Component)collider2D : collider);
                            heightOffsetCache.Add(target.GetInstanceID(), obtainHeightOffset);
                        }

                        position = target.transform.position;
                    }

                    TextPopupDisplayer textPopupDisplayer = PoolManager.SpawnObject(displayTextPrefab).GetComponent<TextPopupDisplayer>();
                    textPopupDisplayer.transform.rotation = displayTextPrefab.transform.rotation;

                    textPopupDisplayer.Configure(position, info.Amount.ToString(), textPopupConfigurations[i].Color, obtainHeightOffset);
                }
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(HealthEffectDisplayTextPopup))]
    public class HealthEffectDisplayTextPopupInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox("You can modify global settings within the text popup prefab.", MessageType.Info);
            base.DrawDefaultInspector();
        }
    }
#endif
}