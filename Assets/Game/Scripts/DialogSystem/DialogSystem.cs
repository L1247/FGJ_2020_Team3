using System;
using System.Collections.Generic;
using Game.Scripts.Utility.TextUtility;
using UniRx;
using UnityEngine;

namespace FGJ2020_Team3.DialogSystem
{
    public class DialogSystem : MonoBehaviour
    {
        [SerializeField] private List<Dialog>      _dialogs;
        [SerializeField] private UITextTypeWriter  _textTypeWriter;
        [SerializeField] private UITextTypeWriter  _otherTextTypeWriter;
        [SerializeField] private Canvas            _canvas;
        public                   IObservable<Unit> GetDialogEnd => _onDialogEnd;
        private                  Subject<Unit>     _onDialogEnd = new Subject<Unit>();
        
        private                  int              _diglogIndex;

        private void Start()
        {
            _otherTextTypeWriter.GetTypeEnd
                                .Subscribe(unit =>
                                {
                                    if(_diglogIndex < _dialogs.Count) CallNext();
                                }).AddTo(gameObject);

        }

        public void CallNext()
        {
            _canvas.gameObject.GetComponent<CanvasGroup>().alpha = 1;
            var dialog = _dialogs[_diglogIndex];
            _textTypeWriter.CallNextEffect(dialog.Context);
            _diglogIndex++;
            if(_diglogIndex >= _dialogs.Count)
               _textTypeWriter.GetTypeEnd.Subscribe(unit => _onDialogEnd.OnNext(Unit.Default));
        }
    }

    [Serializable]
    internal class Dialog
    {
        [Multiline] public string Context;
    }
}