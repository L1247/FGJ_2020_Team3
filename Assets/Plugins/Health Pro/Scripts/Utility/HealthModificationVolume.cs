using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Lowscope.HealthPro
{
    [AddComponentMenu("Health/Utility/Health Modification Volume")]
    public class HealthModificationVolume : HealthModificationVolumeBase
    {

#if UNITY_EDITOR

        [MenuItem(itemName: "GameObject/Health/Health Modification Volume")]
        private static void CreateVolume()
        {
            GameObject createObject = new GameObject();
            createObject.gameObject.AddComponent<BoxCollider>();
            createObject.AddComponent<HealthModificationVolume>();

            createObject.name = "Health Modification Volume";
            Selection.activeGameObject = createObject.gameObject;
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

#endif
        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider collision)
        {
            ApplyTarget(collision.gameObject);
        }

        private void OnTriggerExit(Collider collision)
        {
            RemoveTarget(collision.gameObject);
        }
    }
}