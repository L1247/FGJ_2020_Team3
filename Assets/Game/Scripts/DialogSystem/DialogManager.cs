using System;
using DG.Tweening;
using Game.Scripts.Utility.TextUtility;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FGJ2020_Team3.DialogSystem
{
    public class DialogManager : MonoBehaviour
    {
        public DialogSystem dialogSystemCharacter , DialogSystemWitch;
        public string       NextScene;


        private void Start()
        {
            dialogSystemCharacter.CallNext();
            dialogSystemCharacter
                .GetDialogEnd
                .Subscribe(unit =>
                {
                    // Delay
                    Observable.Timer(TimeSpan.FromSeconds(1.5f))
                              .Subscribe(l => CallCharacterMove());
                });
        }

        private void CallCharacterMove()
        {
            DialogSystemWitch.GetComponentInChildren<CanvasGroup>().alpha = 0;
            dialogSystemCharacter.GetComponentInChildren<CanvasGroup>().alpha = 0;
            DialogSystemWitch.transform.DOMoveY(10 , 1)
                                 .OnComplete(() =>
                                 {
                                     dialogSystemCharacter.transform.DOMoveY(10 , 1)
                                                          .OnComplete(() => ChangeScene());
                                 });
        }

        private void ChangeScene()
        {
            SceneManager.LoadScene(NextScene);
        }

        public void StartDialog()
        {
            
        }

        public void StopDialog()
        {
            
        }
    }
}