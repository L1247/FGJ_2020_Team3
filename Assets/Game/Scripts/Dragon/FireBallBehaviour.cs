using System;
using UnityEngine;

namespace Game.Scripts.Dragon
{
    public class FireBallBehaviour : MonoBehaviour
    {
        private void Start()
        {
            Destroy(gameObject , 5);
        }

        private void Update()
        {
            
        }
    }
}