using System;
using FGJ2020_Team3.DialogSystem;
using UnityEngine;

namespace FGJ2020_Team3.Character
{
    public class CharacterDialogTrigger : MonoBehaviour
    {
        // public DialogCharacter DialogCharacter; 
        private void OnTriggerEnter2D(Collider2D other)
        {
            var characterDialogger = other.GetComponent<CharacterDialogger>();
            if (characterDialogger != null)
            {
                Debug.Log($"Trigger");
            }
        }
    }
}