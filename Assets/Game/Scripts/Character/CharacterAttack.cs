using System;
using UnityEngine;

namespace FGJ2020_Team3.Character
{
    public class CharacterAttack : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log($"Click");
                
            }
        }
    }
}