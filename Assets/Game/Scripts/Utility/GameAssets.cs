using UnityEngine;

namespace FGJ2020_Team3.Utility
{
    public class GameAssets : MonoBehaviour
    {
        private static GameAssets _instance;

        public static GameAssets Instance {
            get {
                if (_instance == null) _instance = Instantiate(Resources.Load<GameAssets>("GameAssets"));
                return _instance;
            }
        }

        public GameObject pfSwordSlash;
    }
}