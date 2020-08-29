using MonsterLove.HealthPro.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Lowscope.HealthPro.Examples.Shooter
{
    [AddComponentMenu("")]
    public class ShooterPlayer : MonoBehaviour, IDamageCallback, IKillable, IDamageable
    {
        [SerializeField] private Transform gun = null;
        [SerializeField] private ParticleSystem gunMuzzleFlashParticle = null;
        [SerializeField] private Animator gunAnimator = null;
        [SerializeField] private AudioSource gunAudioSource = null;
        [SerializeField] private float shootCooldown = 0.1f;
        [SerializeField] private GameObject groundHitPrefab = null;
        [SerializeField] private Text scoreText = null;

        private float currentShootCooldown = 0;
        private Health stats;
        private new Camera camera;
        private Vector3 lastMousePosition;
        private int currentScore;
        private Vector3 aimDirection;

        private void Awake()
        {
            stats = this.gameObject.GetComponent<Health>();
            this.camera = Camera.main;
        }

        public void OnDamageDone(HealthInfo info)
        {
            // We verify if the target has died
            if (info.CurrentHealth <= 0)
            {
                currentScore++;
                scoreText.text = currentScore.ToString();
            }
        }

        void Update()
        {
            if (currentShootCooldown > 0)
            {
                currentShootCooldown -= Time.deltaTime;
            }

            if (Input.mousePosition != lastMousePosition)
            {
                aimDirection = camera.ScreenPointToRay(Input.mousePosition).direction;

                gun.transform.LookAt(-aimDirection * 50);

                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                if (currentShootCooldown > 0)
                    return;

                gunAnimator.PlayInFixedTime("Shoot");
                gunAudioSource.pitch = Random.Range(0.95f, 1.05f);
                gunAudioSource.Play();
                gunMuzzleFlashParticle.Play();

                RaycastHit hit;

                Ray screenRay = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(screenRay, out hit, 150))
                {
                    Health getStats = hit.collider.GetComponent<Health>();

                    if (getStats != null)
                    {
                        getStats.Damage(1, hit.point, stats);
                    }
                    else
                    {
                        SpawnHitDust(hit);
                    }
                }

                currentShootCooldown = shootCooldown;
            }
        }

        private void SpawnHitDust(RaycastHit raycastHit)
        {
            bool isRecycled;

            GameObject hitPrefab = PoolManager.SpawnObject(groundHitPrefab, raycastHit.point + new Vector3(0, 0.1f, 0),
                groundHitPrefab.transform.rotation, out isRecycled);

            if (!isRecycled)
            {
                hitPrefab.AddComponent<ReturnToPoolOnDisable>();
            }
        }

        public void OnDamaged(HealthInfo healthInfo)
        {
            stats.SetInvulnerable(2);
        }

        public void OnDeath(HealthInfo healthInfo)
        {
            enabled = false;
            gun.gameObject.SetActive(false);
            StartCoroutine(LookDownCoroutine());
        }

        IEnumerator LookDownCoroutine()
        {
            float t = 0;

            while (t < 1)
            {
                camera.transform.rotation = Quaternion.Euler(Mathf.Lerp(0, Mathf.SmoothStep(0,-90,t), t), 0, 0);
                t += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(1);

            SceneManager.LoadScene(0);
        }
    }
}