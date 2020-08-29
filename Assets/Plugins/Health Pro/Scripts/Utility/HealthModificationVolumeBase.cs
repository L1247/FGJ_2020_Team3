using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lowscope.HealthPro
{
    public abstract class HealthModificationVolumeBase : MonoBehaviour
    {
        [Header("Affect Settings")]

        [SerializeField, Tooltip("What kind of effect do we want to apply to the effected?")]
        private EHealthModificationEffect effect = EHealthModificationEffect.Damage;

        [SerializeField, Tooltip("The amount of the specified effect it should apply. Only applies to doing damage and healing")]
        private float amount = 1;

        [SerializeField, Tooltip("Should it only be applied once per entity? This will get reset once the damage volume is disabled.")]
        private bool applyEffectOncePerEntity = false;

        [SerializeField, Tooltip("Should it only be able to apply to one entity? This will be reset once the damage volume is disabled.")]
        private bool applyEffectOnce = false;

        [SerializeField, Tooltip("Should this affect be applied every x seconds?")]
        private bool applyContinuousEffect = false;

        [SerializeField, Tooltip("The rate in seconds at which it should apply damage.")]
        private float continuousEffectTickRate = 0.5f;

        [Header("Deactivation Settings")]

        [SerializeField, Tooltip("Should the object deactivate automatically?")]
        private bool autoDeactivate;

        [SerializeField, Tooltip("Deactivates the object after in specified time after being enabled")]
        private float autoDeactivationTime;

        [SerializeField, Tooltip("Deactivate the game object once affect is applied.")]
        private bool deactivateOnEffect = false;

        [SerializeField, Tooltip("In case deactivate on effect is set, how long should it be possible to apply the effect?")]
        private float deactivateEffectTime;

        [SerializeField, Tooltip("In case deactivate on effect is set, how long should it take before the object is disabled?")]
        private float deactivateObjectTime;

        [Header("Filter settings")]

        [SerializeField, Tooltip("Optional, in case it is set then the following reference will be set as the owner of the volume")]
        private Health owner = null;

        [SerializeField, Tooltip("Should it be possible to hit the owner? This can be set through Configure() or by adressing the owner field")]
        private bool affectOwner = true;

        [SerializeField, Tooltip("Only apply the affect to the specified tags")]
        private bool onlyAffectTags = false;

        [SerializeField, Tooltip("If only affect tags is set, which tags should we affect?")]
        private string[] affectableTags = new string[0];

        private List<int> continuousDamageRegister = new List<int>();
        private HashSet<int> ignoreRegister = new HashSet<int>();

        private bool canApplyEffect = true;
        public int OwnerInstanceID { private set; get; }

        private Coroutine affectTickCoroutine;

        private List<IHealthModificationListener> listeners = new List<IHealthModificationListener>();

        public void AddListener(IHealthModificationListener listsener)
        {
            listeners.Add(listsener);
        }

        public void RemoveListener(IHealthModificationListener listener)
        {
            if (listeners.Contains(listener))
            {
                listeners.Remove(listener);
            }
        }

        public void SetOwner(int ownerInstanceID)
        {
            this.OwnerInstanceID = ownerInstanceID;
        }

        public void SetOwner(Health owner)
        {

#if UNITY_EDITOR
            if (owner == null)
            {
                Debug.LogWarning("Attempted to set Health" +
                    " Modification Volume owner as null ");
            }
#endif

            if (owner != null)
            {
                this.OwnerInstanceID = owner.gameObject.GetInstanceID();
            }
        }

        public void SetAmount(float amount)
        {
            this.amount = amount;
        }

        public void SetEffect(EHealthModificationEffect effect)
        {
            this.effect = effect;
        }

        public void SetAffectableTags(params string[] tags)
        {
            this.affectableTags = tags;
        }

        public void SetDeactivateOnEffect(bool deactivateOnEffect)
        {
            this.deactivateOnEffect = deactivateOnEffect;
        }

        public void SetAutoDeactivationTime(float time)
        {
            this.autoDeactivationTime = time;
        }

        public void Configure(ModificationVolumeConfiguration config)
        {
            OwnerInstanceID = config.Owner;
            amount = config.Amount;
            deactivateOnEffect = config.DeactivateOnEffect;
            autoDeactivationTime = config.DeactivationTime;
            effect = config.Effect;

            if (config.TargetTags != null)
            {
                affectableTags = config.TargetTags;
            }

            continuousDamageRegister.Clear();

            canApplyEffect = true;
        }

        private void OnEnable()
        {
            if (owner != null)
            {
                OwnerInstanceID = owner.gameObject.GetInstanceID();
            }
            else
            {
                OwnerInstanceID = 0;
            }

            if (autoDeactivationTime != 0)
            {
                StartCoroutine(DisableGameObjectAfterTimeCoroutine(autoDeactivationTime));
            }
        }

        private void OnDisable()
        {
            ignoreRegister.Clear();
            continuousDamageRegister.Clear();
        }

        public void ApplyTarget(GameObject target)
        {
            if (!canApplyEffect)
            {
                return;
            }

            if (IsvalidTarget(target))
            {
                Health getHealth = Health.GetInstance(target);

                if (getHealth == null)
                {
                    return;
                }
                else
                {
                    // Add component to get a callback it has been disabled.

                    HMVDisableTrigger trigger = getHealth.GetComponent<HMVDisableTrigger>();

                    if (trigger == null)
                    {
                        trigger = getHealth.gameObject.AddComponent<HMVDisableTrigger>();
                    }

                    trigger.AddReference(this);

                    Vector3 hitLocation = this.transform.position;

                    if (applyEffectOnce)
                    {
                        canApplyEffect = false;

                        ApplyEffect(getHealth, hitLocation);

                        return;
                    }

                    if (applyEffectOncePerEntity)
                    {
                        ignoreRegister.Add(target.GetInstanceID());

                        ApplyEffect(getHealth, hitLocation);
                    }
                    else
                    {
                        if (applyContinuousEffect)
                        {
                            if (!continuousDamageRegister.Contains(target.GetInstanceID()))
                            {
                                continuousDamageRegister.Add(target.GetInstanceID());

                                if (applyContinuousEffect && continuousDamageRegister.Count == 1)
                                {
                                    affectTickCoroutine = StartCoroutine(DamageTickCoroutine());
                                }
                            }
                        }
                        else
                        {
                            ApplyEffect(getHealth, hitLocation);
                        }
                    }
                }
            }
        }

        public void RemoveTarget(GameObject target)
        {
            if (IsvalidTarget(target))
            {
                if (!Health.InstanceExists(target))
                    return;

                int instanceID = target.GetInstanceID();

                if (continuousDamageRegister.Contains(instanceID))
                {
                    continuousDamageRegister.Remove(instanceID);

                    if (continuousDamageRegister.Count == 0 && affectTickCoroutine != null)
                    {
                        StopCoroutine(affectTickCoroutine);
                    }
                }

                for (int i = 0; i < listeners.Count; i++)
                {
                    listeners[i].OnEffectRemoved();
                }
            }
        }

        IEnumerator DamageTickCoroutine()
        {
            while (continuousDamageRegister.Count > 0)
            {
                int registerCount = continuousDamageRegister.Count;

                for (int i = registerCount - 1; i >= 0; i--)
                {
                    Health getHealthInstance = Health.GetInstance(continuousDamageRegister[i]);

                    bool intersecting = GetComponent<Collider>().bounds.Intersects(getHealthInstance.gameObject.GetComponent<Collider>().bounds);

                    if (intersecting)
                    {
                        ApplyEffect(getHealthInstance, this.transform.position);
                    }
                    else
                    {
                        // Remove target in case it has been disabled.
                        RemoveTarget(getHealthInstance.gameObject);
                    }
                }

                yield return new WaitForSeconds(continuousEffectTickRate);
            }
        }

        private bool HasEffectableTag(GameObject target)
        {
            for (int i = 0; i < affectableTags.Length; i++)
            {
                if (target.CompareTag(affectableTags[i]))
                    return true;
            }

            return false;
        }

        private bool IsvalidTarget(GameObject target)
        {
            if (ignoreRegister.Contains(target.GetInstanceID()))
                return false;

            if (onlyAffectTags && !HasEffectableTag(target))
                return false;

            if (affectOwner && target.GetInstanceID() == OwnerInstanceID)
                return false;

            if (Health.GetInstance(target) as HealthGroup != null)
                return false;

            return true;
        }

        private void ApplyEffect(Health target, Vector3 hitLocation)
        {
            if (target != null)
            {
                switch (effect)
                {
                    case EHealthModificationEffect.Damage:
                        target.Damage(amount, hitLocation, Health.GetInstance(OwnerInstanceID));
                        break;
                    case EHealthModificationEffect.Heal:
                        target.Heal(amount, hitLocation, Health.GetInstance(OwnerInstanceID));
                        break;
                    case EHealthModificationEffect.Revive:
                        target.Revive(cause: Health.GetInstance(OwnerInstanceID));
                        break;
                    case EHealthModificationEffect.Kill:
                        target.Kill(true, hitLocation, Health.GetInstance(OwnerInstanceID));
                        break;
                    default:
                        break;
                }

                if (deactivateOnEffect)
                {
                    Deactivate();
                }
                else
                {
                    for (int i = 0; i < listeners.Count; i++)
                    {
                        listeners[i].OnEffectApplied(amount, hitLocation, effect);
                    }
                }
            }
            else
            {
                Debug.LogWarning(string.Format("Attempted to attack object without a health component: {0}.", gameObject.name));
            }
        }

        private IEnumerator DisableGameObjectAfterTimeCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            this.gameObject.SetActive(false);
        }

        private IEnumerator DisableDamageAfterTimeCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            canApplyEffect = false;
        }

        private void Deactivate()
        {
            if (deactivateObjectTime <= 0)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                StartCoroutine(DisableGameObjectAfterTimeCoroutine(deactivateObjectTime));

                if (deactivateEffectTime <= 0)
                {
                    canApplyEffect = false;
                }
                else
                {
                    StartCoroutine(DisableDamageAfterTimeCoroutine(deactivateEffectTime));
                }
            }
        }
    }
}