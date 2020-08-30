using System;
using UnityEngine;

namespace FGJ2020_Team3.DialogSystem
{
    public class DialogManager : MonoBehaviour
    {
        public DialogSystem dialogSystemCharacter , DialogSystemWitch;
        private void Start()
        {
            dialogSystemCharacter.CallNext();
        }

        public void StartDialog()
        {
            
        }

        public void StopDialog()
        {
            
        }
    }
}