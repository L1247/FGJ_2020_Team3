using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Flash Renderer On Damaged")]
    public class HealthEffectFlashRendererOnDamaged : MonoBehaviour, IDamageable, IKillable
    {
        [SerializeField]
        private List<Renderer> renderers = new List<Renderer>();

        private List<Material[]> rendererStartMaterials = new List<Material[]>();
        private List<Material[]> rendererFlashMaterials = new List<Material[]>();

        private static int intensityPropertyHash = 0;

        [SerializeField]
        private Color flashColor = Color.white;

        [SerializeField, Range(0,1), Tooltip("1 = Full White, 0 = No Fresnel or Flash, Only applies to 3D shaders.")]
        private float fresnelPower = 1f;

        [SerializeField]
        private Shader flashShader = null;

        [SerializeField]
        private float flashTime = 0.5f;

        [SerializeField]
        public AnimationCurve intensityCurve = new AnimationCurve(
            new Keyframe(0, 0),
            new Keyframe(0.05f, 1, 0, 0),
            new Keyframe(1, 0));

        // This happens only once, when the object is added or reset to defaults is done.
        private void Reset()
        {
            foreach (MeshRenderer item in GetComponentsInChildren<MeshRenderer>(true))
            {
                renderers.Add(item);
            }

            foreach (SkinnedMeshRenderer item in GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                renderers.Add(item);
            }

            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

            // In case we can only find sprite renderers, 
            // we will set the default shader to a sprite one.
            if (spriteRenderers.Length > 0 && renderers.Count == 0)
            {
                flashShader = Shader.Find("Health/Hit Shader/Sprite");
            }

            foreach (SpriteRenderer item in spriteRenderers)
            {
                renderers.Add(item);
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                for (int i = 0; i < rendererFlashMaterials.Count; i++)
                {
                    for (int i2 = 0; i2 < rendererFlashMaterials[i].Length; i2++)
                    {
                        rendererFlashMaterials[i][i2].SetColor("_FlashColor", flashColor);
                        rendererFlashMaterials[i][i2].SetFloat("_FresnelPower", Mathf.Lerp(3, 0, fresnelPower));
                    }
                }
            }
        }

        private Material CreateFlashMaterial(Material source)
        {
            Material flashMaterial = new Material(source);
            flashMaterial.shader = flashShader;
            flashMaterial.SetColor("_FlashColor", flashColor);
            flashMaterial.SetFloat("_FresnelPower", Mathf.Lerp(3, 0, fresnelPower));
            return flashMaterial;
        }

        private void Start()
        {
            if (intensityPropertyHash == 0)
            {
                intensityPropertyHash = Shader.PropertyToID("_Intensity");
            }

            if (renderers.Count > 0)
            {
                // Create a flash material for each material found on the renderer.
                for (int i = 0; i < renderers.Count; i++)
                {
                    Material[] getRendererMaterials = renderers[i].sharedMaterials;
                    Material[] flashMaterials = new Material[getRendererMaterials.Length];

                    for (int i2 = 0; i2 < getRendererMaterials.Length; i2++)
                    {
                        Material createFlashMaterial = CreateFlashMaterial(getRendererMaterials[i2]);
                        flashMaterials[i2] = createFlashMaterial;
                    }

                    rendererStartMaterials.Add(getRendererMaterials);
                    rendererFlashMaterials.Add(flashMaterials);
                }
            }
        }

        public void OnDamaged(HealthInfo info)
        {
            StopAllCoroutines();

            if (this.gameObject.activeSelf)
            {
                StartCoroutine(HitFlashCoroutine());
            }
        }

        IEnumerator HitFlashCoroutine()
        {
            float t = 0;

            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].materials = rendererFlashMaterials[i];
            }

            while (t < flashTime)
            {
                for (int i = 0; i < rendererFlashMaterials.Count; i++)
                {
                    for (int i2 = 0; i2 < rendererFlashMaterials[i].Length; i2++)
                    {
                        float curveEvaluation = Mathf.Clamp(intensityCurve.Evaluate(t / flashTime), 0, 1);
                        rendererFlashMaterials[i][i2].SetFloat(intensityPropertyHash, curveEvaluation);
                    }
                }

                t += Time.deltaTime;
                yield return null;
            }

            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].materials = rendererStartMaterials[i];
            }
        }

        public void OnDeath(HealthInfo healthInfo)
        {
            StopAllCoroutines();

            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].materials = rendererStartMaterials[i];
            }
        }
    }
}