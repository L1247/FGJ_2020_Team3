using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Utility.TextUtility
{
    public class UITextTypeWriter : MonoBehaviour
    {
        public  IObservable<Unit> GetTypeEnd => OnTypeEnd;
        private Subject<Unit>     OnTypeEnd = new Subject<Unit>();

        [SerializeField] private float typeSpeed = 0.125f;

        Text   txt;
        string story;

        void Awake()
        {
            txt = GetComponent<Text>();
            // story    = txt.text;
            txt.text = "";

            // TODO: add optional delay when to start
        }

        public void CallNextEffect(string context)
        {
            txt.text = "";
            story    = context;
            StopCoroutine("PlayText");
            StartCoroutine("PlayText");
        }

        IEnumerator PlayText()
        {
            foreach (char c in story)
            {
                txt.text += c;
                yield return new WaitForSeconds(typeSpeed);
            }
            OnTypeEnd.OnNext(Unit.Default);
        }
    }
}