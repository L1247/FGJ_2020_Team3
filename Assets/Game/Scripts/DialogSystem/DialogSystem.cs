using System;
using System.Collections.Generic;
using Game.Scripts.Utility.TextUtility;
using UniRx;
using UnityEngine;

namespace FGJ2020_Team3.DialogSystem
{
    public class DialogSystem : MonoBehaviour
    {
        [SerializeField] private List<Dialog>     _dialogs;
        [SerializeField] private UITextTypeWriter _textTypeWriter;

        private int _diglogIndex;

        private void Start()
        {
            _textTypeWriter.GetTypeEnd
                           .Subscribe(unit =>
                           {
                               if(_diglogIndex < _dialogs.Count)
                                   CallNext();
                           });
            CallNext();
        }

        private void CallNext()
        {
            var dialog = _dialogs[_diglogIndex];
            _textTypeWriter.CallNextEffect(dialog.Context);
            _diglogIndex++;
        }
    }

    [Serializable]
    internal class Dialog
    {
        [Multiline] public string Context;
    }
}