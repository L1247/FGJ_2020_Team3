using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Utility/Health Modification Volume 2D")]
    public class HealthModificationVolume2D : HealthModificationVolumeBase
    {

#if UNITY_EDITOR

        [MenuItem(itemName: "GameObject/Health/Health Modification Volume 2D")]
        private static void CreateVolume()
        {
            GameObject createObject = new GameObject();
            createObject.gameObject.AddComponent<BoxCollider2D>();
            createObject.AddComponent<HealthModificationVolume2D>();

            createObject.name = "Health Modification Volume 2D";
            Selection.activeGameObject = createObject.gameObject;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

#endif

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            ApplyTarget(collision.gameObject);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            RemoveTarget(collision.gameObject);
        }
    }
}