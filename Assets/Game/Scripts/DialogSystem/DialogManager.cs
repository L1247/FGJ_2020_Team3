using UnityEngine;

namespace FGJ2020_Team3.DialogSystem
{
    public class DialogManager
    {
        public static DialogManager Instance
        {
            get
            {
                if(_dialogManager == null)
                    _dialogManager = new DialogManager();
                return _dialogManager;
            }
        }

        private static DialogManager _dialogManager;

        public void StartDialog()
        {
            
        }

        public void StopDialog()
        {
            
        }
    }
}