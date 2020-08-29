using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lowscope.HealthPro
{

    [AddComponentMenu("Health/Health Effect/Health Effect Fade On Invulnerable")]
    public class HealthEffectFadeOnInvulnerable : MonoBehaviour, IInvulnerable
    {
        [SerializeField]
        private List<Renderer> renderers = new List<Renderer>();

        [SerializeField]
        private float minAlpha = 0.5f;

        [SerializeField]
        private bool pulseAlpha = true;

        [SerializeField]
        private float pulseSpeed = 5;

        [SerializeField]
        private float transitionTime = 0.2f;

        private List<Material[]> rendererStartMaterials = new List<Material[]>();
        private List<Material[]> rendererInvisMaterials = new List<Material[]>();
        private List<bool[]> rendererHasColors = new List<bool[]>();

        [SerializeField]
        private Shader invisibilityShader = null;

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

            foreach (SpriteRenderer item in GetComponentsInChildren<SpriteRenderer>(true))
            {
                renderers.Add(item);
            }
        }

        private Material CreateFadeMaterial(Material source)
        {
            Material flashMaterial = new Material(source);
            flashMaterial.shader = invisibilityShader;
            return flashMaterial;
        }

        private void Start()
        {
            if (renderers.Count > 0)
            {
                // Create a flash material for each material found on the renderer.
                for (int i = 0; i < renderers.Count; i++)
                {
                    Material[] getRendererMaterials = renderers[i].sharedMaterials;
                    Material[] invisMaterials = new Material[getRendererMaterials.Length];
                    bool[] hasColorProperty = new bool[getRendererMaterials.Length];

                    for (int i2 = 0; i2 < getRendererMaterials.Length; i2++)
                    {
                        Material createFlashMaterial = CreateFadeMaterial(getRendererMaterials[i2]);
                        invisMaterials[i2] = createFlashMaterial;
                        hasColorProperty[i2] = getRendererMaterials[i2].HasProperty("_Color");
                    }

                    rendererStartMaterials.Add(getRendererMaterials);
                    rendererInvisMaterials.Add(invisMaterials);
                    rendererHasColors.Add(hasColorProperty);
                }
            }
        }

        public void BecameInvulnerable(bool isInvulerable)
        {
            if (!this.gameObject.activeSelf)
            {
                return;
            }

            if (isInvulerable)
            {
                StopAllCoroutines();
                StartCoroutine(InvisibilityCoroutine());
            }
            else
            {
                StopAllCoroutines();
                StartCoroutine(RecoverCoroutine());
            }
        }

        private IEnumerator RecoverCoroutine()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].materials = rendererInvisMaterials[i];
            }

            float t = 0;

            if (transitionTime > 0)
            {
                while (t < transitionTime)
                {

                    for (int i = 0; i < rendererStartMaterials.Count; i++)
                    {
                        for (int i2 = 0; i2 < rendererStartMaterials[i].Length; i2++)
                        {
                            Color targetColor = rendererHasColors[i][i2] ? rendererStartMaterials[i][i2].GetColor("_Color") : Color.white;
                            targetColor.a = Mathf.Lerp(0, 1, t / transitionTime);

                            rendererInvisMaterials[i][i2].SetColor("_Color", targetColor);
                        }
                    }

                    yield return null;
                    t += Time.deltaTime;
                }
            }

            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].materials = rendererStartMaterials[i];
            }
        }

        private IEnumerator InvisibilityCoroutine()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].materials = rendererInvisMaterials[i];
            }

            if (!pulseAlpha)
            {
                if (transitionTime <= 0)
                {

                    for (int i = 0; i < rendererStartMaterials.Count; i++)
                    {
                        for (int i2 = 0; i2 < rendererStartMaterials[i].Length; i2++)
                        {
                            Color targetColor = rendererHasColors[i][i2] ? rendererStartMaterials[i][i2].GetColor("_Color") : Color.white ;
                            targetColor.a = minAlpha;

                            rendererInvisMaterials[i][i2].SetColor("_Color", targetColor);
                        }
                    }

                    yield break;
                }

                float t = 0;

                while (t < transitionTime)
                {
                    for (int i = 0; i < rendererStartMaterials.Count; i++)
                    {
                        for (int i2 = 0; i2 < rendererStartMaterials[i].Length; i2++)
                        {
                            Color targetColor = rendererHasColors[i][i2]? rendererStartMaterials[i][i2].GetColor("_Color") : Color.white;
                            targetColor.a = Mathf.Lerp(1, minAlpha, t / transitionTime);

                            rendererInvisMaterials[i][i2].SetColor("_Color", targetColor);
                        }
                    }

                    yield return null;
                    t += Time.deltaTime;
                }

                yield break;
            }

            float currentTime = Time.time;

            while (true)
            {
                for (int i = 0; i < rendererStartMaterials.Count; i++)
                {
                    for (int i2 = 0; i2 < rendererStartMaterials[i].Length; i2++)
                    {
                        Color targetColor = rendererHasColors[i][i2] ? rendererStartMaterials[i][i2].GetColor("_Color") : Color.white;
                        targetColor.a = Mathf.Max(minAlpha, (Mathf.Cos((currentTime - Time.time) * pulseSpeed) * 0.5f) + 0.5f);

                        rendererInvisMaterials[i][i2].SetColor("_Color", targetColor);
                    }
                }

                yield return null;
            }
        }

    }

}