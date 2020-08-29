using System;
using System.Collections.Generic;
using UnityEngine;

namespace FGJ2020_Team3.DialogSystem
{
    public class DialogSystem : UnityEngine.MonoBehaviour
    {
        public int DialogID = 1;
        
        [SerializeField]
        private List<Dialog> _dialogs;
    }

    [System.Serializable]
    internal class Dialog
    {
        public DialogCharacter DialogCharacter;
        [Multiline]
        public string          Context;
    }

    public enum DialogCharacter
    {
        主角,
        女巫,
        龍
    }
}