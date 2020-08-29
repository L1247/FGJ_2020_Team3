using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Health Effect/Health Effect Fade On Death")]
    public class HealthEffectFadeOnDeath : MonoBehaviour, IKillable, IReviveable
    {
        [SerializeField]
        private float startWaitMin = 0, startWaitMax = 0;

        [SerializeField]
        private float fadeDuration = 0.5f;

        [SerializeField]
        private List<Renderer> renderers = new List<Renderer>();

        private List<bool[]> rendererStartValidProperty = new List<bool[]>();
        private List<Material[]> rendererStartMaterials = new List<Material[]>();
        private List<Material[]> rendererFlashMaterials = new List<Material[]>();

        [SerializeField]
        private Shader shader = null;

        [SerializeField]
        private string property = "_Color";

        private int colorHash;

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

            if (renderers.Count == 0 && spriteRenderers.Length > 0)
            {
                shader = Shader.Find("Sprites/Default");
            }

            foreach (SpriteRenderer item in spriteRenderers)
            {
                renderers.Add(item);
            }
        }

        private Material CreateFadeMaterial(Material source)
        {
            Material fadeMaterial = new Material(source);
            fadeMaterial.shader = shader;
            return fadeMaterial;
        }

        private void Start()
        {
            colorHash = Shader.PropertyToID(property);

            if (renderers.Count > 0)
            {
                // Create a flash material for each material found on the renderer.
                for (int i = 0; i < renderers.Count; i++)
                {
                    Material[] getRendererMaterials = renderers[i].sharedMaterials;
                    Material[] fadeMaterials = new Material[getRendererMaterials.Length];
                    bool[] hasColorProperty = new bool[getRendererMaterials.Length];

                    for (int i2 = 0; i2 < getRendererMaterials.Length; i2++)
                    {
                        Material createFlashMaterial = CreateFadeMaterial(getRendererMaterials[i2]);
                        fadeMaterials[i2] = createFlashMaterial;

                        hasColorProperty[i2] = getRendererMaterials[i2].HasProperty(property);
                    }

                    rendererStartMaterials.Add(getRendererMaterials);
                    rendererFlashMaterials.Add(fadeMaterials);
                    rendererStartValidProperty.Add(hasColorProperty);
                }
            }
        }
        public void OnDeath(HealthInfo healthInfo)
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(FadeCoroutine());
            }
        }

        private IEnumerator FadeCoroutine()
        {
            yield return new WaitForSeconds(Random.Range(startWaitMin, startWaitMax));

            float t = 0;

            Color targetColor = Color.white;
            targetColor.a = 0;

            for (int i = 0; i < rendererFlashMaterials.Count; i++)
            {
                for (int i2 = 0; i2 < rendererFlashMaterials[i].Length; i2++)
                {
                    rendererFlashMaterials[i][i2].SetColor(colorHash, 
                        rendererStartValidProperty[i][i2]? rendererStartMaterials[i][i2].GetColor(colorHash) : Color.white);
                }
            }

            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].materials = rendererFlashMaterials[i];
            }

            while (t < fadeDuration)
            {
                for (int i = 0; i < rendererFlashMaterials.Count; i++)
                {
                    for (int i2 = 0; i2 < rendererFlashMaterials[i].Length; i2++)
                    {
                        rendererFlashMaterials[i][i2].SetColor(
                            colorHash,
                            Color.Lerp(rendererFlashMaterials[i][i2].GetColor(colorHash), targetColor, t / fadeDuration));
                    }
                }

                t += Time.deltaTime;
                yield return null;
            }
        }

        public void OnRevive(HealthInfo info)
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].materials = rendererStartMaterials[i];
            }
        }
    }
}