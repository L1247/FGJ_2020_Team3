
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using UnityEditor.SceneManagement;
#endif

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health", -2), DisallowMultipleComponent]
    public class Health : MonoBehaviour
    {
        private List<IHealthConfigure> iHealthConfigureInterfaces = new List<IHealthConfigure>();
        private List<IHealthCorrection> iHealthCorrectionInterfaces = new List<IHealthCorrection>();
        private List<IDamageable> iDamageableInterfaces = new List<IDamageable>();
        private List<IHealable> iHealableInterfaces = new List<IHealable>();
        private List<IInvulnerable> iInvulnerableInterfaces = new List<IInvulnerable>();
        private List<IKillable> iKillableInterfaces = new List<IKillable>();
        private List<IReviveable> iReviveableInterfaces = new List<IReviveable>();
        private List<IDamageCallback> iDamageCallbackInterfaces = new List<IDamageCallback>();
        private List<IHealCallback> iHealCallbackInterfaces = new List<IHealCallback>();

        [SerializeField, HideInInspector]
        private float totalHealth = 10;
        public float TotalHealth
        {
            get { return totalHealth; }
        }

        [SerializeField, HideInInspector]
        private float startingHealth = 10;

        [SerializeField, HideInInspector]
        private float currentHealth;
        public float CurrentHealth
        {
            get { return currentHealth; }
        }

        [SerializeField, Tooltip("Ensures curHP does not go below 0 and above maxHP")]
        private bool clampHealth = true;

        [SerializeField]
        private bool isAlive = true;
        public bool IsAlive
        {
            get { return isAlive; }
        }

        [SerializeField]
        private bool isInvulnerable;
        public bool IsInvulnerable
        {
            get { return isInvulnerable; }
        }

        [SerializeField]
        private bool reviveOnEnable = true;

        // All interfaces are stored as MonoBehaviours since interfaces are not serializeable by Unity.
        [SerializeField, HideInInspector]
        private CachedInterfaceContainers cachedInterfaces;

        [System.Serializable]
        public class CachedInterfaceContainers
        {
            public List<MonoBehaviour> Configure = new List<MonoBehaviour>();
            public List<MonoBehaviour> Correction = new List<MonoBehaviour>();
            public List<MonoBehaviour> Damage = new List<MonoBehaviour>();
            public List<MonoBehaviour> Healable = new List<MonoBehaviour>();
            public List<MonoBehaviour> Invulnerable = new List<MonoBehaviour>();
            public List<MonoBehaviour> Killable = new List<MonoBehaviour>();
            public List<MonoBehaviour> Reviveable = new List<MonoBehaviour>();
            public List<MonoBehaviour> Restore = new List<MonoBehaviour>();
            public List<MonoBehaviour> DamageCallback = new List<MonoBehaviour>();
            public List<MonoBehaviour> HealCallback = new List<MonoBehaviour>();
        }

        [SerializeField, Tooltip("Scan objects that might contain any combat related" +
            " interfaces (Healthbar etc), and notify them incase something happens.")]
        private List<GameObject> externalListeners = new List<GameObject>();

        private static Dictionary<int, Health> healthInstances = new Dictionary<int, Health>();

        public static Health GetInstance(int identifier)
        {
            Health health;
            healthInstances.TryGetValue(identifier, out health);
            return health;
        }

        public static Health GetInstance(GameObject owner)
        {
            Health health;
            healthInstances.TryGetValue(owner.GetInstanceID(), out health);
            return health;
        }

        public static bool InstanceExists(GameObject owner)
        {
            return healthInstances.ContainsKey(owner.GetInstanceID());
        }

        public static bool InstanceExists(int identifier)
        {
            return healthInstances.ContainsKey(identifier);
        }


#if UNITY_EDITOR

        [SerializeField, Tooltip("Obtain all interfaces that are apparent on this object and it's children")]
        private bool scanChildren = true;

        [SerializeField, Tooltip("Obtain all interfaces on this object")]
        private bool scanSelf = true;

        [SerializeField, HideInInspector]
        private List<MonoBehaviour> cachedListenerComponents = new List<MonoBehaviour>();

        [SerializeField, HideInInspector]
        private int totalListeners;

        private double lastUpdateTime;

        public void OnValidate()
        {
            double getStartupTime = EditorApplication.timeSinceStartup;

            if (getStartupTime - lastUpdateTime < 0.5f)
                return;

            lastUpdateTime = getStartupTime;

            if (Application.isPlaying)
            {
                return;
            }

            if (!IsPrefab())
            {
                HealthValidator.Add(this as Health);
            }

            PrefabUtility.RecordPrefabInstancePropertyModifications(this);

            List<IHealthInterface> obtainedInterfaces = new List<IHealthInterface>();

            if (scanChildren)
            {
                GetComponentsInChildren<IHealthInterface>(true, obtainedInterfaces);

                // Get Components In Children also gets the components of this object.
                // So we remove them in case we don't want the interfaces on this object.
                if (!scanSelf)
                {
                    obtainedInterfaces.RemoveRange(0, GetComponents<IHealthInterface>().Length);
                }
            }
            else
            {
                if (scanSelf)
                {
                    GetComponents<IHealthInterface>(obtainedInterfaces);
                }
            }

            // Iterate through list of all Unique listeners, scan them for components that implement IHealthInterface
            for (int i = 0; i < externalListeners.Count; i++)
            {
                if (externalListeners[i] == null || externalListeners[i] == this.gameObject)
                {
                    continue;
                }

                obtainedInterfaces.AddRange(externalListeners[i].GetComponents<IHealthInterface>());
            }

            cachedInterfaces = new CachedInterfaceContainers();

            cachedListenerComponents = obtainedInterfaces.Cast<MonoBehaviour>().ToList();

            for (int i = obtainedInterfaces.Count - 1; i >= 0; i--)
            {
                if (cachedListenerComponents[i] == null || cachedListenerComponents[i] as Health != null)
                {
                    cachedListenerComponents.RemoveAt(i);
                }
            }

            // Iterate through all MonoBehaviours that have a interface that is derived from IHealthInterface
            for (int i = 0; i < cachedListenerComponents.Count; i++)
            {
                AddValidEntry(cachedListenerComponents[i] as IHealthConfigure, cachedInterfaces.Configure);
                AddValidEntry(cachedListenerComponents[i] as IHealthCorrection, cachedInterfaces.Correction);
                AddValidEntry(cachedListenerComponents[i] as IDamageable, cachedInterfaces.Damage);
                AddValidEntry(cachedListenerComponents[i] as IHealable, cachedInterfaces.Healable);
                AddValidEntry(cachedListenerComponents[i] as IInvulnerable, cachedInterfaces.Invulnerable);
                AddValidEntry(cachedListenerComponents[i] as IKillable, cachedInterfaces.Killable);
                AddValidEntry(cachedListenerComponents[i] as IReviveable, cachedInterfaces.Reviveable);
                AddValidEntry(cachedListenerComponents[i] as IDamageCallback, cachedInterfaces.DamageCallback);
                AddValidEntry(cachedListenerComponents[i] as IHealCallback, cachedInterfaces.HealCallback);
            }

            if (IsPrefab())
            {
                // Set the current prefab dirty, in case it is a prefab with a health component
                EditorUtility.SetDirty(this.gameObject);
            }
            else
            {
                // Set the scene dirty, in case it is an object in the scene that gets updated.
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        private bool IsPrefab()
        {
#if UNITY_2018_3_OR_NEWER
            return PrefabUtility.GetCorrespondingObjectFromSource(this.gameObject) == null && PrefabUtility.GetPrefabInstanceHandle(this.gameObject) != null;
#elif UNITY_2018_1_OR_NEWER
            return PrefabUtility.GetCorrespondingObjectFromSource(this.gameObject) == null && PrefabUtility.GetPrefabObject(this.gameObject) != null;
#else
            return PrefabUtility.GetPrefabParent(this.gameObject) == null && PrefabUtility.GetPrefabObject(this.gameObject) != null;
#endif
        }

        void AddValidEntry<T>(T entry, List<MonoBehaviour> targetList)
        {
            if (entry != null)
                targetList.Add(entry as MonoBehaviour);
        }

#endif

        public void AddListener(IHealthCorrection iHealthCorrection)
        {
            if (iHealthCorrection != null && !iHealthCorrectionInterfaces.Contains(iHealthCorrection))
            {
                iHealthCorrectionInterfaces.Insert
                    (
                    iHealthCorrection.OnCorrectionOrder(this.gameObject.GetInstanceID()), iHealthCorrection
                    );
            }
        }

        public void AddListener(IHealthConfigure iHealthConfigure) { AddInterface(iHealthConfigure, iHealthConfigureInterfaces); }
        public void AddListener(IDamageable iDamageable) { AddInterface(iDamageable, iDamageableInterfaces); }
        public void AddListener(IHealable iHealable) { AddInterface(iHealable, iHealableInterfaces); }
        public void AddListener(IKillable iKillable) { AddInterface(iKillable, iKillableInterfaces); }
        public void AddListener(IInvulnerable iInvulerable) { AddInterface(iInvulerable, iInvulnerableInterfaces); }
        public void AddListener(IReviveable iReviveable) { AddInterface(iReviveable, iReviveableInterfaces); }
        public void AddListener(IDamageCallback iDamageCallback) { AddInterface(iDamageCallback, iDamageCallbackInterfaces); }
        public void AddListener(IHealCallback iHealCallback) { AddInterface(iHealCallback, iHealCallbackInterfaces); }

        public void RemoveListener(IHealthConfigure iHealthConfigure) { RemoveInterface(iHealthConfigure, iHealthConfigureInterfaces); }
        public void RemoveListener(IHealthCorrection iHealthCorrection) { RemoveInterface(iHealthCorrection, iHealthCorrectionInterfaces); }
        public void RemoveListener(IDamageable iDamageable) { RemoveInterface(iDamageable, iDamageableInterfaces); }
        public void RemoveListener(IHealable iHealable) { RemoveInterface(iHealable, iHealableInterfaces); }
        public void RemoveListener(IKillable iKillable) { RemoveInterface(iKillable, iKillableInterfaces); }
        public void RemoveListener(IInvulnerable iInvulerable) { RemoveInterface(iInvulerable, iInvulnerableInterfaces); }
        public void RemoveListener(IReviveable iReviveable) { RemoveInterface(iReviveable, iReviveableInterfaces); }
        public void RemoveListener(IDamageCallback iDamageCallback) { RemoveInterface(iDamageCallback, iDamageCallbackInterfaces); }
        public void RemoveListener(IHealCallback iHealCallback) { RemoveInterface(iHealCallback, iHealCallbackInterfaces); }

        private void AddInterface<T>(T instance, List<T> list) where T : IHealthInterface
        {
            if (instance != null && !list.Contains(instance)) list.Add(instance);
        }

        private void RemoveInterface<T>(T instance, List<T> list) where T : IHealthInterface
        {
            if (instance != null && list.Contains(instance)) list.Remove(instance);
        }

        /// <summary>
        /// This will scan and add all interfaces that inherit from IHealthInterfaces
        /// Note, it is advised to not do this every frame, preferably at initialization.
        /// </summary>
        /// <param name="gameObject">Object to scan</param>
        public void AddGameObjectInterfaces(GameObject gameObject, bool scanChildren)
        {
            IHealthInterface[] healthInterfaces = gameObject.GetComponentsInChildren<IHealthInterface>(scanChildren);

            for (int i = 0; i < healthInterfaces.Length; i++)
            {
                AddListener(healthInterfaces[i] as IHealthConfigure);
                AddListener(healthInterfaces[i] as IHealthCorrection);
                AddListener(healthInterfaces[i] as IDamageable);
                AddListener(healthInterfaces[i] as IHealable);
                AddListener(healthInterfaces[i] as IInvulnerable);
                AddListener(healthInterfaces[i] as IKillable);
                AddListener(healthInterfaces[i] as IReviveable);
                AddListener(healthInterfaces[i] as IDamageCallback);
                AddListener(healthInterfaces[i] as IHealCallback);
            }
        }

        private void ImplementInterfaces<T>(List<T> target, List<MonoBehaviour> from) where T : IHealthInterface
        {
            int capacity = from.Count;
            target.Capacity = capacity;

            for (int i = 0; i < capacity; i++)
            {
                target.Add((T)((IHealthInterface)from[i]));
            }
        }

        private void Awake()
        {
            currentHealth = startingHealth;

            if (healthInstances == null)
            {
                healthInstances = new Dictionary<int, Health>();
            }

            healthInstances.Add(this.gameObject.GetInstanceID(), this);

            ImplementInterfaces(iHealthConfigureInterfaces, cachedInterfaces.Configure);
            ImplementInterfaces(iHealthCorrectionInterfaces, cachedInterfaces.Correction);
            ImplementInterfaces(iDamageableInterfaces, cachedInterfaces.Damage);
            ImplementInterfaces(iHealableInterfaces, cachedInterfaces.Healable);
            ImplementInterfaces(iInvulnerableInterfaces, cachedInterfaces.Invulnerable);
            ImplementInterfaces(iKillableInterfaces, cachedInterfaces.Killable);
            ImplementInterfaces(iReviveableInterfaces, cachedInterfaces.Reviveable);
            ImplementInterfaces(iDamageCallbackInterfaces, cachedInterfaces.DamageCallback);
            ImplementInterfaces(iHealCallbackInterfaces, cachedInterfaces.HealCallback);

            Configure(currentHealth, totalHealth, true);
        }

        private void OnDestroy()
        {
            healthInstances.Remove(this.gameObject.GetInstanceID());

            if (healthInstances.Count == 0)
            {
                healthInstances = null;
            }

# if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                HealthValidator.Remove(this);
            }
#endif
        }

        private void OnEnable()
        {
            if (reviveOnEnable && !isAlive)
            {
                Revive();
            }
        }


        public void Configure(float currentHealth, float totalHealth, bool markAlive = true)
        {
            this.currentHealth = currentHealth;
            this.totalHealth = totalHealth;

            if (clampHealth)
            {
                this.currentHealth = Mathf.Clamp(this.currentHealth, 0, totalHealth);
            }

            if (markAlive)
            {
                isAlive = true;
            }

            NotifyConfigureInterfaces(currentHealth, totalHealth);
        }

        /// <summary>
        /// Become invulnerable for all damage sources for a given amount of time.
        /// This invokes the IInvulnerable interfaces.
        /// </summary>
        public void SetInvulnerable(float time)
        {
            this.isInvulnerable = true;

            NofifyInvulnerableInterfaces(true);

            if (time == 0)
            {
                return;
            }

            StartCoroutine(RecoverVulnerabilityCoroutine(time));
        }

        public void SetInvulnerable(bool state)
        {
            isInvulnerable = state;

            NofifyInvulnerableInterfaces(state);
        }

        private IEnumerator RecoverVulnerabilityCoroutine(float time)
        {
            yield return new WaitForSeconds(time);

            SetInvulnerable(false);
        }


        // Simplified method for compatability with UnityEvents
        public void Heal(float amount)
        {
            Heal(amount, this.transform.position, null);
        }

        /// <summary>
        /// Heals the health towards a for percentage. The percentage means the total health * percentage restored.
        /// </summary>
        /// <param name="cause"></param>
        /// <param name="percentage"> 0 = 0%, 1 = 100% </param>
        public void Heal(float percentage = 1, Health cause = null)
        {
            Heal((percentage == 100) ? totalHealth : totalHealth * percentage, this.transform.position, cause);
        }

        public void Heal(float amount, Vector3 hitLocation, Health healer)
        {
            if (!isAlive)
                return;

            if (amount < 0)
            {
                Damage(Mathf.Abs(amount), hitLocation, healer);
                return;
            }

            for (int i = 0; i < iHealthCorrectionInterfaces.Count; i++)
            {
                HealthCorrectionData data = iHealthCorrectionInterfaces[i].
                    OnCorrectHeal(amount, (healer != null) ? healer.gameObject.GetInstanceID() : 0);

                if (data.stopExecution)
                    return;

                amount = data.amount;
            }

            float previousHealth = currentHealth;

            currentHealth += amount;

            if (clampHealth)
            {
                currentHealth = Mathf.Clamp(currentHealth, 0, totalHealth);
            }

            HealthInfo healInfo = new HealthInfo
            (
                previousHealth,
                currentHealth,
                totalHealth,
                hitLocation,
                healer == null ? 0 : healer.gameObject.GetInstanceID(),
                this.gameObject.GetInstanceID(),
                amount
            );

            NotifyHealableInterfaces(healInfo);

            if (healer != null)
            {
                NotifyHealCallbackInterfaces(healer, healInfo);
            }
        }

        // Simplified method for compatability with UnityEvents
        public void Damage(float amount)
        {
            Damage(amount, this.transform.position, null);
        }

        public void Damage(float amount, Vector3 hitLocation, Health attacker)
        {
            if (isInvulnerable)
                return;

            if (amount < 0)
            {
                Heal(Mathf.Abs(amount), hitLocation, attacker);
                return;
            }

            for (int i = 0; i < iHealthCorrectionInterfaces.Count; i++)
            {
                HealthCorrectionData data = iHealthCorrectionInterfaces[i].
                    OnCorrectDamage(amount, (attacker != null) ? attacker.gameObject.GetInstanceID() : 0);

                if (data.stopExecution)
                {
                    return;
                }

                amount = data.amount;
            }

            float previousHealth = currentHealth;

            currentHealth -= amount;

            if (clampHealth)
            {
                currentHealth = Mathf.Clamp(currentHealth, 0, totalHealth);
            }

            HealthInfo healthInfo = new HealthInfo
            (
                previousHealth,
                currentHealth,
                totalHealth,
                hitLocation,
                attacker == null ? 0 : attacker.gameObject.GetInstanceID(),
                this.gameObject.GetInstanceID(),
                amount
            );

            if (isAlive)
            {
                if (iDamageableInterfaces.Count > 0)
                {
                    NofifyDamageableInterfaces(healthInfo);
                }

                if (attacker != null)
                {
                    NofifyDamageCallbackInterfaces(attacker, healthInfo);
                }
            }

            // In case health is below zero, we invoke death
            if (currentHealth <= 0 && isAlive)
            {
                isAlive = false;

                NofifyKillableInterfaces(healthInfo);
            }
        }

        // Simplified method for compatability with UnityEvents
        public void Revive()
        {
            Revive(1, null);
        }

        /// <summary>
        /// Resets the health to given percentage. And calls the OnRevive interface method on listeners.
        /// It also marks the object as alive again. Meaning it can be damaged and healed.
        /// </summary>
        /// <param name="cause"> Health component of the attacker, to include within the revive info </param>
        /// <param name="percentage"> Percentage of health to revive towards, 1 = 100%, 0.6 = 60% etc. </param>
        public void Revive(float percentage = 1, Health cause = null)
        {
            if (isAlive)
            {
                Debug.LogWarning(string.Format("Attempted to revive object: {0} " +
                    "with as cause: {1}. While the object was already alive.", this.gameObject.name, (cause != null) ? cause.name : "Null"));
                return;
            }

            for (int i = 0; i < iHealthCorrectionInterfaces.Count; i++)
            {
                HealthCorrectionData data = iHealthCorrectionInterfaces[i].
                    OnCorrectRevive(percentage, (cause != null) ? cause.gameObject.GetInstanceID() : 0);

                percentage = data.amount;

                if (data.stopExecution)
                    return;
            }

            isAlive = true;

            currentHealth = (percentage == 100) ? totalHealth : totalHealth * percentage;

            HealthInfo info = new HealthInfo
            (
                0,
                currentHealth,
                totalHealth,
                this.transform.position,
                cause == null ? 0 : cause.gameObject.GetInstanceID(),
                this.gameObject.GetInstanceID(),
                currentHealth
            );

            NofifyReviveableInterfaces(info);
        }

        // Simplified method for compatability with UnityEvents
        public void Kill(bool applyAsDamage)
        {
            Kill(applyAsDamage, this.transform.position, null);
        }

        /// <summary>
        /// Will zero out the health and invoke effect interfaces. By applying as damage
        /// it will still process corrections and stop if invulnerable.
        /// </summary>
        /// <param name="applyAsDamage"> This will have the same effect as calling Damage() for the total health </param>
        /// <param name="cause"></param>
        public void Kill(bool applyAsDamage, Vector3 hitLocation, Health attacker = null)
        {
            if (!isAlive)
                return;

            if (applyAsDamage)
            {
                Damage(totalHealth, hitLocation, attacker);
                return;
            }

            float previousHealth = 0;

            currentHealth = 0;

            isAlive = false;

            HealthInfo healthInfo = new HealthInfo
            (
                previousHealth,
                currentHealth,
                totalHealth,
                this.transform.position,
                attacker == null ? 0 : attacker.gameObject.GetInstanceID(),
                this.gameObject.GetInstanceID(),
                totalHealth
            );

            NofifyKillableInterfaces(healthInfo);
        }

        private void NofifyKillableInterfaces(HealthInfo healthInfo)
        {
            for (int i = iKillableInterfaces.Count - 1; i >= 0; i--)
            {
                if (!System.Object.ReferenceEquals(iKillableInterfaces[i], null))
                {
                    iKillableInterfaces[i].OnDeath(healthInfo);
                }
                else
                {
                    iKillableInterfaces.RemoveAt(i);
                }
            }
        }

        private void NofifyReviveableInterfaces(HealthInfo healthInfo)
        {
            for (int i = iReviveableInterfaces.Count - 1; i >= 0; i--)
            {
                if (!System.Object.ReferenceEquals(iReviveableInterfaces[i], null))
                {
                    iReviveableInterfaces[i].OnRevive(healthInfo);
                }
                else
                {
                    iReviveableInterfaces.RemoveAt(i);
                }
            }
        }

        private void NofifyDamageableInterfaces(HealthInfo healthInfo)
        {
            for (int i = 0; i < iDamageableInterfaces.Count; i++)
            {
                if (!System.Object.ReferenceEquals(iDamageableInterfaces[i], null))
                {
                    iDamageableInterfaces[i].OnDamaged(healthInfo);
                }
                else
                {
                    iDamageableInterfaces.RemoveAt(i);
                }
            }
        }

        private void NofifyInvulnerableInterfaces(bool state)
        {
            for (int i = 0; i < iInvulnerableInterfaces.Count; i++)
            {
                if (!System.Object.ReferenceEquals(iInvulnerableInterfaces[i], null))
                {
                    iInvulnerableInterfaces[i].BecameInvulnerable(state);
                }
                else
                {
                    iInvulnerableInterfaces.RemoveAt(i);
                }
            }
        }

        private void NofifyDamageCallbackInterfaces(Health attacker, HealthInfo healthInfo)
        {
            for (int i = 0; i < attacker.iDamageCallbackInterfaces.Count; i++)
            {
                if (!System.Object.ReferenceEquals(attacker.iDamageCallbackInterfaces[i], null))
                {
                    attacker.iDamageCallbackInterfaces[i].OnDamageDone(healthInfo);
                }
                else
                {
                    attacker.iDamageCallbackInterfaces.RemoveAt(i);
                }
            }
        }

        private void NotifyHealCallbackInterfaces(Health healer, HealthInfo healInfo)
        {
            for (int i = 0; i < healer.iHealCallbackInterfaces.Count; i++)
            {
                if (!System.Object.ReferenceEquals(healer.iHealCallbackInterfaces[i], null))
                {
                    healer.iHealCallbackInterfaces[i].OnHealDone(healInfo);
                }
                else
                {
                    healer.iHealCallbackInterfaces.RemoveAt(i);
                }
            }
        }

        private void NotifyHealableInterfaces(HealthInfo healInfo)
        {
            for (int i = 0; i < iHealableInterfaces.Count; i++)
            {
                if (!System.Object.ReferenceEquals(iHealableInterfaces[i], null))
                {
                    iHealableInterfaces[i].OnHealed(healInfo);
                }
                else
                {
                    iHealableInterfaces.RemoveAt(i);
                }
            }
        }

        private void NotifyConfigureInterfaces(float currentHealth, float totalHealth)
        {
            for (int i = 0; i < iHealthConfigureInterfaces.Count; i++)
            {
                if (!System.Object.ReferenceEquals(iHealthConfigureInterfaces[i], null))
                {
                    iHealthConfigureInterfaces[i].OnHealthConfigure(
                        new HealthInfo(
                            currentHealth,
                            currentHealth,
                            totalHealth,
                            Vector3.zero,
                            0,
                            this.gameObject.GetInstanceID(),
                            0));
                }
                else
                {
                    iHealthConfigureInterfaces.RemoveAt(i);
                }
            }
        }
    }
}