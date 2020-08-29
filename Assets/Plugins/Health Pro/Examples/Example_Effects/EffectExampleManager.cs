using Lowscope.HealthPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lowscope.HealthPro.Examples.Effects
{
    public class EffectExampleManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject groupContainer = null;

        private GameObject[] exampleGroups = null;

        [SerializeField]
        private Text textExampleName = null;

        [SerializeField]
        private Button buttonNext = null;

        [SerializeField]
        private Button buttonPrevious = null;

        [SerializeField]
        private Button buttonDoDamage = null;

        [SerializeField]
        private Button buttonDoHeal = null;

        [SerializeField]
        private Button buttonDoRevive = null;

        private int currentExampleID;
        private Health health;

        private void Awake()
        {
            Application.targetFrameRate = 120;

            exampleGroups = new GameObject[groupContainer.transform.childCount];

            for (int i = 0; i < exampleGroups.Length; i++)
            {
                exampleGroups[i] = groupContainer.transform.GetChild(i).gameObject;
            }

            health = GetComponent<Health>();

            buttonNext.onClick.AddListener(DisableCurrentExample);
            buttonPrevious.onClick.AddListener(DisableCurrentExample);

            buttonNext.onClick.AddListener(NextExample);
            buttonPrevious.onClick.AddListener(PreviousExample);

            buttonDoDamage.onClick.AddListener(DoDamage);
            buttonDoHeal.onClick.AddListener(DoHeal);
            buttonDoRevive.onClick.AddListener(DoRevive);


            SetActiveExample(0);
        }

        private void DoRevive()
        {
            Health getHealth = exampleGroups[currentExampleID].GetComponentInChildren<Health>(true);
            getHealth.Revive();
        }

        private void DoHeal()
        {
            Health getHealth = exampleGroups[currentExampleID].GetComponentInChildren<Health>(true);
            Vector3 getPosition = getHealth.transform.position;

            getHealth.Heal(1, getPosition, health);
        }

        private void DoDamage()
        {
            Health getHealth = exampleGroups[currentExampleID].GetComponentInChildren<Health>(true);
            Vector3 getPosition = getHealth.transform.position;

            getHealth.Damage(1, getPosition, health);
        }

        private void DisableCurrentExample()
        {
            exampleGroups[currentExampleID].gameObject.SetActive(false);
        }

        private void PreviousExample()
        {
            currentExampleID--;
            if (currentExampleID < 0)
            {
                currentExampleID = exampleGroups.Length - 1;
            }

            SetActiveExample(currentExampleID);
        }

        private void NextExample()
        {
            currentExampleID++;
            if (currentExampleID >= exampleGroups.Length)
            {
                currentExampleID = 0;
            }

            SetActiveExample(currentExampleID);
        }

        private void SetActiveExample(int index)
        {
            if (index >= 0 && index < exampleGroups.Length)
            {
                exampleGroups[index].gameObject.SetActive(true);

                textExampleName.text = string.Format("{0} ({1}/{2})", exampleGroups[index].gameObject.name, index + 1, exampleGroups.Length);
            }
        }
    }
}